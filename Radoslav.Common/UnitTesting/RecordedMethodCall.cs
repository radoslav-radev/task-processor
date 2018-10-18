using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Radoslav
{
    public sealed class RecordedMethodCall
    {
        private static readonly MethodInfo ToArrayMethod = typeof(Enumerable).GetMethod("ToArray");

        private readonly MethodBase method;
        private readonly string methodName;
        private readonly List<object> arguments = new List<object>();
        private readonly DateTime timestampUtc = DateTime.UtcNow;

        internal RecordedMethodCall(MethodBase methodInfo, params object[] arguments)
        {
            this.method = methodInfo;

            ParameterInfo[] parameters = methodInfo.GetParameters();

            for (int i = 0; i < arguments.Length; i++)
            {
                if (parameters[i].ParameterType.IsGenericType && parameters[i].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    MethodInfo toArrayMethod2 = RecordedMethodCall.ToArrayMethod.MakeGenericMethod(parameters[i].ParameterType.GetGenericArguments());

                    this.arguments.Add(toArrayMethod2.Invoke(null, new object[] { arguments[i] }));
                }
                else
                {
                    this.arguments.Add(arguments[i]);
                }
            }

            int lastDotIndex = this.method.Name.LastIndexOf('.');

            if (lastDotIndex > 0)
            {
                // For explicitly implemented interface methods.
                this.methodName = this.method.Name.Substring(lastDotIndex + 1);
            }
            else
            {
                this.methodName = this.method.Name;
            }
        }

        public MethodBase Method
        {
            get { return this.method; }
        }

        public string MethodName
        {
            get { return this.methodName; }
        }

        public IReadOnlyList<object> Arguments
        {
            get { return this.arguments; }
        }

        public DateTime TimestampUtc
        {
            get { return this.timestampUtc; }
        }

        public override string ToString()
        {
            return "{0}({1})".FormatInvariant(this.methodName, string.Join(", ", this.arguments));
        }
    }
}