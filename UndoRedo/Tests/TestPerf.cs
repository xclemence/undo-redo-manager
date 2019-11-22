using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UndoRedo.Tests
{
    public class TestPerf
    {
        public uint TryNumber { get; set; } = 1000000;

        public IEnumerable<(string test, TimeSpan duration)> RunTest()
        {
            var testObject = new ObjectReflexion();

            yield return ("PropertyInfo With Reflexion", ExecuteTest(() => testObject.TestFindPropertyInfoReflexion(nameof(ObjectReflexion.Name))));
            yield return ("PropertyInfo With Expression", ExecuteTest(() => testObject.TestFindPropertyInfoExpression(() => testObject.Name)));
            yield return ("MathodInfo with Reflexion", ExecuteTest(() => testObject.TestFindSetterAndCreateMethodInfo(nameof(ObjectReflexion.Name))));
            
            yield return ("Delegate From Delegate", ExecuteTest(() => ObjectReflexion.TestFindSetterAndCreateDelegate<ObjectReflexion, string>(nameof(ObjectReflexion.Name))));
            //yield return ("Delegate From MethodInfo", ExecuteTest(() => testObject.TestFindSetterAndCreateDelegateFromInfo<string>(nameof(ObjectReflexion.Name))));
            
            yield return ("Action From MethodInfo (capture)", ExecuteTest(() => testObject.TestFindSetterCreateActionInvoke(nameof(ObjectReflexion.Name))));
            yield return ("Action From MethodInfo (no capture)", ExecuteTest(() => testObject.TestFindSetterFullAction<string>(nameof(ObjectReflexion.Name))));

            yield return ("-------------------------------", TimeSpan.Zero);
            
            yield return ("Set Direct", ExecuteTest(() => testObject.Name = "truc"));
            
            var propertyInfo = testObject.TestFindPropertyInfoReflexion(nameof(ObjectReflexion.Name));
            yield return ("Set PropertyInfo", ExecuteTest(() => propertyInfo.SetValue(testObject, "truc")));

            var methodInfo = testObject.TestFindSetterAndCreateMethodInfo(nameof(ObjectReflexion.Name));
            yield return ("Set MethodIndo", ExecuteTest(() => methodInfo.Invoke(testObject, new[] { "tsffdsruc" })));

            var setterDelegate = ObjectReflexion.TestFindSetterAndCreateDelegate<ObjectReflexion, string>(nameof(ObjectReflexion.Name));
            var setterDelegateFromMethod = testObject.TestFindSetterAndCreateDelegateFromInfo<string>(nameof(ObjectReflexion.Name));
            
            var setterActionContext = testObject.TestFindSetterCreateActionInvoke(nameof(ObjectReflexion.Name));
            var setterActionNoContext = testObject.TestFindSetterFullAction<string>(nameof(ObjectReflexion.Name));

            yield return ("Set Delegate", ExecuteTest(() => setterDelegate(testObject, "machin")));
            //yield return ("Set Delegate from method", ExecuteTest(() => setterDelegateFromMethod(testObject, "machin1à")));

            yield return ("Set Action Context", ExecuteTest(() => setterActionContext(testObject, "machin1à")));
            yield return ("Set Action No context", ExecuteTest(() => setterActionNoContext(testObject, "hello")));

        }

        private TimeSpan ExecuteTest(Action action)
        {
            var watch = new Stopwatch();

            watch.Start();
            for (var i = 0; i < TryNumber; ++i)
            {
                action();
            }

            watch.Stop();

            return watch.Elapsed;
        }
    }
}
