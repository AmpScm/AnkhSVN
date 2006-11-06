// $Id$
using System;

//Start: SDH Edit
using System.Runtime.Serialization;
//End: SDH Edit

namespace NSvn.Common
{
    /// <summary>
    /// Base class for all exceptions thrown from NSvn.
    /// </summary>
    [Serializable] //SDH Edit
    public class SvnException : ApplicationException
    {
        public SvnException()
        {

        }

        public SvnException( string message )
            : base( message )
        {

        }

        public SvnException( string message, Exception innerException )
            :
            base( message, innerException )
        {
        }

        //START: SDH Edit
        public SvnException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {

        }

        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );
        }
        //END: SDH Edit

    }
}
