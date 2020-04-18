using System;
using System.Collections.Generic;
using Fody;
using Mono.Cecil;

namespace Xce.TrackingItem.Fody
{
    internal class ReferenceProvider
    {
        private readonly Dictionary<string, MethodReference> methodCache = new Dictionary<string, MethodReference>();
        private readonly Dictionary<string, TypeReference> typeCache = new Dictionary<string, TypeReference>();

        private readonly BaseModuleWeaver moduleWeaver;

        public ReferenceProvider(BaseModuleWeaver moduleWeaver) => this.moduleWeaver = moduleWeaver;

        public TypeReference GetTypeReference(string typeName)
        {
            var typeDefinition = moduleWeaver.FindTypeDefinition(typeName);

            if (typeCache.ContainsKey(typeDefinition.FullName))
                return typeCache[typeDefinition.FullName];

            return typeCache[typeDefinition.FullName] = moduleWeaver.ModuleDefinition.ImportReference(typeDefinition);
        }

        public TypeReference GetTypeReference(Type type)
        {
            if (typeCache.ContainsKey(type.FullName))
                return typeCache[type.FullName];

            return typeCache[type.FullName] = moduleWeaver.ModuleDefinition.ImportReference(type);
        }


        public MethodReference GetMethodReference(MethodReference method)
        {
            if (methodCache.ContainsKey(method.FullName))
                return methodCache[method.FullName];

            return methodCache[method.FullName] = moduleWeaver.ModuleDefinition.ImportReference(method);
        }

        public MethodReference GetMethodReference(GenericInstanceMethod method, IGenericParameterProvider generic)
        {
            // test if fullName of generic method contrains generics parameters !!.
            if (methodCache.ContainsKey(method.FullName))
                return methodCache[method.FullName];

            return methodCache[method.FullName] = moduleWeaver.ModuleDefinition.ImportReference(method, generic);
        }
    }
}
