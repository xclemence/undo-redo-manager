using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Xce.TrackingItem.Fody.Extensions
{
    public static class MakeGenericExtensions
    {
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

        public static MethodReference MakeGeneric(this MethodReference reference)
        {
            if (reference.DeclaringType.HasGenericParameters)
            {
                var declaringType = new GenericInstanceType(reference.DeclaringType);
                
                foreach (var parameter in reference.DeclaringType.GenericParameters)
                    declaringType.GenericArguments.Add(parameter);

                var methodReference = new MethodReference(reference.Name, reference.MethodReturnType.ReturnType, declaringType);
                
                foreach (var parameterDefinition in reference.Parameters)
                    methodReference.Parameters.Add(parameterDefinition);
               
                methodReference.HasThis = reference.HasThis;
                return methodReference;
            }

            return reference;
        }
    }
}
