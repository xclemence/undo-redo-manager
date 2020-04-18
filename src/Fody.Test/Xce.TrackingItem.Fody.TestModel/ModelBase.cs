using Xce.TrackingItem.Attributes;

namespace Xce.TrackingItem.Fody.TestModel
{
    [Tracking]
    public class ModelBase
    {
        public int Value { get; set; }

        [NoPropertyTracking]
        public object NoTracking { get; set; }
    }
}
