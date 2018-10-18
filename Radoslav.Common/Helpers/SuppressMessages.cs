namespace Radoslav
{
    /// <summary>
    /// Reusable messages for <see cref="System.Diagnostics.CodeAnalysis.SuppressMessageAttribute"/> attribute justification.
    /// </summary>
    public static class SuppressMessages
    {
        /// <summary>
        /// A justification for performance optimization.
        /// </summary>
        public const string PerformanceOptimization = "Performance optimization to skip property access in inheriting classes.";

        /// <summary>
        /// A justification because not resource expensive and disposed in the destructor.
        /// </summary>
        public const string ResourceNotExpensiveAndDisposedInDestructor = "Not resource expensive and disposed in the destructor.";

        /// <summary>
        /// A justification because strongly-typed expression is really needed in order to pass information in the specified expression.
        /// </summary>
        public const string StronglyTypedExpressionNeeded = "Strongly-typed expression is needed in order to pass information in the specified expression.";

        /// <summary>
        /// A justification because the property or class is used by UI or other client via reflection.
        /// </summary>
        public const string UsedViaReflection = "Used by UI or other client via reflection.";

        /// <summary>
        /// A justification because the caller of this method is responsible for disposing the result.
        /// </summary>
        public const string CallerResponsibleForDispose = "The caller of this method is responsible for disposing the result.";

        /// <summary>
        /// A justification because the instance initialized in [TestInitialize] method will be disposed in [TestCleanup] method.
        /// </summary>
        public const string DisposedOnTestCleanup = "Disposed in [TestCleanup] method";

        /// <summary>
        /// A justification because the initialization code is too long to be inline.
        /// </summary>
        public const string InitializationTooLong = "Initialization code is too long to be inline.";

        /// <summary>
        /// A justification because a field is disposed in the Dispose method of the classes where it is defined.
        /// </summary>
        public const string FieldDisposedInDisposeMethod = "Field is disposed in dispose method.";

        /// <summary>
        /// A justification for copy/paste code.
        /// </summary>
        public const string CopyPasteCode = "The code is copy/pasted from Stack Overflow or another source.";

        /// <summary>
        /// A justification that a value is never null.
        /// </summary>
        public const string ValueIsNeverNull = "Value is used only internally and is never null.";

        /// <summary>
        /// A justification that it is more clear for the developer using the IntelliSense this way.
        /// </summary>
        public const string IntelliSense = "It is more clear for the IntelliSense this way.";

        /// <summary>
        /// A justification for a configuration property used via Reflection.
        /// </summary>
        public const string ConfigurationProperty = "This class or property is used via reflection by App.Config configuration manager.";

        /// <summary>
        /// A justification that something is like this because of serialization concerns.
        /// </summary>
        public const string Serialization = "It needs to be like this because of serialization concerns.";

        /// <summary>
        /// A justification that something is OK.
        /// </summary>
        public const string FuckOff = "I know but I am OK with that..";
    }
}