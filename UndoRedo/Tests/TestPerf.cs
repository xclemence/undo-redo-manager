using System;
using System.Diagnostics;

namespace UndoRedo.Tests
{
    public class TestPerf
    {
        public void RunTest()
        {
            var testObject = new ObjectReflexion();

            var propertyByReflexion = ExecuteTest(() => testObject.TestFindPropertyInfoReflexion(nameof(ObjectReflexion.Name)));
            var propertyByExpression = ExecuteTest(() => testObject.TestFindPropertyInfoExpression(() => testObject.Name));


            var setterMethod = ExecuteTest(() => ObjectReflexion.TestFindSetter<ObjectReflexion, string>(nameof(ObjectReflexion.Name)));
            var setterMethod2 = ExecuteTest(() => testObject.TestFindSetter2(nameof(ObjectReflexion.Name)));

            var setterMethod3 = ExecuteTest(() => testObject.TestFindSetter3(nameof(ObjectReflexion.Name)));
            var setterMethod4 = ExecuteTest(() => testObject.TestFindSetter4<string>(nameof(ObjectReflexion.Name)));
            var setterMethod5 = ExecuteTest(() => testObject.TestFindSetter5<string>(nameof(ObjectReflexion.Name)));

            var property = testObject.TestFindPropertyInfoExpression(() => testObject.Name);

            var setPropertyValue = ExecuteTest(() => testObject.TestSet(property, "truc"));
            var setDirectValue = ExecuteTest(() => testObject.Name = "truc");

            var setter = ObjectReflexion.TestFindSetter<ObjectReflexion, string>(nameof(ObjectReflexion.Name));
            var setter3 = testObject.TestFindSetter3(nameof(ObjectReflexion.Name));
            var setter4 = testObject.TestFindSetter4<string>(nameof(ObjectReflexion.Name));
            var setter5 = testObject.TestFindSetter5<string>(nameof(ObjectReflexion.Name));
        
            var setterCall = ExecuteTest(() => setter(testObject, "machin"));
            var setterCall3 = ExecuteTest(() => setter3(testObject, "machin1à"));
            var setterCall4 = ExecuteTest(() => setter4(testObject, "machin1à"));
            var setterCall5 = ExecuteTest(() => setter5(testObject, "hello"));

        }

        private TimeSpan ExecuteTest(Action action)
        {
            var watch = new Stopwatch();

            watch.Start();
            for (var i = 0; i < 1000000; ++i)
            {
                action();
            }

            watch.Stop();

            return watch.Elapsed;
        }
    }
}
