using System;
using System.Runtime.Serialization;
using Radoslav.Collections;

namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// An implementation of the <see cref="ITaskSummary"/> interface that represents the summary as a dictionary.
    /// </summary>
    /// <remarks>This class must be serializable to binary, XML, JSON and/or other formats.</remarks>
    [Serializable]
    public sealed class DictionaryTaskSummary : SerializableStringDictionary, ITaskSummary
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryTaskSummary"/> class.
        /// </summary>
        public DictionaryTaskSummary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryTaskSummary"/> class with the serialized data.
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> object containing the information required to serialize the <see cref="DictionaryTaskSummary"/>.</param>
        /// <param name="context">A <see cref=" StreamingContext"/> structure containing the source and destination of the serialized stream associated with the <see cref=" DictionaryTaskSummary"/>.</param>
        private DictionaryTaskSummary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Constructors
    }
}