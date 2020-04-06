using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xce.TrackingItem.Logger
{
    public class TrackingLoggerScope : IDisposable
    {
        private readonly Stopwatch watch;

        public TrackingLoggerScope(IList<TrackingLog> logs, string operation, string type = null)
        {
            Logs = logs ?? throw new ArgumentNullException(nameof(logs));
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            Type = type;

            watch = new Stopwatch();
            watch.Start();
        }

        public IList<TrackingLog> Logs { get; }
        public string Operation { get; }
        public string Type { get; set; }

        public void Dispose()
        {
            watch.Stop();
            Logs.Add(new TrackingLog { Operation = Operation, Type = Type, Time = watch.Elapsed });
        }
    }

}
