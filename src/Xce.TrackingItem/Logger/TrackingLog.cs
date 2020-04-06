using System;

namespace Xce.TrackingItem.Logger
{
    public class TrackingLog
    {
        public string Operation { get; set; }
        public string Type { get; set; }

        public TimeSpan Time { get; set; }
    }
}
