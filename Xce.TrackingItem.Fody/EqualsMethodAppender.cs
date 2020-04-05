using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Xce.TrackingItem.Fody
{
    public static class TypeReferenceExtensions
    {
        private static IList<string> CeqStructNames { get; } = new List<string>
                             {
                                 typeof (int).Name,
                                 typeof (uint).Name,
                                 typeof (long).Name,
                                 typeof (ulong).Name,
                                 typeof (float).Name,
                                 typeof (double).Name,
                                 typeof (bool).Name,
                                 typeof (short).Name,
                                 typeof (ushort).Name,
                                 typeof (byte).Name,
                                 typeof (sbyte).Name,
                                 typeof (char).Name,
            };

        public static bool SupportsCeq(this TypeReference typeReference)
        {
            if (CeqStructNames.Contains(typeReference.Name))
            {
                return true;
            }
            if (typeReference.IsArray)
            {
                return false;
            }
            if (typeReference.ContainsGenericParameter)
            {
                return false;
            }
            var typeDefinition = typeReference.Resolve();
            if (typeDefinition == null)
            {
                throw new Exception($"Could not resolve '{typeReference.FullName}'.");
            }
            if (typeDefinition.IsEnum)
            {
                return true;
            }

            return !typeDefinition.IsValueType;
        }
    }

    internal class EqualsMethodAppender
    {
        private readonly EqualsMethodProvider methodProvider;

        public EqualsMethodAppender(EqualsMethodProvider methodProvider)
        {
            this.methodProvider = methodProvider;
        }

        public void InjectEqualityCheck(int index, IList<Instruction> instructions, Instruction valueInstruction, TypeReference targetType, Instruction trueInstruction)
        {
            var equalsMethod = methodProvider.FindTypeEquality(targetType);

            if (targetType.FullName == typeof(string).FullName)
            {
                instructions.InsertList(index,
                    Instruction.Create(OpCodes.Ldarg_0),
                    valueInstruction,
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Ldc_I4, methodProvider.GetOrdinalStringComparison()),
                    Instruction.Create(OpCodes.Call, equalsMethod),
                    Instruction.Create(OpCodes.Brtrue_S, trueInstruction));
                return;
            }

            if (equalsMethod != null)
            {
                instructions.InsertList(index,
                    Instruction.Create(OpCodes.Ldarg_0),
                    valueInstruction,
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Call, equalsMethod),
                    Instruction.Create(OpCodes.Brtrue_S, trueInstruction));
                return;
            }

            var supportsCeq = targetType.SupportsCeq();

            if (supportsCeq)
            {
                instructions.InsertList(index,
                    Instruction.Create(OpCodes.Ldarg_0),
                    valueInstruction,
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Ceq),
                    Instruction.Create(OpCodes.Brtrue_S, trueInstruction));
                return;
            }
            var objectEqualsMethod = methodProvider.GetObjectEqualsMethod();

            if (targetType.IsValueType || targetType.IsGenericParameter)
            {
                instructions.InsertList(index,
                    Instruction.Create(OpCodes.Ldarg_0),
                    valueInstruction,
                    Instruction.Create(OpCodes.Box, targetType),
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Box, targetType),
                    Instruction.Create(OpCodes.Call, objectEqualsMethod),
                    Instruction.Create(OpCodes.Brtrue_S, trueInstruction));
                return;
            }

            instructions.InsertList(index,
                Instruction.Create(OpCodes.Ldarg_0),
                valueInstruction,
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Call, objectEqualsMethod),
                Instruction.Create(OpCodes.Brtrue_S, trueInstruction));
            return;
        }
    }
}
