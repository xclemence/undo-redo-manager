using System.Linq;
using Fody;
using Mono.Cecil;

namespace Xce.TrackingItem.Fody.Extensions
{
    public static class FindMethodExtensions
    {
        public static MethodDefinition FindMethod(this TypeDefinition typeDefinition, string method, params string[] paramTypes) =>
        typeDefinition.FindMethod(method, 0, paramTypes);

        public static MethodDefinition FindMethod(this TypeDefinition typeDefinition, string method, int expectedGenericNumber, params string[] paramTypes)
        {
            var firstOrDefault = typeDefinition?.Methods?.FirstOrDefault(x => x.Name == method && x.IsMatch(expectedGenericNumber, paramTypes));

            if (firstOrDefault == null)
            {
                var parameterNames = string.Join(", ", paramTypes);
                throw new WeavingException($"Expected to find method '{method}({parameterNames})' on type '{typeDefinition?.FullName}'.");
            }
            return firstOrDefault;
        }

        private static bool IsMatch(this MethodReference methodReference, int expectedGenericNumber, params string[] paramTypes)
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
    }
}
