using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Radoslav.Linq.Expressions
{
    /// <summary>
    /// An expression visitor that parses an expression to a method info and parameters.
    /// </summary>
    internal sealed class MethodCallExpressionVisitor : ExpressionVisitor
    {
        private readonly List<object> methodCallArguments = new List<object>();
        private readonly Stack<Action<object>> valueFoundCallbacks = new Stack<Action<object>>();

        /// <summary>
        /// Gets the method call defined in the visited expression.
        /// </summary>
        /// <value>The values of the method arguments defined in the visited expression.</value>
        internal MethodInfo MethodCallInfo { get; private set; }

        /// <summary>
        /// Gets the values of the method arguments defined in the visited expression.
        /// </summary>
        /// <value>The values of the method arguments defined in the visited expression.</value>
        internal IReadOnlyList<object> MethodCallArguments
        {
            get { return this.methodCallArguments; }
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (this.MethodCallInfo == null)
            {
                this.MethodCallInfo = node.Method;

                this.VisitMethodArguments(node, value => this.methodCallArguments.Add(value));
            }
            else
            {
                List<object> methodArguments = new List<object>();

                object instance = null;

                // For static methods node.Object is null.
                if (node.Object != null)
                {
                    this.valueFoundCallbacks.Push(value => instance = value);

                    this.Visit(node.Object);
                }

                this.VisitMethodArguments(node, value => methodArguments.Add(value));

                object methodResult = node.Method.Invoke(instance, methodArguments.ToArray());

                this.OnValueFound(methodResult);
            }

            return node;
        }

        /// <inheritdoc />
        protected override Expression VisitNew(NewExpression node)
        {
            this.CreateNewObject(node);

            return node;
        }

        /// <inheritdoc />
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            Type elementType = node.Type.GetElementType();

            Array array = Array.CreateInstance(elementType, node.Expressions.Count);

            for (int i = 0; i < node.Expressions.Count; i++)
            {
                this.valueFoundCallbacks.Push(value => array.SetValue(value, i));

                this.Visit(node.Expressions[i]);
            }

            this.OnValueFound(array);

            return node;
        }

        /// <inheritdoc />
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            this.OnValueFound(node.Value);

            return node;
        }

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            object owner = null;

            if (node.Expression != null)
            {
                this.valueFoundCallbacks.Push(v => owner = v);

                this.Visit(node.Expression);

                if (owner == null)
                {
                    throw new InvalidOperationException();
                }
            }

            object value;

            switch (node.Member.MemberType)
            {
                case MemberTypes.Field:
                    value = ((FieldInfo)node.Member).GetValue(owner);
                    break;

                case MemberTypes.Property:
                    value = ((PropertyInfo)node.Member).GetValue(owner);
                    break;

                default:
                    throw new NotSupportedException<MemberTypes>(node.Member.MemberType);
            }

            this.OnValueFound(value);

            return node;
        }

        /// <inheritdoc />
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            object leftValue = null, rightValue = null;

            this.valueFoundCallbacks.Push(value => leftValue = value);

            this.Visit(node.Left);

            this.valueFoundCallbacks.Push(value => rightValue = value);

            this.Visit(node.Right);

            object result;

            if (node.NodeType == ExpressionType.ArrayIndex)
            {
                result = ((Array)leftValue).GetValue((int)rightValue);
            }
            else if (node.Type == typeof(int))
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        result = ((int)leftValue) + ((int)rightValue);
                        break;

                    default:
                        throw new NotSupportedException<ExpressionType>(node.NodeType);
                }
            }
            else if (node.Type == typeof(DateTime))
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        if ((leftValue.GetType() == typeof(DateTime)) && (rightValue.GetType() == typeof(TimeSpan)))
                        {
                            result = ((DateTime)leftValue) + ((TimeSpan)rightValue);
                        }
                        else
                        {
                            throw new NotSupportedException("Operation '{0}' between types '{1}' and '{2}' is not supported.".FormatInvariant(node.NodeType, leftValue.GetType(), rightValue.GetType()));
                        }

                        break;

                    default:
                        throw new NotSupportedException<ExpressionType>(node.NodeType);
                }
            }
            else
            {
                throw new NotSupportedException("Node Type '{0}' is not supported.".FormatInvariant(node.Type));
            }

            this.OnValueFound(result);

            return node;
        }

        /// <inheritdoc />
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            object instance = this.CreateNewObject(node.NewExpression);

            foreach (MemberBinding binding in node.Bindings)
            {
                switch (binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        this.valueFoundCallbacks.Push(value =>
                            {
                                switch (binding.Member.MemberType)
                                {
                                    case MemberTypes.Property:
                                        ((PropertyInfo)binding.Member).SetValue(instance, value);
                                        break;

                                    default:
                                        throw new NotSupportedException<MemberTypes>(binding.Member.MemberType);
                                }
                            });

                        this.VisitMemberAssignment((MemberAssignment)binding);
                        break;

                    default:
                        throw new NotSupportedException<MemberBindingType>(binding.BindingType);
                }
            }

            return node;
        }

        private void VisitMethodArguments(MethodCallExpression node, Action<object> onValueFound)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                this.valueFoundCallbacks.Push(onValueFound);

                switch (node.Arguments[i].NodeType)
                {
                    case ExpressionType.Add:
                    case ExpressionType.Call:
                    case ExpressionType.Constant:
                    case ExpressionType.Convert:
                    case ExpressionType.MemberAccess:
                    case ExpressionType.New:
                    case ExpressionType.NewArrayInit:
                    case ExpressionType.MemberInit:
                        this.Visit(node.Arguments[i]);
                        break;

                    default:
                        throw new NotSupportedException<ExpressionType>(node.Arguments[i].NodeType);
                }
            }
        }

        private void OnValueFound(object value)
        {
            Action<object> valueFoundCallback = this.valueFoundCallbacks.Pop();

            valueFoundCallback(value);
        }

        private object CreateNewObject(NewExpression node)
        {
            object[] constructorParameters = new object[node.Arguments.Count];

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                this.valueFoundCallbacks.Push(value => constructorParameters[i] = value);

                this.Visit(node.Arguments[i]);
            }

            object result = node.Constructor.Invoke(constructorParameters);

            this.OnValueFound(result);

            return result;
        }
    }
}