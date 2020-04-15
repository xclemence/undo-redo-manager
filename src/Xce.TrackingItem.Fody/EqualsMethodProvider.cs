using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Xce.TrackingItem.Fody.Extensions;

namespace Xce.TrackingItem.Fody
{
    internal class EqualsMethodProvider
    {
        private readonly IDictionary<string, MethodReference> methodCache = new Dictionary<string, MethodReference>();

        private readonly ReferenceProvider referenceProvider;
        private readonly Action<string> warningLogger;

        public EqualsMethodProvider(ReferenceProvider referenceProvider, Action<string> warningLogger)
        {
            this.referenceProvider = referenceProvider;
            this.warningLogger = warningLogger;
        }

        public MethodReference GetObjectEqualsMethod()
        {
            var objectDefinition = referenceProvider.GetTypeReference(typeof(object).FullName).Resolve();
            var objectEqualsMethodDefinition = objectDefinition.Methods.First(x => x.Name == "Equals" && x.Parameters.Count == 2);
            return referenceProvider.GetMethodReference(objectEqualsMethodDefinition);
        }

        private MethodReference FindStringComparisonMethods()
        {
            var type = referenceProvider.GetTypeReference(typeof(string).FullName).Resolve();

            var methodDefintion = type.Methods.First(x => x.IsStatic &&
                                                          x.Name == "Equals" &&
                                                          x.Parameters.Count == 3 &&
                                                          x.Parameters[0].ParameterType.Name == "String" &&
                                                          x.Parameters[1].ParameterType.Name == "String" &&
                                                          x.Parameters[2].ParameterType.Name == "StringComparison");

            return referenceProvider.GetMethodReference(methodDefintion);
        }

        private MethodReference FindNullableComparisonMethods()
        {
            var nullableDefinition = referenceProvider.GetTypeReference(typeof(Nullable).FullName).Resolve();

            var method = nullableDefinition.Methods.First(x => x.Name == "Equals");

            return referenceProvider.GetMethodReference(method);
        }

        public int GetOrdinalStringComparison()
        {
            return (int)FindStringComparisonMethods()
                 .Parameters[2]
                .ParameterType
                .Resolve()
                .Fields
                .First(x => x.Name == "Ordinal")
                .Constant;
        }

        public MethodReference FindTypeEquality(TypeReference typeReference)
        {
            var fullName = typeReference.FullName;
            if (methodCache.TryGetValue(fullName, out var methodReference))
                return methodReference;

            var equality = GetEquality(typeReference);
            methodCache[fullName] = equality;
            return equality;
        }

        private MethodReference GetEquality(TypeReference typeReference)
        {
            if (typeReference.FullName == typeof(string).FullName)
                return FindStringComparisonMethods();

            if (typeReference.FullName.StartsWith("System.Nullable", StringComparison.InvariantCulture))
                return GetNullableEquality(typeReference);

            return GetStaticEquality(typeReference);
        }

        private MethodReference GetNullableEquality(TypeReference typeReference)
        {
            var genericInstanceMethod = new GenericInstanceMethod(FindNullableComparisonMethods());

            var typeWrappedByNullable = ((GenericInstanceType)typeReference).GenericArguments.First();
            genericInstanceMethod.GenericArguments.Add(typeWrappedByNullable);

            if (typeWrappedByNullable.IsGenericParameter)
                return referenceProvider.GetMethodReference(genericInstanceMethod, typeWrappedByNullable.DeclaringType);

            return referenceProvider.GetMethodReference(genericInstanceMethod);
        }

        private MethodReference GetStaticEquality(TypeReference typeReference)
        {
            TypeDefinition typeDefinition;
            try
            {
                typeDefinition = typeReference.Resolve();

                if (typeDefinition.IsInterface)
                    return null;

            }
            catch (Exception ex)
            {
                warningLogger($"Ignoring static equality of type {typeReference.FullName} => {ex.Message}");
            }

            return FindNamedMethod(typeReference);
        }

        public MethodReference FindNamedMethod(TypeReference typeReference)
        {
            var typeDefinition = typeReference.Resolve();

            var equalsMethod = FindEqualsNamedInstanceMethod(typeDefinition, "Equals", typeReference) ??
                               FindEqualsNamedMethod(typeDefinition, "Equals", typeReference) ??
                               FindEqualsNamedMethod(typeDefinition, "op_Equality", typeReference);

            if (equalsMethod == null || !(typeReference is GenericInstanceType genericInstanceType))
                return equalsMethod;

            var genericType = new GenericInstanceType(equalsMethod.DeclaringType);

            foreach (var argument in genericInstanceType.GenericArguments)
                genericType.GenericArguments.Add(argument);

            return equalsMethod.MakeGeneric();
        }

        private MethodReference FindEqualsNamedMethod(TypeDefinition typeDefinition, string methodName, TypeReference parameterType)
        {
            var reference = typeDefinition.Methods
                .FirstOrDefault(x => x.Name == methodName &&
                                     x.IsStatic &&
                                     x.ReturnType.FullName == typeof(bool).FullName &&
                                     x.Parameters.Count == 2 &&
                                     MatchParameter(x.Parameters[0], parameterType) &&
                                     MatchParameter(x.Parameters[1], parameterType));

            if (reference != null)
                return referenceProvider.GetMethodReference(reference);

            return null;
        }

        private MethodReference FindEqualsNamedInstanceMethod(TypeDefinition typeDefinition, string methodName, TypeReference parameterType)
        {
            var reference = typeDefinition.Methods.FirstOrDefault(x => x.Name == methodName &&
                                                                       x.ReturnType.FullName == typeof(bool).FullName &&
                                                                       x.Parameters.Count == 1 &&
                                                                       MatchParameter(x.Parameters[0], parameterType));

            if (reference != null)
                return referenceProvider.GetMethodReference(reference);

            return null;
        }

        private static bool MatchParameter(ParameterDefinition parameter, TypeReference typeMatch)
        {
            if (parameter.ParameterType == typeMatch)
                return true;

            if (!parameter.ParameterType.IsGenericInstance || !typeMatch.IsGenericInstance)
                return false;

            return parameter.ParameterType.FullName == typeMatch.FullName;
        }
    }
}
