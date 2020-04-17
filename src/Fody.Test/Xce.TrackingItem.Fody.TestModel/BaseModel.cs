using Xce.TrackingItem.Attributes;

namespace Xce.TrackingItem.Fody.TestModel
{
    [Tracking]
    public class BaseModel
    {
        public int Value { get; set; }

        [NoPropertyTracking]
        public object NoTracking { get; set; }
    }
}
