using System;
using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Xce.TrackingItem.Attributes;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        private readonly ReferenceProvider referenceProvider;

        public ModuleWeaver()
        {
            referenceProvider = new ReferenceProvider(this);
        }

        private IEnumerable<(TypeDefinition type, List<PropertyDefinition> properties)> GetProperties()
        {
            var types = ModuleDefinition.Types.Where(x => x.IsClass && x.HasCustomAttributes && x.CustomAttributes.Any(a => a.AttributeType.Name == nameof(TrackingAttribute)));

            return types.SelectMany(x => x.Properties, (x, y) => (type: x, property: y))
                             .Where(x => x.property.SetMethod != null && !x.property.CustomAttributes.Any(a => a.AttributeType.Name == nameof(NoTrackingAttribute)))
                             .GroupBy(x => x.type, x => x.property)
                             .Select(x => (type: x.Key, properties: x.ToList()));
        }


        public override void Execute()
        {
            WriteInfo($"Start Fody Xce Tracking");

            var results = GetProperties();

            foreach (var (type, properties) in results) 
            {
                WriteInfo($"Procceed Type {type.FullName}");
                var tracerField = InjectField(type);

                foreach (var property in properties)
                    UpdateProperty(property, tracerField);
            }

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

        private MethodDefinition AddTracePropertyMethod(PropertyDefinition item)
        {
            var method = new MethodDefinition($"TrackingItemSetter_{item.Name}", MethodAttributes.Private | MethodAttributes.Static, TypeSystem.VoidReference);

            var itemParameter = new ParameterDefinition(item.DeclaringType);
            var valueParameter = new ParameterDefinition(item.PropertyType);

            method.Parameters.Add(itemParameter);
            method.Parameters.Add(valueParameter);

            var setterMethod = ModuleDefinition.ImportReference(item.SetMethod);
            var instructions = method.Body.Instructions;

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
