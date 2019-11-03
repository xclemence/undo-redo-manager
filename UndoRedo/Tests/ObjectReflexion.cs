using System;
using System.Linq.Expressions;
using System.Reflection;

namespace UndoRedo.Tests
{
    public class ObjectReflexion
    {
        
        public string Name  { get; set; }


        public PropertyInfo TestFindPropertyInfoReflexion(string callerName) => GetType().GetProperty(callerName);

        public PropertyInfo TestFindPropertyInfoExpression<T>(Expression<Func<T>> expression)
        {
            var body = (MemberExpression) expression.Body;
            return (PropertyInfo) body.Member;
        }

        public static  Action<TObject, TValue> TestFindSetter<TObject, TValue>(string callerName) => 
            (Action<TObject, TValue>)Delegate.CreateDelegate(typeof(Action<TObject, TValue>), null, typeof(TObject).GetProperty(callerName).GetSetMethod());

        public MethodInfo TestFindSetter2(string callerName) => TestFindPropertyInfoReflexion(callerName).GetSetMethod();
        public Action<ObjectReflexion, dynamic> TestFindSetter3(string callerName)
        {
            var methodInfo = TestFindPropertyInfoReflexion(callerName).GetSetMethod();
            return (x, y) => methodInfo.Invoke(x, new[] { y });
        }

        public Action<ObjectReflexion, T> TestFindSetter4<T>(string callerName)
        {
            var methodInfo = TestFindPropertyInfoReflexion(callerName).GetSetMethod();
            return (Action<ObjectReflexion, T>)methodInfo.CreateDelegate(typeof(Action<ObjectReflexion, T>));
        }

        public Action<ObjectReflexion, T> TestFindSetter5<T>(string callerName)
        {
            return (x, t) => x.GetType().GetProperty(callerName).GetSetMethod().CreateDelegate(typeof(Action<ObjectReflexion, T>));
        }

        public void TestSet(PropertyInfo info, string value) => info.SetValue(this, value);
    }
}
