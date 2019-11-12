using System;
using System.Collections.Generic;
using System.Text;

namespace Xce.TrackingItem
{
    public class TrackingLog
    {
        public string Operation { get; set; }
        public string Type { get; set; }

        public TimeSpan Time { get; set; }
    }
}
