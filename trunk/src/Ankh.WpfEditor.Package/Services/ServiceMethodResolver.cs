using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Ankh.VS;

namespace Ankh.WpfPackage.Services
{
    [GlobalService(typeof(ILinqInterfaceDelegateService))]
    sealed class ServiceMethodResolver : AnkhService, ILinqInterfaceDelegateService
    {
        public ServiceMethodResolver(IAnkhServiceProvider context)
            : base(context)
        {
        }

        // Helper for AnkhService.GetInterfaceDelegate()
        public TDelegate GetInterfaceDelegate<TDelegate>(Type fromInterface, System.Reflection.MethodInfo method, object ob) where TDelegate : class
        {
            if (fromInterface == null)
                throw new ArgumentNullException("fromInterface");
            else if (ob == null)
                throw new ArgumentNullException("onService");
            
            Type type = typeof(TDelegate);

            ParameterExpression[] args;
            MethodCallExpression mce = GetMethodCall(fromInterface, method, ob, out args);

            if (mce != null)
                return Expression.Lambda<TDelegate>(mce, args).Compile();

            return null;
        }

        // Helper method containing most not generic code
        private MethodCallExpression GetMethodCall(Type fromInterface, MethodInfo methodInfo, object ob, out ParameterExpression[] args)
        {
            List<ParameterExpression> methodArgs = new List<ParameterExpression>();
            foreach (ParameterInfo pi in methodInfo.GetParameters())
            {
                methodArgs.Add(Expression.Parameter(pi.ParameterType, pi.Name));
            }

            args = methodArgs.ToArray();

            return Expression.Call(Expression.Convert(Expression.Constant(ob), fromInterface), methodInfo, (Expression[])args);
        }

    }
}
