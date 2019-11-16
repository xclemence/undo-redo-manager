using System.Collections.Generic;
using System.Linq;
using Xce.TrackingItem.Interfaces;

namespace Xce.TrackingItem.TestModel.Base
{
    public static class IListExtensions
    {
        public static IList<T> DeepCopy<T>(this IList<T> source) where T : ICopiable<T> => source.Select(x => x.DeepCopy()).ToList();

        public static void Set<T>(this IList<T> target, IList<T> source) where T : ISettable<T>, IIdentifiable
        {
            IList<T> newItems = new List<T>();

            var i = 0;
            for (; i < source.Count; ++i)
            {
                if (i < target.Count && source[i].Id == target[i].Id)
                    target[i].Set(source[i]);
            }

        }
    }
}
