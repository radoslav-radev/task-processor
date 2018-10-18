using System;
using System.Collections.Generic;
using System.Reflection;

namespace Radoslav
{
    public partial class MockObject
    {
        private abstract class PredefinedMethodCallBase
        {
            private readonly MethodInfo method;
            private readonly IReadOnlyList<object> arguments;

            internal PredefinedMethodCallBase(MethodInfo method, IReadOnlyList<object> arguments)
            {
                this.method = method;
                this.arguments = arguments;
            }

            internal MethodInfo Method
            {
                get { return this.method; }
            }

            internal IReadOnlyList<object> Arguments
            {
                get { return this.arguments; }
            }
        }

        private sealed class PredefinedMethodCallResult<TResult> : PredefinedMethodCallBase
        {
            private readonly TResult result;

            internal PredefinedMethodCallResult(MethodInfo method, IReadOnlyList<object> arguments, TResult result)
                : base(method, arguments)
            {
                this.result = result;
            }

            internal TResult Result
            {
                get { return this.result; }
            }
        }

        private sealed class PredefinedMethodCallAction : PredefinedMethodCallBase
        {
            private readonly Action callback;

            internal PredefinedMethodCallAction(MethodInfo method, IReadOnlyList<object> arguments, Action callback)
                : base(method, arguments)
            {
                this.callback = callback;
            }

            internal Action Callback
            {
                get { return this.callback; }
            }
        }
    }
}