using System;

namespace Xce.TrackingItem.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class TrackingAttribute : Attribute
    {
        public TrackingAttribute()
        {
        }
    }
}
