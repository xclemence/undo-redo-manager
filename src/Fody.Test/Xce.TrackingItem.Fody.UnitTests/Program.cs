﻿using Fody;

namespace Xce.TrackingItem.Fody.UnitTests
{
    internal class Program
    {
        private static void Main()
        {
            var weavingTask = new ModuleWeaver();
            weavingTask.ExecuteTestRun("Xce.TrackingItem.Fody.TestModel.dll", false);
        }
    }
}
