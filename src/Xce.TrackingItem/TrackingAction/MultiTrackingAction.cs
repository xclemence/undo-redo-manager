using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;

namespace Xce.TrackingItem.TrackingAction
{
    public class MultiTrackingAction : ITrackingAction
    {
        private const string Indentation = "   ";

        public MultiTrackingAction(IList<ITrackingAction> actions) => Actions = actions.ToList();

        public IList<ITrackingAction> Actions { get; }
        
        public void Apply()
        {
            var revertActions = Actions.Reverse().ToList();

            foreach (var item in revertActions)
                item.Apply();
        }

        public void Revert()
        {
            foreach (var item in Actions)
                item.Revert();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Multi: {Actions.Count}");

            foreach (var item in Actions)
                builder.AppendLine($"{Indentation}{item}".Replace(Environment.NewLine, $"{Environment.NewLine}{Indentation}"));

            return builder.ToString();
        }
    }
}
