using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Radoslav.Collections
{
    /// <summary>
    /// A dictionary is serializable to binary, XML, JSON and/or other formats.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    [Serializable]
    public abstract class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        protected SerializableDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class with the serialized data.
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> object containing the information required to serialize the <see cref="SerializableDictionary{TKey, TValue}"/>.</param>
        /// <param name="context">A <see cref=" StreamingContext"/> structure containing the source and destination of the serialized stream associated with the <see cref=" SerializableDictionary{TKey, TValue}"/>.</param>
        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Constructors

        #region IXmlSerializable

        /// <summary>
        /// This method is reserved and should not be used.
        /// </summary>
        /// <returns>An XmlSchema that describes the XML representation of the object that is produced by the WriteXml method and consumed by the ReadXml method.</returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader" /> stream from which the object is deserialized.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="reader"/> is null.</exception>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!reader.IsEmptyElement)
            {
                while (reader.ReadToDescendant("Key"))
                {
                    string key = reader.ReadElementContentAsString();

                    string value = reader.ReadElementContentAsString();

                    this.Add(this.DeserializeKey(key), this.DeserializeValue(value));

                    reader.ReadEndElement(); // </Pair>
                }
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter" /> stream to which the object is serialized.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="writer"/> is null.</exception>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            foreach (var pair in this)
            {
                writer.WriteStartElement("Pair");

                writer.WriteElementString("Key", this.SerializeKey(pair.Key));
                writer.WriteElementString("Value", this.SerializeValue(pair.Value));

                writer.WriteEndElement();
            }
        }

        #endregion IXmlSerializable

        #region Keys & Values Serialization

        /// <summary>
        /// Serializes a dictionary key to string.
        /// </summary>
        /// <param name="key">The key to serialize.</param>
        /// <returns>A serialized string for the key.</returns>
        protected abstract string SerializeKey(TKey key);

        /// <summary>
        /// Serializes a dictionary value to string.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <returns>A serialized string for the value.</returns>
        protected abstract string SerializeValue(TValue value);

        /// <summary>
        /// Deserializes a dictionary key.
        /// </summary>
        /// <param name="key">The key to deserialize.</param>
        /// <returns>The deserialized key.</returns>
        protected abstract TKey DeserializeKey(string key);

        /// <summary>
        /// Deserializes a dictionary value.
        /// </summary>
        /// <param name="value">The value to deserialize.</param>
        /// <returns>The deserialized value.</returns>
        protected abstract TValue DeserializeValue(string value);

        #endregion Keys & Values Serialization
    }
}