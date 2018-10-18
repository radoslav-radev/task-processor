namespace System.Xml
{
    /// <summary>
    /// Class for <see cref="XmlReader"/> extensions.
    /// </summary>
    public static class XmlReaderExtensions
    {
        /// <summary>
        /// Reads the content of a XML node as a <see cref="Guid"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> instance to extend.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="reader"/> is null.</exception>
        /// <exception cref="FormatException">Content cannot be parsed to a value <see cref="Guid"/>.</exception>
        /// <returns>The content of the XML node as a <see cref="Guid"/>.</returns>
        public static Guid ReadContentAsGuid(this XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            string value = reader.ReadContentAsString();

            return Guid.Parse(value);
        }
    }
}