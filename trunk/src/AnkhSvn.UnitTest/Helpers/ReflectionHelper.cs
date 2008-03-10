using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnkhSvn_UnitTestProject.Helpers
{
    static class ReflectionHelper
    {
        public static TReturn InvokeMethod<TTarget, TReturn>(TTarget targetInstance, string methodName, params object[] parameters)
            where TReturn : class
        {
            MethodInfo info = typeof(TTarget).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(info);
            object returnValue = info.Invoke(targetInstance, parameters);
            return returnValue as TReturn;
        }
    }
}
