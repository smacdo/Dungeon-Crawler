using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Scott.Game.Content
{
    /// <summary>
    ///  Abstract base class for content readers.
    /// </summary>
    internal abstract class ContentReader<T>
    {
        public ContentReader()
        {
            // empty
        }

        public abstract T Read( Stream input, string filePath, ContentManagerX content );
    }

    /// <summary>
    ///  Attribute that describes what type of content a given content reader class reads.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, Inherited = true )]
    public class ContentReaderAttribute : Attribute
    {
        public Type ContentType { get; private set; }
        public string Extension { get; private set; }

        public ContentReaderAttribute( Type contentType, string fileExtension )
        {
            ContentType = contentType;
            Extension = fileExtension;
        }
    }
}
