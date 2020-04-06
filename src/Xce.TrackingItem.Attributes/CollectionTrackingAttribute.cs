using System;

namespace Xce.TrackingItem.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CollectionTrackingAttribute : Attribute
    {
        public CollectionTrackingAttribute()
        {
        }
    }
}
