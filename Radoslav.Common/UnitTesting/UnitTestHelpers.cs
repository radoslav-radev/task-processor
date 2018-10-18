using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Linq.Expressions;

namespace Radoslav
{
    public static class UnitTestHelpers
    {
        public static void AssertEqualToMillisecond(DateTime value1, DateTime value2)
        {
            Assert.AreEqual(value1.Date, value2.Date);
            Assert.AreEqual(value1.TimeOfDay.TotalMilliseconds, value2.TimeOfDay.TotalMilliseconds, 1);
        }

        public static TValue GetPrivateField<TValue>(string fieldName, object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }

            FieldInfo fieldInfo = entity.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            return (TValue)fieldInfo.GetValue(entity);
        }

        public static IEnumerable<T> AsExpected<T>(this IEnumerable<T> collection, bool ordered)
        {
            return new ExpectedCollection<T>(collection)
            {
                IsOrdered = ordered
            };
        }

        public static Version Increase(this Version value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return new Version(value.Major + 1, value.Minor);
        }

        public static Version Decrease(this Version value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Major > 0)
            {
                return new Version(value.Major - 1, 0);
            }

            if (value.Minor > 0)
            {
                return new Version(value.Major, value.Minor - 1);
            }

            if (value.Build > 0)
            {
                return new Version(value.Major, value.Minor, value.Build - 1);
            }

            if (value.Revision > 0)
            {
                return new Version(value.Major, value.Minor, value.Build, value.Revision - 1);
            }

