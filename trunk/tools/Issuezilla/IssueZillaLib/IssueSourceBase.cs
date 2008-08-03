using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Fines.IssueZillaLib
{
    public abstract class IssueSourceBase : IIssueSource
    {
        public abstract issuezilla GetAllIssues();

        protected issuezilla Deserialize( StreamReader reader )
        {
            XmlSerializer serializer = new XmlSerializer( typeof( issuezilla ) );
            issuezilla zilla = (issuezilla)serializer.Deserialize( reader );
            return zilla;
        }

        protected void Serialize( issuezilla zilla, StreamWriter writer )
        {
            XmlSerializer serializer = new XmlSerializer( typeof( issuezilla ) );
            serializer.Serialize( writer, zilla );
        }
    }
}
