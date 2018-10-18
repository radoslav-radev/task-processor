using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Timers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Linq.Expressions;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class MethodCallExpressionVisitorTests
    {
        [TestMethod]
        public void NoArguments()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.Clear();

            visitor.Visit(expression);

            Assert.AreEqual(0, visitor.MethodCallArguments.Count);
        }

        [TestMethod]
        public void ConstantArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.Add(1);

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);
            Assert.AreEqual(1, visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void TwoConstantArguments()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.GetRange(1, 2);

            visitor.Visit(expression);

            Assert.AreEqual(2, visitor.MethodCallArguments.Count);
            Assert.AreEqual(1, visitor.MethodCallArguments[0]);
            Assert.AreEqual(2, visitor.MethodCallArguments[1]);
        }

        [TestMethod]
        public void LocalVariableArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            object[] buffer = new object[0];

            Expression<Action> expression = () => collection.CopyTo(buffer);

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.AreSame(buffer, visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void LocalVariableNullArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            object[] buffer = null;

            Expression<Action> expression = () => collection.CopyTo(buffer);

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.IsNull(visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void NewOneElementArrayArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.CopyTo(new object[] { 5 });

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.IsInstanceOfType(visitor.MethodCallArguments[0], typeof(object[]));

            Assert.AreEqual(1, ((object[])visitor.MethodCallArguments[0]).Length);
            Assert.AreEqual(5, ((object[])visitor.MethodCallArguments[0])[0]);
        }

        [TestMethod]
        public void NewTwoElementsArrayArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.CopyTo(new object[] { 5, 6 });

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.IsInstanceOfType(visitor.MethodCallArguments[0], typeof(object[]));

            Assert.AreEqual(2, ((object[])visitor.MethodCallArguments[0]).Length);
            Assert.AreEqual(5, ((object[])visitor.MethodCallArguments[0])[0]);
            Assert.AreEqual(6, ((object[])visitor.MethodCallArguments[0])[1]);
        }

        [TestMethod]
        public void NewEmptyListArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.Equals(new List<int>());

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.IsInstanceOfType(visitor.MethodCallArguments[0], typeof(List<int>));

            Assert.IsTrue(((List<int>)visitor.MethodCallArguments[0]).IsEmpty());
        }

        [TestMethod]
        public void PropertyArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            DateTime timestamp = new DateTime(2015, 1, 1);

            Expression<Action> expression = () => collection.Equals(timestamp.Day);

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.AreEqual(1, visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void StaticPropertyArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.Equals(DateTime.Today);

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.AreEqual(DateTime.Today, (DateTime)visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void PropertyChainArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            DateTime timestamp = new DateTime(2015, 1, 1);

            Expression<Action> expression = () => collection.Equals(timestamp.Date.Year);

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.AreEqual(2015, visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void StaticPropertyChainArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.Equals(DateTime.Now.Date.Year);

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.AreEqual(DateTime.Today.Year, visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void IntegerSumArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.Add(DateTime.Now.Day + DateTime.Now.Month);

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.AreEqual(DateTime.Now.Day + DateTime.Now.Month, visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void DateTimePlusTimeSpanArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.Add(DateTime.Today + DateTime.Today.TimeOfDay);

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.AreEqual(DateTime.Today, visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void MethodCallWithoutParametersArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.Add(collection.GetType());

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.AreEqual(collection.GetType(), visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void MethodCallWithParametersArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            DateTime timestamp = new DateTime(2015, 1, 1);

            Expression<Action> expression = () => collection.Add(timestamp.AddYears(1));

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.AreEqual(new DateTime(2016, 1, 1), visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void StaticMethodCallWithParametersArgument()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.Add(DateTime.DaysInMonth(2015, 1));

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.AreEqual(31, visitor.MethodCallArguments[0]);
        }

        [TestMethod]
        public void PropertyInitializationEmptyConstructor()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.Add(new Timer() { AutoReset = false });

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.IsInstanceOfType(visitor.MethodCallArguments[0], typeof(Timer));

            Assert.IsFalse(((Timer)visitor.MethodCallArguments[0]).AutoReset);
        }

        [TestMethod]
        public void PropertyInitializationWithConstructor()
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor();

            ArrayList collection = new ArrayList();

            Expression<Action> expression = () => collection.Add(new Timer(1000) { AutoReset = false, Enabled = true });

            visitor.Visit(expression);

            Assert.AreEqual(1, visitor.MethodCallArguments.Count);

            Assert.IsInstanceOfType(visitor.MethodCallArguments[0], typeof(Timer));

            Assert.AreEqual(1000, ((Timer)visitor.MethodCallArguments[0]).Interval);
            Assert.IsFalse(((Timer)visitor.MethodCallArguments[0]).AutoReset);
            Assert.IsTrue(((Timer)visitor.MethodCallArguments[0]).Enabled);
        }
    }
}