            throw new ArgumentException("Cannot get version less than 0.", "value");
        }

        public static bool AreEqualByPublicScalarProperties<T>(T item1, T item2)
        {
            if (item1 == null)
            {
                throw new ArgumentNullException("item1");
            }

            if (item2 == null)
            {
                throw new ArgumentNullException("item2");
            }

            return typeof(T).GetProperties()
               .Where(p => p.CanRead && p.PropertyType.IsPrimitive)
               .All(p => object.Equals(p.GetValue(item1), p.GetValue(item2)));
        }

        public static void AssertEqualByPublicScalarProperties<T>(T item1, T item2)
        {
            if (item1 == null)
            {
                throw new ArgumentNullException("item1");
            }

            if (item2 == null)
            {
                throw new ArgumentNullException("item2");
            }

            typeof(T).GetProperties()
               .Where(p => p.CanRead && (p.PropertyType.IsPrimitive || p.PropertyType == typeof(Version)))
               .ForEach(false, p => Assert.AreEqual(p.GetValue(item1), p.GetValue(item2)));
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = SuppressMessages.StronglyTypedExpressionNeeded)]
        public static void PredefineMethodCall<TMockObject>(this TMockObject mockObject, Expression<Action<TMockObject>> expression, Action callback)
            where TMockObject : MockObject
        {
            if (mockObject == null)
            {
                throw new ArgumentNullException("mockObject");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            visitor.Visit(expression);

            mockObject.PredefineMethodCall(visitor.MethodCallInfo, visitor.MethodCallArguments, callback);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = SuppressMessages.StronglyTypedExpressionNeeded)]
        public static void PredefineResult<TMockObject, TResult>(this TMockObject mockObject, TResult result, Expression<Func<TMockObject, TResult>> expression)
            where TMockObject : MockObject
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            visitor.Visit(expression);

            mockObject.PredefineResult(visitor.MethodCallInfo, visitor.MethodCallArguments, result);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = SuppressMessages.StronglyTypedExpressionNeeded)]
        public static void AssertMethodCallsCount<TMockObject>(this TMockObject mockObject, int expectedMethodCallsCount, Expression<Action<TMockObject>> expectedMethodCallExpression)
            where TMockObject : MockObject
        {
            if (mockObject == null)
            {
                throw new ArgumentNullException("mockObject");
            }

            if (expectedMethodCallExpression == null)
            {
                throw new ArgumentNullException("expectedMethodCallExpression");
            }

            if (expectedMethodCallsCount < 0)
            {
                throw new ArgumentOutOfRangeException("expectedMethodCallsCount");
            }

            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            visitor.Visit(expectedMethodCallExpression);

            RecordedMethodCall[] recordedMethodCalls = mockObject.RecordedMethodCalls
                .Where(c => c.Method == visitor.MethodCallInfo)
                .ToArray();

            Assert.AreEqual(expectedMethodCallsCount, recordedMethodCalls.Length);

            foreach (RecordedMethodCall record in recordedMethodCalls)
            {
                mockObject.RecordedMethodCalls.Remove(record);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = SuppressMessages.StronglyTypedExpressionNeeded)]
        public static void AssertNoMethodCall<TMockObject>(this TMockObject mockObject, Expression<Action<TMockObject>> expectedMethodCallExpression)
            where TMockObject : MockObject
        {
            if (mockObject == null)
            {
                throw new ArgumentNullException("mockObject");
            }

            if (expectedMethodCallExpression == null)
            {
                throw new ArgumentNullException("expectedMethodCallExpression");
            }

            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            visitor.Visit(expectedMethodCallExpression);

            Assert.IsFalse(mockObject.RecordedMethodCalls.Any(c => c.Method == visitor.MethodCallInfo));
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = SuppressMessages.StronglyTypedExpressionNeeded)]
        public static void AssertNoMethodCallWithArguments<TMockObject>(this TMockObject mockObject, Expression<Action<TMockObject>> expectedMethodCallExpression)
            where TMockObject : MockObject
        {
            if (mockObject == null)
            {
                throw new ArgumentNullException("mockObject");
            }

            if (expectedMethodCallExpression == null)
            {
                throw new ArgumentNullException("expectedMethodCallExpression");
            }

            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            visitor.Visit(expectedMethodCallExpression);

            RecordedMethodCall[] recordedMethodCalls = mockObject.RecordedMethodCalls
                .Where(c => c.Method == visitor.MethodCallInfo)
                .ToArray();

            Type[] parameterTypes = visitor.MethodCallInfo.GetParameters()
                .Select(p => p.ParameterType)
                .ToArray();

            Assert.IsFalse(recordedMethodCalls.Any(recordedMethodCall =>
                UnitTestHelpers.CompareMethodCallArguments(parameterTypes, visitor.MethodCallArguments, recordedMethodCall.Arguments, mockObject.EqualityComparerFactory)));
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = SuppressMessages.StronglyTypedExpressionNeeded)]
        public static void AssertMethodCallWithArguments<TMockObject>(this TMockObject mockObject, Expression<Action<TMockObject>> expectedMethodCallExpression)
            where TMockObject : MockObject
        {
            if (mockObject == null)
            {
                throw new ArgumentNullException("mockObject");
            }

            if (expectedMethodCallExpression == null)
            {
                throw new ArgumentNullException("expectedMethodCallExpression");
            }

            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            visitor.Visit(expectedMethodCallExpression);

            RecordedMethodCall[] recordedMethodCalls = mockObject.RecordedMethodCalls
                .Where(c => c.Method == visitor.MethodCallInfo)
                .ToArray();

            Type[] parameterTypes = visitor.MethodCallInfo.GetParameters()
                .Select(p => p.ParameterType)
                .ToArray();

            RecordedMethodCall methodCallMatch = recordedMethodCalls.FirstOrDefault(recordedMethodCall =>
                UnitTestHelpers.CompareMethodCallArguments(parameterTypes, visitor.MethodCallArguments, recordedMethodCall.Arguments, mockObject.EqualityComparerFactory));

            Assert.IsNotNull(methodCallMatch, "Method '{0}' was not called.", visitor.MethodCallInfo);

            mockObject.RecordedMethodCalls.Remove(methodCallMatch);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = SuppressMessages.StronglyTypedExpressionNeeded)]
        public static RecordedMethodCall AssertMethodCallOnce<TMockObject>(this TMockObject mockObject, Expression<Action<TMockObject>> expectedMethodCallExpression)
            where TMockObject : MockObject
        {
            return mockObject.AssertMethodCallOnceInternal(expectedMethodCallExpression, false);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = SuppressMessages.StronglyTypedExpressionNeeded)]
        public static void AssertMethodCallOnceWithArguments<TMockObject>(this TMockObject mockObject, Expression<Action<TMockObject>> expectedMethodCallExpression)
            where TMockObject : MockObject
        {
            mockObject.AssertMethodCallOnceInternal(expectedMethodCallExpression, true);
        }

        internal static bool CompareMethodCallArguments(IReadOnlyList<Type> parameterTypes, IReadOnlyList<object> expectedArguments, IReadOnlyList<object> recordedArguments, IEqualityComparerFactory equalityComparerFactory)
        {
            if ((parameterTypes.Count != recordedArguments.Count) || (parameterTypes.Count != expectedArguments.Count))
            {
                return false;
            }

            for (int i = 0; i < parameterTypes.Count; i++)
            {
                if (!UnitTestHelpers.CompareMethodCallArgument(parameterTypes[i], expectedArguments[i], recordedArguments[i], equalityComparerFactory))
                {
                    return false;
                }
            }

            return true;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = SuppressMessages.StronglyTypedExpressionNeeded)]
        private static RecordedMethodCall AssertMethodCallOnceInternal<TMockObject>(this TMockObject mockObject, Expression<Action<TMockObject>> expectedMethodCallExpression, bool compareArguments)
            where TMockObject : MockObject
        {
            if (mockObject == null)
            {
                throw new ArgumentNullException("mockObject");
            }

            if (expectedMethodCallExpression == null)
            {
                throw new ArgumentNullException("expectedMethodCallExpression");
            }

            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            visitor.Visit(expectedMethodCallExpression);

            RecordedMethodCall[] recordedMethodCalls;

            lock (mockObject.RecordedMethodCalls)
            {
                recordedMethodCalls = mockObject.RecordedMethodCalls
                    .Where(c => c.Method == visitor.MethodCallInfo)
                    .ToArray();
            }

            switch (recordedMethodCalls.Length)
            {
                case 0:
                    Assert.Fail("Method '{0}' was not called.", visitor.MethodCallInfo.Name);
                    break;

                case 1:
                    break;

                default:
                    Assert.Fail("Method '{0}' was called {1} times instead of 1.", visitor.MethodCallInfo.Name, recordedMethodCalls.Length);
                    break;
            }

            RecordedMethodCall result = recordedMethodCalls.First();

            if (compareArguments)
            {
                Type[] parameterTypes = visitor.MethodCallInfo.GetParameters()
                    .Select(p => p.ParameterType)
                    .ToArray();

                Assert.IsTrue(UnitTestHelpers.CompareMethodCallArguments(parameterTypes, visitor.MethodCallArguments, result.Arguments, mockObject.EqualityComparerFactory));
            }

            mockObject.RecordedMethodCalls.Remove(result);

            return result;
        }

        private static bool CompareMethodCallArgument(Type parameterType, object expectedValue, object recordedValue, IEqualityComparerFactory equalityComparerFactory)
        {
            if (expectedValue == null)
            {
                return recordedValue == null;
            }

            if (recordedValue == null)
            {
                return false;
            }

            if (!parameterType.IsAssignableFrom(expectedValue.GetType()) || !parameterType.IsAssignableFrom(recordedValue.GetType()))
            {
                return false;
            }

            MethodInfo genericCompareArgumentMethod = typeof(UnitTestHelpers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .First(m => (m.Name == "CompareMethodCallArgument") && m.IsGenericMethodDefinition);

            genericCompareArgumentMethod = genericCompareArgumentMethod.MakeGenericMethod(parameterType);

            return (bool)genericCompareArgumentMethod.Invoke(null, new object[] { expectedValue, recordedValue, equalityComparerFactory });
        }

        private static bool CompareMethodCallArgument<T>(T expectedValue, T recordedValue, IEqualityComparerFactory equalityComparerFactory)
        {
            if (typeof(T).IsGenericType)
            {
                string assertMethodCallCollectionArgumentMethodName = null;

                if (typeof(T).GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    IExpectedCollection expectedCollection = expectedValue as IExpectedCollection;

                    if (expectedCollection == null)
                    {
                        throw new NotSupportedException("Cannot compare collection of type {0}. Use {1} instead.".FormatInvariant(expectedValue.GetType().Name, typeof(IExpectedCollection).Name));
                    }

                    assertMethodCallCollectionArgumentMethodName = expectedCollection.IsOrdered ? "CompareMethodCallExpectedOrderedCollectionArgument" : "CompareMethodCallExpectedCollectionArgument";
                }
                else if ((typeof(T).GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)) || (typeof(T).GetGenericTypeDefinition() == typeof(IDictionary<,>)))
                {
                    assertMethodCallCollectionArgumentMethodName = "CompareMethodCallDictionaryArgument";
                }

                if (!string.IsNullOrEmpty(assertMethodCallCollectionArgumentMethodName))
                {
                    MethodInfo compareCollectionsMethod = typeof(UnitTestHelpers).GetMethod(assertMethodCallCollectionArgumentMethodName, BindingFlags.Static | BindingFlags.NonPublic);

                    compareCollectionsMethod = compareCollectionsMethod.MakeGenericMethod(typeof(T).GetGenericArguments());

                    return (bool)compareCollectionsMethod.Invoke(null, new object[] { expectedValue, recordedValue, equalityComparerFactory });
                }
            }

            IEqualityComparer<T> equalityComparer = equalityComparerFactory.GetEqualityComparer<T>();

            return equalityComparer.Equals(expectedValue, recordedValue);
        }

        private static bool CompareMethodCallExpectedOrderedCollectionArgument<T>(IEnumerable<T> expectedValues, IEnumerable<T> recordedValues, IEqualityComparerFactory equalityComparerFactory)
        {
            IEnumerator<T> expectedValuesEnumerator = expectedValues.GetEnumerator();
            IEnumerator<T> recordedValuesEnumerator = recordedValues.GetEnumerator();

            while (expectedValuesEnumerator.MoveNext())
            {
                if (!recordedValuesEnumerator.MoveNext())
                {
                    return false;
                }

                if (!UnitTestHelpers.CompareMethodCallArgument(expectedValuesEnumerator.Current, recordedValuesEnumerator.Current, equalityComparerFactory))
                {
                    return false;
                }
            }

            return !recordedValuesEnumerator.MoveNext();
        }

        private static bool CompareMethodCallExpectedCollectionArgument<T>(IEnumerable<T> expectedValues, IEnumerable<T> recordedValues, IEqualityComparerFactory equalityComparerFactory)
        {
            IEqualityComparer<T> equalityComparer = equalityComparerFactory.GetEqualityComparer<T>();

            List<T> expectedValuesList = new List<T>(expectedValues);
            List<T> recordedValuesList = new List<T>(recordedValues);

            while (expectedValuesList.Any())
            {
                T expectedValue = expectedValuesList.First();

                List<T> foundExpectedValues = expectedValuesList.FindAll(v => equalityComparer.Equals(expectedValue, v));
                List<T> foundRecordedValues = recordedValuesList.FindAll(v => equalityComparer.Equals(expectedValue, v));

                if (foundExpectedValues.Count != foundRecordedValues.Count)
                {
                    return false;
                }

                expectedValuesList.RemoveRange(foundExpectedValues);
                recordedValuesList.RemoveRange(foundRecordedValues);
            }

            return recordedValuesList.Count == 0;
        }

        private static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> itemsToRemove)
        {
            foreach (T item in itemsToRemove)
            {
                collection.Remove(item);
            }
        }

        private static bool CompareMethodCallDictionaryArgument<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> expectedValues, IEnumerable<KeyValuePair<TKey, TValue>> recordedValues, IEqualityComparerFactory equalityComparerFactory)
        {
            IEqualityComparer<TValue> equalityComparer = equalityComparerFactory.GetEqualityComparer<TValue>();

            Dictionary<TKey, TValue> expectedDictionary = new Dictionary<TKey, TValue>();
            Dictionary<TKey, TValue> recordedDictionary = new Dictionary<TKey, TValue>();

            foreach (var expectedValue in expectedValues)
            {
                expectedDictionary.Add(expectedValue.Key, expectedValue.Value);
            }

            foreach (var recordedValue in recordedValues)
            {
                recordedDictionary.Add(recordedValue.Key, recordedValue.Value);
            }

            while (expectedDictionary.Any())
            {
                var expectedPair = expectedDictionary.First();

                TValue recordedValue;

                if (!recordedDictionary.TryGetValue(expectedPair.Key, out recordedValue))
                {
                    return false;
                }

                if (!equalityComparer.Equals(expectedPair.Value, recordedValue))
                {
                    return false;
                }

                expectedDictionary.Remove(expectedPair.Key);
                recordedDictionary.Remove(expectedPair.Key);
            }

            return !recordedDictionary.Any();
        }
    }
}