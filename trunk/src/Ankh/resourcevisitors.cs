using System;
using NSvn;
using NSvn.Common;
using NSvn.Core;
using System.IO;
using System.Collections;
using System.Text;

namespace Ankh
{
    /// <summary>
    /// A visitor that checks if the visitees are modified.
    /// </summary>
    internal class ModifiedVisitor : LocalResourceVisitorBase
    {
        public bool Modified = false;
        public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
        {
            if ( resource.Status.TextStatus != StatusKind.Normal || 
                (resource.Status.PropertyStatus != StatusKind.Normal && 
                resource.Status.PropertyStatus != StatusKind.None ) )
                this.Modified = true;
        }
    }

    /// <summary>
    /// A visitor that checks if all the visitees are versioned.
    /// </summary>
    internal class VersionedVisitor : LocalResourceVisitorBase
    {
        public bool IsVersioned = true;
        public override void VisitUnversionedResource( UnversionedResource r )
        {
            this.IsVersioned = false;
        }
    }

    /// <summary>
    /// A visitor that checks if all the visitees are unversioned.
    /// </summary>
    internal class UnversionedVisitor : LocalResourceVisitorBase
    {
        public bool IsVersioned = false;
        public override void VisitWorkingCopyResource( WorkingCopyResource r )
        {
            this.IsVersioned = true;
        }
    }

    internal class ResourceGathererVisitor : LocalResourceVisitorBase
    {
        public ArrayList WorkingCopyResources = new ArrayList();
        public ArrayList UnversionedResources = new ArrayList();

        public override void VisitUnversionedResource(NSvn.UnversionedResource resource)
        {
            this.UnversionedResources.Add( resource );
        }

        public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
        {
            this.WorkingCopyResources.Add( resource );            
        }
    }

    internal class DiffVisitor : LocalResourceVisitorBase
    {
//        public string DiffFile
//        {
//            get
//            {
//                this.stream.Close();
//                return this.GenerateHtmlFile();
//            }
//        }

        public string Diff
        {
            get{ return Encoding.UTF8.GetString( this.stream.ToArray() ); }
        }


        public DiffVisitor()
        {
            this.stream = new MemoryStream();           
        }

        public override void VisitWorkingCopyFile(NSvn.WorkingCopyFile file)
        {
            file.Diff( this.stream, Stream.Null );
        }

//        private string GenerateHtmlFile()
//        {
//            string htmlFile = Path.GetTempFileName() + ".html";
//            using ( StreamWriter writer = new StreamWriter( htmlFile, false ) )
//            {
//                this.WriteHeader( writer );
//                using( StreamReader reader = new StreamReader( this.diffFile ) )
//                {
//                    this.ParseLine(reader, writer);
//                } 
//                this.WriteFooter( writer );
//                   
//            }
//
//            return htmlFile;
//        }
//
//        private void ParseLine(StreamReader reader, StreamWriter writer)
//        {
//            string line;
//            while( ( line = reader.ReadLine() ) != null  )
//            {
//                string style = "default";
//                switch( line[0] )
//                {
//                    case '+':
//                        style = "plus";
//                        break;
//                    case '-':
//                        style = "minus";                            
//                        break;
//                }
//                writer.WriteLine( "<span class='{0}'>{1}</span>", style, 
//                    line.Replace( "<", "&lt;" ).Replace( ">", "&gt;" ) );
//            }
//        }
//
//        private void WriteHeader( StreamWriter writer )
//        {
//            writer.WriteLine( 
//                @"<html>
//   <head> 
//      <title>Diff</title>
//      <style type='text/css'>
//       <!--
//          .plus {  color: blue;  }
//          .minus { color: red; }
//          .default { color: green;}
//       -->
//      </style>
//</head>
//<body>
//    <pre>" );
//        }
//
//        private void WriteFooter( StreamWriter writer )
//        {
//            writer.WriteLine( "</pre></body></html>" );
//        }

        private MemoryStream stream;
    }
}
