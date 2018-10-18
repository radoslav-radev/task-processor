using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Radoslav
{
    public abstract partial class MockObject
    {
        private readonly List<RecordedMethodCall> recordedMethodCalls = new List<RecordedMethodCall>();
        private readonly List<PredefinedMethodCallBase> predefinedResults = new List<PredefinedMethodCallBase>();
        private readonly CallbackEqualityComparerFactory equalityComparerFactory = new CallbackEqualityComparerFactory();

        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "No other parameters are needed.")]
        public event EventHandler<RecordedMethodCall> MethodCallRecorded;

        public ICollection<RecordedMethodCall> RecordedMethodCalls
        {
            get { return this.recordedMethodCalls; }
        }

        public CallbackEqualityComparerFactory EqualityComparerFactory
        {
            get { return this.equalityComparerFactory; }
        }

        internal void PredefineResult<TResult>(MethodInfo method, IReadOnlyList<object> arguments, TResult result)
        {
            this.predefinedResults.Add(new PredefinedMethodCallResult<TResult>(method, arguments, result));
        }

        internal void PredefineMethodCall(MethodInfo method, IReadOnlyList<object> arguments, Action callback)
        {
            this.predefinedResults.Add(new PredefinedMethodCallAction(method, arguments, callback));
        }

        protected void ExecutePredefinedMethod(params object[] arguments)
        {
            PredefinedMethodCallAction result = this.GetPredefinedMethodCall<PredefinedMethodCallAction>(arguments);

            if (result != null)
            {
                this.predefinedResults.Remove(result);

                result.Callback();
            }
        }

        protected bool HasPredefinedResult<TResult>(params object[] arguments)
        {
            PredefinedMethodCallResult<TResult> result = this.GetPredefinedMethodCall<PredefinedMethodCallResult<TResult>>(arguments);

            return result != null;
        }

        protected TResult GetPredefinedResult<TResult>(params object[] arguments)
        {
            PredefinedMethodCallResult<TResult> result = this.GetPredefinedMethodCall<PredefinedMethodCallResult<TResult>>(arguments);

            this.predefinedResults.Remove(result);

            return result.Result;
        }

        protected TResult GetPredefinedResultOrDefault<TResult>(params object[] arguments)
        {
            PredefinedMethodCallResult<TResult> result1 = this.GetPredefinedMethodCall<PredefinedMethodCallResult<TResult>>(arguments);

            if (result1 != null)
            {
                this.predefinedResults.Remove(result1);

                return result1.Result;
            }
            else
            {
                return default(TResult);
            }
        }

        protected void RecordMethodCall(params object[] arguments)
        {
            this.RecordMethodCall(
                method =>
                {
                    Assert.IsFalse(method.IsGenericMethodDefinition);

                    return method;
                },
                arguments);
        }

        protected void RecordMethodCall<T>(params object[] arguments)
        {
            this.RecordMethodCall(
                method =>
                {
                    Assert.IsTrue(method.IsGenericMethodDefinition);

                    Assert.AreEqual(1, method.GetGenericArguments().Length);

                    return ((MethodInfo)method).MakeGenericMethod(typeof(T));
                },
                arguments);
        }

        private void RecordMethodCall(Func<MethodInfo, MethodInfo> callback, params object[] arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            StackFrame frame = new StackFrame(2);

            MethodInfo method = (MethodInfo)frame.GetMethod();

            method = callback(method);

            Assert.AreEqual(method.GetParameters().Length, arguments.Length);

            RecordedMethodCall recordedMethodCall = new RecordedMethodCall(method, arguments);

            this.recordedMethodCalls.Add(recordedMethodCall);

            if (this.MethodCallRecorded != null)
            {
                this.MethodCallRecorded(this, recordedMethodCall);
            }
        }

        private TPredefinedMethodCall GetPredefinedMethodCall<TPredefinedMethodCall>(object[] arguments)
            where TPredefinedMethodCall : PredefinedMethodCallBase
        {
            StackFrame frame = new StackFrame(2);

            MethodBase method = frame.GetMethod();

            ParameterInfo[] parameters = method.GetParameters();

            Assert.AreEqual(parameters.Length, arguments.Length);

            Type[] parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

            return this.predefinedResults
                .OfType<TPredefinedMethodCall>()
                .FirstOrDefault(r => (r.Method == method) && UnitTestHelpers.CompareMethodCallArguments(parameterTypes, arguments, r.Arguments, this.equalityComparerFactory));
        }
    }
}