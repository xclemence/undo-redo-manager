using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using Xce.TrackingItem.Attributes;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        private readonly ReferenceProvider referenceProvider;
        private readonly EqualsMethodAppender equalsMethodAppender;
        private readonly EqualsMethodProvider equalsMethodProvider;

        public ModuleWeaver()
        {
            referenceProvider = new ReferenceProvider(this);
            equalsMethodProvider = new EqualsMethodProvider(referenceProvider);
            equalsMethodAppender = new EqualsMethodAppender(equalsMethodProvider);
        }

        private IEnumerable<TypeDefinition> GetTrackingTypes()
        {
            return ModuleDefinition.Types.Where(x => x.IsClass && x.HasCustomAttributes && x.CustomAttributes.Any(a => a.AttributeType.Name == nameof(TrackingAttribute)));

        }
        private IEnumerable<PropertyDefinition> GetTrackingProperties(TypeDefinition type)
        {
            return type.Properties
                             .Where(x => x.SetMethod != null && !x.CustomAttributes.Any(a => a.AttributeType.Name == nameof(NoPropertyTrackingAttribute)));
        }

        private IEnumerable<PropertyDefinition> GetCollectionProperties(TypeDefinition type)
        {
            return type.Properties.Where(x => x.GetMethod != null && x.CustomAttributes.Any(a => a.AttributeType.Name == nameof(CollectionTrackingAttribute)));
        }

        public override void Execute()
        {
            WriteInfo($"Start Fody Xce Tracking");

            foreach (var type in GetTrackingTypes())
            {
                WriteInfo($"Procceed Type {type.FullName}");
                var tracerField = InjectField(type);

                foreach (var property in GetTrackingProperties(type))
                    UpdatePropertyWithLog(property, tracerField);

                var collectionProperties = GetCollectionProperties(type);

                foreach (var property in collectionProperties)
                    AddCollectionChanged(type, property, tracerField);
            }

        }

        private void AddCollectionChanged(TypeDefinition type, PropertyDefinition property, FieldDefinition fieldTracking)
        {
            var collectionChangedMethod = AddCollectionTrackingMethod(type, property, fieldTracking);
            AddCollectionChangedConstructor(type, property, collectionChangedMethod);
            AddCollectionFinaliser(type, property, collectionChangedMethod);
        }

        public (MethodDefinition method, int insertPosition) AddFinalizer(TypeDefinition type)
        {
            var existingFinalizeMethod = type.Methods.FirstOrDefault(x => !x.IsStatic && x.Name == "Finalize");
            if (existingFinalizeMethod != null)
            {
                return (existingFinalizeMethod, 1);
            }

            var objectTypeDefinition = referenceProvider.GetTypeReference(typeof(object));
            var objectFinalizeReference = referenceProvider.GetMethodReference(objectTypeDefinition.Resolve().FindMethod("Finalize"));


            var finalizeMethod = new MethodDefinition("Finalize", MethodAttributes.HideBySig | MethodAttributes.Family | MethodAttributes.Virtual, TypeSystem.VoidReference);
            var instructions = finalizeMethod.Body.Instructions;

            var ret = Instruction.Create(OpCodes.Ret);

            var tryStart = Instruction.Create(OpCodes.Nop);
            instructions.Add(tryStart);

            instructions.Add(Instruction.Create(OpCodes.Leave, ret));
            var tryEnd = Instruction.Create(OpCodes.Ldarg_0);
            instructions.Add(tryEnd);
            instructions.Add(Instruction.Create(OpCodes.Call, objectFinalizeReference));
            instructions.Add(Instruction.Create(OpCodes.Endfinally));
            instructions.Add(ret);

            var finallyHandler = new ExceptionHandler(ExceptionHandlerType.Finally)
            {
                TryStart = tryStart,
                TryEnd = tryEnd,
                HandlerStart = tryEnd,
                HandlerEnd = ret
            };

            finalizeMethod.Body.ExceptionHandlers.Add(finallyHandler);
            type.Methods.Add(finalizeMethod);

            return (finalizeMethod, 1);
        }


        private MethodDefinition AddCollectionTrackingMethod(TypeDefinition type, PropertyDefinition property, FieldDefinition fieldTracking)
        {
            var method = new MethodDefinition($"TrackingCollection_{property.Name}", MethodAttributes.Private, TypeSystem.VoidReference);

            var itemParameter = new ParameterDefinition(referenceProvider.GetTypeReference(typeof(object)));
            var valueParameter = new ParameterDefinition(referenceProvider.GetTypeReference(typeof(NotifyCollectionChangedEventArgs)));

            method.Parameters.Add(itemParameter);
            method.Parameters.Add(valueParameter);

            var TrickingActionfactoryTypeDef = referenceProvider.GetTypeReference(typeof(TrackingActionFactory));
            var methodDefinition = TrickingActionfactoryTypeDef.Resolve().FindMethod(nameof(TrackingActionFactory.GetCollectionChangedTrackingActionLIst), typeof(IList<>).FullName, typeof(NotifyCollectionChangedEventArgs).FullName);
            var trackingActionFactoryMethodNoGen = referenceProvider.GetMethodReference(methodDefinition);
            var trackingActionFactoryMethod = new GenericInstanceMethod(trackingActionFactoryMethodNoGen);

            trackingActionFactoryMethod.GenericArguments.Add((property.PropertyType as GenericInstanceType).GenericArguments.First());

            var addActionMethod = ModuleDefinition.ImportReference(fieldTracking.FieldType.Resolve().FindMethod(nameof(TrackingManager.AddActions), typeof(IList<>).FullName));

            var instructions = method.Body.Instructions;

            instructions.InsertFirst(Instruction.Create(OpCodes.Ldarg_0),
                                     Instruction.Create(OpCodes.Ldfld, fieldTracking),
                                     Instruction.Create(OpCodes.Ldarg_0),
                                     Instruction.Create(OpCodes.Call, property.GetMethod),
                                     Instruction.Create(OpCodes.Ldarg_2),
                                     Instruction.Create(OpCodes.Call, trackingActionFactoryMethod),
                                     Instruction.Create(OpCodes.Callvirt, addActionMethod),
                                     Instruction.Create(OpCodes.Nop),
                                     Instruction.Create(OpCodes.Ret));

            property.DeclaringType.Methods.Add(method);

            return method;

           // IL_0000: ldarg.0
		   // IL_0001: ldfld class [Xce.TrackingItem]Xce.TrackingItem.TrackingManager Xce.TrackingItem.Fody.TestModel.ReferenceCollectionModel::trackingManager
		   // IL_0006: ldarg.0
		   // IL_0007: call instance class [netstandard]System.Collections.ObjectModel.ObservableCollection`1<int32> Xce.TrackingItem.Fody.TestModel.ReferenceCollectionModel::get_TestCollection()
		   // IL_000c: ldarg.2
		   // IL_000d: call class [netstandard]System.Collections.Generic.IList`1<class [Xce.TrackingItem]Xce.TrackingItem.TrackingAction.ITrackingAction> [Xce.TrackingItem]Xce.TrackingItem.TrackingActionFactory::GetCollectionChangedTrackingActionLIst<int32>(class [netstandard]System.Collections.Generic.IList`1<!!0>, class [netstandard]System.Collections.Specialized.NotifyCollectionChangedEventArgs)
		   // IL_0012: callvirt instance void [Xce.TrackingItem]Xce.TrackingItem.TrackingManager::AddActions(class [netstandard]System.Collections.Generic.IList`1<class [Xce.TrackingItem]Xce.TrackingItem.TrackingAction.ITrackingAction>)
		   // IL_0017: nop
		   // IL_0018: ret
        }

        private void AddCollectionChangedConstructor(TypeDefinition type, PropertyDefinition property, MethodDefinition collectionChangedMethod)
        {
            var instructions = type.GetConstructors().First().Body.Instructions;

            var insertPosition = instructions.FindRetPosition();

            AddCollectionChangeEventHandler(property, collectionChangedMethod, instructions, insertPosition, "add_CollectionChanged");

            //IL_0013: ldarg.0
            //IL_0014: call instance class [netstandard]System.Collections.ObjectModel.ObservableCollection`1<int32> Xce.TrackingItem.Fody.TestModel.ReferenceCollectionModel::get_TestCollection()
            //IL_0019: ldarg.0
            //IL_001a: ldftn instance void Xce.TrackingItem.Fody.TestModel.ReferenceCollectionModel::OnTestCollectionCollectionChanged(object, class [netstandard]System.Collections.Specialized.NotifyCollectionChangedEventArgs)
            //IL_0020: newobj instance void [netstandard]System.Collections.Specialized.NotifyCollectionChangedEventHandler::.ctor(object, native int)
            //IL_0025: callvirt instance void class [netstandard]System.Collections.ObjectModel.ObservableCollection`1<int32>::add_CollectionChanged(class [netstandard]System.Collections.Specialized.NotifyCollectionChangedEventHandler)
            //IL_002a: nop
        }

        private void AddCollectionFinaliser(TypeDefinition type, PropertyDefinition property, MethodDefinition collectionChangedMethod)
        {
            var (method, insertPosition) = AddFinalizer(type);

            var instructions = method.Body.Instructions;

            AddCollectionChangeEventHandler(property, collectionChangedMethod, instructions, insertPosition, "remove_CollectionChanged");

            // IL_0001: nop
            // IL_0002: ldarg.0
            // IL_0003: call instance class [netstandard]System.Collections.ObjectModel.ObservableCollection`1<int32> Xce.TrackingItem.Fody.TestModel.ReferenceCollectionModel2::get_TestCollection()
            // IL_0008: ldarg.0
            // IL_0009: ldftn instance void Xce.TrackingItem.Fody.TestModel.ReferenceCollectionModel2::OnTestCollectionCollectionChanged(object, class [netstandard]System.Collections.Specialized.NotifyCollectionChangedEventArgs)
            // IL_000f: newobj instance void [netstandard]System.Collections.Specialized.NotifyCollectionChangedEventHandler::.ctor(object, native int)
            // IL_0014: callvirt instance void class [netstandard]System.Collections.ObjectModel.ObservableCollection`1<int32>::remove_CollectionChanged(class [netstandard]System.Collections.Specialized.NotifyCollectionChangedEventHandler)
            // IL_002a: nop
        }

        private void AddCollectionChangeEventHandler(PropertyDefinition property, MethodDefinition collectionChangedMethod, Collection<Instruction> instructions, int insertPosition, string eventHandlerMethod)
        {
            var collectionChangedEventHandlerTypeDef = referenceProvider.GetTypeReference(typeof(NotifyCollectionChangedEventHandler));
            var collectionChangedEventHandlerCtor = referenceProvider.GetMethodReference(collectionChangedEventHandlerTypeDef.Resolve().FindMethod(".ctor", typeof(Object).FullName, typeof(IntPtr).FullName));

            var addMethodNoGen = property.PropertyType.Resolve().FindMethod(eventHandlerMethod, typeof(NotifyCollectionChangedEventHandler).FullName);

            MethodReference addCollectionChangedMethod;

            if (property.PropertyType is GenericInstanceType genericInstanceType)
            {
                var methodReferenceGen = addMethodNoGen.MakeHostInstanceGeneric(genericInstanceType.GenericArguments.ToArray());
                addCollectionChangedMethod = referenceProvider.GetMethodReference(methodReferenceGen);
            }
            else
            {
                addCollectionChangedMethod = referenceProvider.GetMethodReference(addMethodNoGen);
            }

            instructions.InsertList(insertPosition,
                                    Instruction.Create(OpCodes.Ldarg_0),
                                    Instruction.Create(OpCodes.Call, property.GetMethod),
                                    Instruction.Create(OpCodes.Ldarg_0),
                                    Instruction.Create(OpCodes.Ldftn, collectionChangedMethod),
                                    Instruction.Create(OpCodes.Newobj, collectionChangedEventHandlerCtor),
                                    Instruction.Create(OpCodes.Callvirt, addCollectionChangedMethod),
                                    Instruction.Create(OpCodes.Nop));
        }

        private FieldDefinition InjectField(TypeDefinition type)
        {
            var trackingManagerRefrence = referenceProvider.GetTypeReference(typeof(TrackingManager));

            var trackingField = new FieldDefinition("trackingManager", FieldAttributes.Private, trackingManagerRefrence);

            var trackingProviderTypeDef = referenceProvider.GetTypeReference(typeof(TrackingManagerProvider));
            var tackingManagerProviderMethod = referenceProvider.GetMethodReference(trackingProviderTypeDef.Resolve().FindMethod(nameof(TrackingManagerProvider.GetDefault)));
            
            var instructions = type.GetConstructors().First().Body.Instructions;

            var insertPosition = instructions.FindRetPosition();

            instructions.InsertList(insertPosition, 
                                    Instruction.Create(OpCodes.Ldarg_0),
                                    Instruction.Create(OpCodes.Call, tackingManagerProviderMethod),
                                    Instruction.Create(OpCodes.Stfld, trackingField));
            
            type.Fields.Add(trackingField);

            //IL_0008: ldarg.0
            //IL_0009: call class [Xce.TrackingItem]Xce.TrackingItem.TrackingManager [Xce.TrackingItem]Xce.TrackingItem.TrackingManagerProvider::GetDefault()
            //IL_000e: stfld class [Xce.TrackingItem]Xce.TrackingItem.TrackingManager Xce.TrackingItem.Fody.TestModel.ReferenceModel::trackingManager

            return trackingField;
        }

        private void UpdateProperty(PropertyDefinition item, FieldDefinition field)
        {
            WriteInfo($"Procceed Property {item.Name}");

            var instructions = item.SetMethod.Body.Instructions;

            var localtracingMethod = AddTracePropertyMethod(item);

            var addActionMethod = ModuleDefinition.ImportReference(field.FieldType.Resolve().FindMethod(nameof(TrackingManager.AddAction), typeof(ITrackingAction).FullName));

            var genericActionTypeDef = referenceProvider.GetTypeReference(typeof(Action<,>)).MakeGenericInstanceType(item.DeclaringType, item.PropertyType);
            var actionContructor = referenceProvider.GetMethodReference(genericActionTypeDef.Resolve().FindMethod(".ctor", typeof(Object).FullName, typeof(IntPtr).FullName)).MakeHostInstanceGeneric(item.DeclaringType, item.PropertyType);

            var trackingPropertyTypeDef = referenceProvider.GetTypeReference(typeof(TrackingPropertyUpdate<,>)).MakeGenericInstanceType(item.DeclaringType, item.PropertyType);
            var trackingActionContructor = referenceProvider.GetMethodReference(trackingPropertyTypeDef.Resolve().FindMethod(".ctor", 3, typeof(Action<,>).FullName)).MakeHostInstanceGeneric(item.DeclaringType, item.PropertyType);

            var logInstanceMethod = new GenericInstanceMethod(addActionMethod);

            logInstanceMethod.GenericArguments.Add(item.DeclaringType);
            logInstanceMethod.GenericArguments.Add(item.PropertyType);

            instructions.InsertFirst(Instruction.Create(OpCodes.Nop),
                                     Instruction.Create(OpCodes.Ldarg_0),
                                     Instruction.Create(OpCodes.Ldfld, field),
                                     Instruction.Create(OpCodes.Ldarg_0),
                                     Instruction.Create(OpCodes.Call, item.GetMethod),
                                     Instruction.Create(OpCodes.Ldarg_1),
                                     Instruction.Create(OpCodes.Ldarg_0),
                                     Instruction.Create(OpCodes.Ldnull),
                                     Instruction.Create(OpCodes.Ldftn, localtracingMethod),
                                     Instruction.Create(OpCodes.Newobj, actionContructor),
                                     Instruction.Create(OpCodes.Newobj, trackingActionContructor),
                                     Instruction.Create(OpCodes.Callvirt, addActionMethod));

            //IL_0000: nop
            //IL_0001: ldarg.0
            //IL_0002: ldfld class [Xce.TrackingItem]Xce.TrackingItem.TrackingManager Xce.TrackingItem.Fody.TestModel.ReferenceModel::trackingManager
            //IL_0007: ldarg.0
            //IL_0008: call instance int32 Xce.TrackingItem.Fody.TestModel.ReferenceModel::get_Value()
            //IL_000d: ldarg.1
            //IL_000e: ldarg.0
            //IL_000f: ldnull
            //IL_0010: ldftn void Xce.TrackingItem.Fody.TestModel.ReferenceModel::TrackingValue(class Xce.TrackingItem.Fody.TestModel.ReferenceModel, int32)
            //IL_0016: newobj instance void class [netstandard]System.Action`2<class Xce.TrackingItem.Fody.TestModel.ReferenceModel, int32>::.ctor(object, native int)
            //IL_001b: newobj instance void class [Xce.TrackingItem]Xce.TrackingItem.TrackingAction.TrackingPropertyUpdate`2<class Xce.TrackingItem.Fody.TestModel.ReferenceModel, int32>::.ctor(!1, !1, !0, class [netstandard]System.Action`2<!0, !1>)
            //IL_0020: callvirt instance void [Xce.TrackingItem]Xce.TrackingItem.TrackingManager::AddAction(class [Xce.TrackingItem]Xce.TrackingItem.TrackingAction.ITrackingAction)
        }


        private void UpdatePropertyWithLog(PropertyDefinition item, FieldDefinition field)
        {
            WriteInfo($"Procceed Property {item.Name}");

            var instructions = item.SetMethod.Body.Instructions;

            var localTracingMethod = AddTracePropertyMethod(item);

            var addActionMethod = ModuleDefinition.ImportReference(field.FieldType.Resolve().FindMethod(nameof(TrackingManager.AddAction), typeof(Func<>).FullName));

            var genericActionTypeDef = referenceProvider.GetTypeReference(typeof(Action<,>)).MakeGenericInstanceType(item.DeclaringType, item.PropertyType);
            var actionContructor = referenceProvider.GetMethodReference(genericActionTypeDef.Resolve().FindMethod(".ctor", typeof(Object).FullName, typeof(IntPtr).FullName)).MakeHostInstanceGeneric(item.DeclaringType, item.PropertyType);

            var logInstanceMethod = new GenericInstanceMethod(addActionMethod);

            logInstanceMethod.GenericArguments.Add(item.DeclaringType);
            logInstanceMethod.GenericArguments.Add(item.PropertyType);

            var TrickingActionfactoryTypeDef = referenceProvider.GetTypeReference(typeof(TrackingActionFactory));
            var trackingActionFactoryMethodNoGen = referenceProvider.GetMethodReference(TrickingActionfactoryTypeDef.Resolve().FindMethod(nameof(TrackingActionFactory.GetTrackingPropertyUpdateFunc), 3, typeof(Action<,>).FullName));
            var trackingActionFactoryMethod = new GenericInstanceMethod(trackingActionFactoryMethodNoGen);

            trackingActionFactoryMethod.GenericArguments.Add(item.DeclaringType);
            trackingActionFactoryMethod.GenericArguments.Add(item.PropertyType);

            var getterMethod = item.GetMethod.GetGeneric();

            var firstInstruction = item.SetMethod.Body.Instructions.FirstOrDefault();

            if (firstInstruction == null)
            {
                firstInstruction = Instruction.Create(OpCodes.Ret);
                item.SetMethod.Body.Instructions.Add(firstInstruction);
            }

            equalsMethodAppender.InjectEqualityCheck(0, instructions, Instruction.Create(OpCodes.Call, getterMethod), item.PropertyType, firstInstruction);

            var index = instructions.IndexOf(firstInstruction);

            instructions.InsertList(index, 
                                    Instruction.Create(OpCodes.Nop),
                                    Instruction.Create(OpCodes.Ldarg_0),
                                    Instruction.Create(OpCodes.Ldfld, field),
                                    Instruction.Create(OpCodes.Ldarg_0),
                                    Instruction.Create(OpCodes.Ldarg_0),
                                    Instruction.Create(OpCodes.Call, getterMethod),
                                    Instruction.Create(OpCodes.Ldarg_1),
                                    Instruction.Create(OpCodes.Ldnull),
                                    Instruction.Create(OpCodes.Ldftn, localTracingMethod),
                                    Instruction.Create(OpCodes.Newobj, actionContructor),
                                    Instruction.Create(OpCodes.Call, trackingActionFactoryMethod),
                                    Instruction.Create(OpCodes.Callvirt, addActionMethod));
            
            // /!\ Code without Equality check (too specific)
            //IL_0000: nop
		    //IL_0001: ldarg.0
		    //IL_0002: ldfld class [Xce.TrackingItem]Xce.TrackingItem.TrackingManager Xce.TrackingItem.Fody.TestModel.ReferenceModel::trackingManager
		    //IL_0007: ldarg.0
		    //IL_0008: ldarg.0
		    //IL_0009: call instance int32 Xce.TrackingItem.Fody.TestModel.ReferenceModel::get_Value()
		    //IL_000e: ldarg.1
		    //IL_000f: ldnull
		    //IL_0010: ldftn void Xce.TrackingItem.Fody.TestModel.ReferenceModel::TrackingValue(class Xce.TrackingItem.Fody.TestModel.ReferenceModel, int32)
		    //IL_0016: newobj instance void class [netstandard]System.Action`2<class Xce.TrackingItem.Fody.TestModel.ReferenceModel, int32>::.ctor(object, native int)
		    //IL_001b: call class [netstandard]System.Func`1<class [Xce.TrackingItem]Xce.TrackingItem.TrackingAction.ITrackingAction> [Xce.TrackingItem]Xce.TrackingItem.TrackingActionFactory::GetTrackingPropertyUpdateFunc<class Xce.TrackingItem.Fody.TestModel.ReferenceModel, int32>(!!0, !!1, !!1, class [netstandard]System.Action`2<!!0, !!1>)
		    //IL_0020: callvirt instance void [Xce.TrackingItem]Xce.TrackingItem.TrackingManager::AddAction(class [netstandard]System.Func`1<class [Xce.TrackingItem]Xce.TrackingItem.TrackingAction.ITrackingAction>)
        }

        private MethodDefinition AddTracePropertyMethod(PropertyDefinition item)
        {
            var method = new MethodDefinition($"TrackingItemSetter_{item.Name}", MethodAttributes.Private | MethodAttributes.Static, TypeSystem.VoidReference);

            var itemParameter = new ParameterDefinition(item.DeclaringType);
            var valueParameter = new ParameterDefinition(item.PropertyType);

            method.Parameters.Add(itemParameter);
            method.Parameters.Add(valueParameter);

            var instructions = method.Body.Instructions;

            var setterMethod = ModuleDefinition.ImportReference(item.SetMethod);

            instructions.InsertFirst(Instruction.Create(OpCodes.Ldarg_0),
                                     Instruction.Create(OpCodes.Ldarg_1),
                                     Instruction.Create(OpCodes.Callvirt, setterMethod),
                                     Instruction.Create(OpCodes.Ret));

            item.DeclaringType.Methods.Add(method);

            return method;

            //IL_0000: ldarg.0
            //IL_0001: ldarg.1
            //IL_0002: callvirt instance void Xce.TrackingItem.Fody.TestModel.BaseModel::set_Value(int32)
            //IL_0007: ret
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "mscorlib";
            yield return "System";
            yield return "System.Runtime";
            yield return "System.Core";
            yield return "netstandard";
            yield return "System.Collections";
            yield return "System.ObjectModel";
            yield return "System.Threading";
            yield return "Xce.TrackingItem";
        }
    }
}
