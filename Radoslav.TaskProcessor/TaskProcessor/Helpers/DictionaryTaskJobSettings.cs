using System;
using System.Runtime.Serialization;
using Radoslav.Collections;

namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// An implementation of the <see cref="ITaskJobSettings"/> interface that contains the task job settings as a string dictionary.
    /// </summary>
    /// <remarks>This class must be serializable to binary, XML, JSON and/or other formats.</remarks>
    [Serializable]
    public sealed class DictionaryTaskJobSettings : SerializableStringDictionary, ITaskJobSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryTaskJobSettings"/> class.
        /// </summary>
        public DictionaryTaskJobSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryTaskJobSettings"/> class with the serialized data.
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> object containing the information required to serialize the <see cref="DictionaryTaskSummary"/>.</param>
        /// <param name="context">A <see cref=" StreamingContext"/> structure containing the source and destination of the serialized stream associated with the <see cref=" DictionaryTaskSummary"/>.</param>
        private DictionaryTaskJobSettings(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Constructors
    }
}