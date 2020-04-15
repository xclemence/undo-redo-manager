using System;
using System.Collections.Generic;
using System.Text;

namespace Xce.TrackingItem.Interfaces
{
    public interface ISettable<in T> where T : ISettable<T>
    {
        void SetItem(T item);
    }
}
