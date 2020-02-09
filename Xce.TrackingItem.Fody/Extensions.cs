using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace Xce.TrackingItem.Fody
{
    public static class Extensions
    {
        public static MethodDefinition FindMethod(this TypeDefinition typeDefinition, string method, int expectedGenericNumber, params string[] paramTypes)
        {
            var firstOrDefault = typeDefinition.Methods.FirstOrDefault(x => x.Name == method && x.IsMatch(expectedGenericNumber, paramTypes));

            if (firstOrDefault == null)
            {
                var parameterNames = string.Join(", ", paramTypes);
                throw new WeavingException($"Expected to find method '{method}({parameterNames})' on type '{typeDefinition.FullName}'.");
            }
            return firstOrDefault;
        }

        public static MethodDefinition FindMethod(this TypeDefinition typeDefinition, string method, params string[] paramTypes) =>
            typeDefinition.FindMethod(method, 0, paramTypes);

        public static bool IsMatch(this MethodReference methodReference, int expectedGenericNumber, params string[] paramTypes)
        {
            var genericParameterNumber = methodReference.Parameters.Count(x => x.ParameterType.IsGenericParameter);
            
            if (expectedGenericNumber != genericParameterNumber)
                return false;

            var parameters = methodReference.Parameters.Where(x => !x.ParameterType.IsGenericParameter).ToList();

            if (parameters.Count != paramTypes.Length)
                return false;

            for (var index = 0; index < parameters.Count; index++)
            {
                var parameterDefinition = parameters[index];
                var paramType = paramTypes[index];
                if (parameterDefinition.ParameterType.GetElementType().FullName != paramType)
                    return false;
            }

            return true;
        }

        public static MethodDefinition GetStaticConstructor(this TypeDefinition type, TypeReference returnType)
        {
            var staticConstructor = type.Methods.FirstOrDefault(x => x.IsConstructor && x.IsStatic);
            
            if (staticConstructor == null)
            {
                const MethodAttributes attributes = MethodAttributes.Static
                                                    | MethodAttributes.SpecialName
                                                    | MethodAttributes.RTSpecialName
                                                    | MethodAttributes.HideBySig
                                                    | MethodAttributes.Private;
                staticConstructor = new MethodDefinition(".cctor", attributes, returnType);

                staticConstructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                type.Methods.Add(staticConstructor);
            }
            staticConstructor.Body.InitLocals = true;
            
            return staticConstructor;
        }

        //public static TypeDefinition GetNonCompilerGeneratedType(this TypeDefinition typeDefinition)
        //{
        //    while (typeDefinition.IsCompilerGenerated() && typeDefinition.DeclaringType != null)
        //    {
        //        typeDefinition = typeDefinition.DeclaringType;
        //    }
        //    return typeDefinition;
        //}

        public static bool IsCompilerGenerated(this TypeDefinition type)
        {
            return type.CustomAttributes.Any(a => a.AttributeType.Name == "CompilerGeneratedAttribute") ||
                type.IsNested && type.DeclaringType.IsCompilerGenerated();
        }

        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, params TypeReference[] arguments)
        {
            var reference = new MethodReference(self.Name, self.ReturnType, self.DeclaringType.MakeGenericInstanceType(arguments))
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };

            foreach (var parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            foreach (var generic_parameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));

            return reference;
        }

        //public static MethodReference MakeGeneric(this MethodReference self, TypeReference declaringType)
        //{
        //    var reference = new MethodReference(self.Name, self.ReturnType)
        //    {
        //        DeclaringType = declaringType,
        //        HasThis = self.HasThis,
        //        ExplicitThis = self.ExplicitThis,
        //        CallingConvention = self.CallingConvention,
        //    };

        //    foreach (var parameter in self.Parameters)
        //    {
        //        reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
        //    }

        //    return reference;
        //}
    }
}
