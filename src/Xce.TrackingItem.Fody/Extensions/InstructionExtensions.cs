using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Xce.TrackingItem.Fody.Extensions
{
    public static class InstructionExtensions
    {
        public static void InsertList<TObject>(this IList<TObject> baseColletion, int position, params TObject[] instructions)
        {
            var currnentPosition = position;

            foreach(var item in instructions)
            {
                baseColletion.Insert(currnentPosition, item);
                currnentPosition++;
            }
        }

        public static void InsertFirst<TObject>(this IList<TObject> baseColletion, params TObject[] instructions) => baseColletion.InsertList(0, instructions);

        public static int FindRetPosition(this IList<Instruction> instructions)
        {
            int i = 0;
            for (; i < instructions.Count; i++)
            {
                if (instructions[i].OpCode == Mono.Cecil.Cil.OpCodes.Ret)
                    return i;
            }

            return i;
        }
    }
}
