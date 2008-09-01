using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Fines.IssueZillaLib
{
    public class FileIssueSource : IssueSourceBase, IIssueSink
    {
        public FileIssueSource( string file )
        {
            this.file = file;
        }

        public override issuezilla GetAllIssues()
        {
            using ( StreamReader reader = new StreamReader( this.file ) )
            {
                return this.Deserialize( reader );
            }
        }
        
        public void StoreIssues( issuezilla zilla )
        {
            using ( StreamWriter writer = new StreamWriter( this.file ) )
            {
                this.Serialize( zilla, writer );
            }
        }

        private string file;
    }
}
