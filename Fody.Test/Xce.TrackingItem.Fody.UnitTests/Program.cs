﻿using Fody;

namespace Xce.TrackingItem.Fody.UnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var weavingTask = new ModuleWeaver();
            weavingTask.ExecuteTestRun("Xce.TrackingItem.Fody.TestModel.dll", false);
        }
    }
}
