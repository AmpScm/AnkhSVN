// $Id$
using System;
using System.Text;

namespace NSvn.Common
{
    /// <summary>
    /// Represents an SVN property
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="text">The text of the property. This will be UTF8-encoded</param>
        public Property( string name, string text ) : this( name, text, Encoding.Default )
        {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="text">The text of the property</param>
        /// <param name="encoding">The encoding to use</param>
        public Property( string name, string text, Encoding encoding ) : 
            this( name, encoding.GetBytes( text ) )
        {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="data">The data associated with the property</param>
        public Property( string name, byte[] data )
        {
            this.name = name;
            this.data = data;
        }

        /// <summary>
        /// The name of the property
        /// </summary>
        public string Name
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.name; }
        }

        /// <summary>
        /// The data of the property
        /// </summary>
        public byte[] Data
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.data; }
        }

        public override string ToString()
        {
            return this.GetString();
        }

        /// <summary>
        /// Returns the data of this property as a string, using the default encoding
        /// </summary>
        /// <returns>A string containing the property data</returns>
        public string GetString()
        {
            return Encoding.Default.GetString( this.data );
        }

        /// <summary>
        /// Returns the data of this property as a string
        /// </summary>
        /// <param name="encoding">The encoding to use</param>
        /// <returns>A string</returns>
        public string GetString( Encoding encoding )
        {
            return encoding.GetString( this.data );
        }


        private byte[] data;
        private string name;

    }
}
