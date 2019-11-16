using System;
using System.Collections.Generic;
using System.Text;

namespace Xce.TrackingItem.Interfaces
{
    public interface ICopiable<out T> where T : ICopiable<T>
    {
        T DeepCopy();
    }
}
