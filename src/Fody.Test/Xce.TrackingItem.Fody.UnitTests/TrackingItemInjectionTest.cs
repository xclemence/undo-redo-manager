using System;
using System.Linq;
using Fody;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xce.TrackingItem.Fody.UnitTests
{
    [TestClass]
    public class TrackingItemInjectionTest
    {
        [TestMethod]
        public void TestWeaver()
        {
            var weavingTask = new ModuleWeaver();
            var testResult = weavingTask.ExecuteTestRun("Xce.TrackingItem.Fody.TestModel.dll", false);

            Assert.AreEqual(0, testResult.Errors.Count, testResult.Errors.Select(x => x.Text).DefaultIfEmpty().Aggregate((x, y) => $"{ x }{ Environment.NewLine }{ y }"));
            Assert.AreEqual(0, testResult.Warnings.Count, testResult.Warnings.Select(x => x.Text).DefaultIfEmpty().Aggregate((x, y) => $"{ x }{ Environment.NewLine }{ y }"));
        }
    }
}
