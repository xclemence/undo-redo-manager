using System;
using System.Collections.Generic;
using System.Linq;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem.Extensions
{
    public static class ITrackingActionExtensions
    {
        internal static IEnumerable<ITrackingAction> Reduce(this IEnumerable<ITrackingAction> trackingActions)
        {
            foreach (var item in trackingActions)
            {
                if (item is MultiTrackingAction multiTrackingAction)
                    foreach (var subItem in multiTrackingAction.Actions)
                        yield return subItem;
                else
                    yield return item;
            }
        }

        public static string ContentToString(this IEnumerable<ITrackingAction> trackingActions) =>
            trackingActions.Select(x => x.ToString()).DefaultIfEmpty().Aggregate((x, y) => $"{x}{Environment.NewLine}{y}");

    }
}
