using System;
using System.Runtime.Serialization;

namespace Radoslav.Collections
{
    /// <summary>
    ///  A string dictionary is serializable to binary, XML, JSON and/or other formats.
    /// </summary>
    [Serializable]
    public class SerializableStringDictionary : SerializableDictionary<string, string>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableStringDictionary"/> class.
        /// </summary>
        protected SerializableStringDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableStringDictionary"/> class with the serialized data.
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> object containing the information required to serialize the <see cref="SerializableStringDictionary"/>.</param>
        /// <param name="context">A <see cref=" StreamingContext"/> structure containing the source and destination of the serialized stream associated with the <see cref=" SerializableStringDictionary"/>.</param>
        protected SerializableStringDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Constructors

        #region Keys & Values Serialization

        /// <inheritdoc />
        protected override string DeserializeKey(string key)
        {
            return key;
        }

        /// <inheritdoc />
        protected override string DeserializeValue(string value)
        {
            return value;
        }

        /// <inheritdoc />
        protected override string SerializeKey(string key)
        {
            return key;
        }

        /// <inheritdoc />
        protected override string SerializeValue(string value)
        {
            return value;
        }

        #endregion Keys & Values Serialization
    }
}