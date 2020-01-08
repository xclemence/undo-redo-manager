using System;
using System.Linq.Expressions;
using System.Reflection;

namespace UndoRedo.Tests
{
    public class ObjectReflexion
    {

        public string Name { get; set; }


        public PropertyInfo TestFindPropertyInfoReflexion(string callerName) => GetType().GetProperty(callerName);

        public PropertyInfo TestFindPropertyInfoExpression<T>(Expression<Func<T>> expression)
        {
            var body = (MemberExpression)expression.Body;
            return (PropertyInfo)body.Member;
        }

        public static Action<TObject, TValue> TestFindSetterAndCreateDelegate<TObject, TValue>(string callerName) =>
            (Action<TObject, TValue>)Delegate.CreateDelegate(typeof(Action<TObject, TValue>), null, typeof(TObject).GetProperty(callerName).GetSetMethod());

        public static Action<TObject, TValue> TestFindSetterLazyDelegate<TObject, TValue>(string callerName)
        {
            return (x, y) => 
            {
                var action = (Action<TObject, TValue>)Delegate.CreateDelegate(typeof(Action<TObject, TValue>), null, typeof(TObject).GetProperty(callerName).GetSetMethod());
                action(x, y);
            };
        }

        public Action<ObjectReflexion, T> TestFindSetterAndCreateDelegateFromInfo<T>(string callerName)
        {
            var methodInfo = TestFindPropertyInfoReflexion(callerName).GetSetMethod();
            return (Action<ObjectReflexion, T>)methodInfo.CreateDelegate(typeof(Action<ObjectReflexion, T>));
        }

        public MethodInfo TestFindSetterAndCreateMethodInfo(string callerName) => TestFindPropertyInfoReflexion(callerName).GetSetMethod();


        public Action<ObjectReflexion, string> TestFindSetterCreateActionInvoke(string callerName)
        {
            var methodInfo = TestFindPropertyInfoReflexion(callerName).GetSetMethod();
            return (x, y) => methodInfo.Invoke(x, new[] { y });
        }

        public Action<ObjectReflexion, T> TestFindSetterFullAction<T>(string callerName)
        {
            return (x, t) => x.GetType().GetProperty(callerName).GetSetMethod().CreateDelegate(typeof(Action<ObjectReflexion, T>));
        }

        public void TestSet(PropertyInfo info, string value) => info.SetValue(this, value);
    }
}
