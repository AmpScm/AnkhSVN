using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Text;


namespace Rogue.DocParser
{
    /// <summary>
    /// Summary description for Parser.
    /// </summary>
    public class Parser
    {
        public int Run( string[] args )
        {
            syntax parser = new syntax();
            Header hdr;
            using( StreamReader reader = new StreamReader( args[0] ) )
                hdr = (Header) parser.Parse( reader );

            if ( hdr == null ) 
                return 1;
           
            XmlDocVisitor visitor = new XmlDocVisitor();
            hdr.Accept( visitor );

            StringBuilder b = new StringBuilder();
            FormatXml( visitor.Doc.DocumentElement, b, 0 );

            Console.WriteLine( b.ToString() );
            using( StreamWriter writer = new StreamWriter( args[1] ) )
                writer.Write( b.ToString() );

            return 0;
        }

        public static int Main( string[] args )
        {
            return new Parser().Run( args );
        }
        

        private static void FormatXml( XmlNode node, StringBuilder text, int level )
        {
            if ( node is XmlWhitespace )
                return;

            const int INDENT = 4;
            text.AppendFormat( "{0}<{1}{2}>", new String( ' ', INDENT*level), node.Name,
                FormatAttributes( node ) );

            if ( (node.ChildNodes.Count == 1 && node.ChildNodes[0] is XmlText) ||
                node.ChildNodes.Count == 0 )
            {
                text.AppendFormat( "{0}</{1}>{2}", node.InnerText, node.Name, 
                    Environment.NewLine );
            }
            else
            {
                text.Append( System.Environment.NewLine );
                foreach( XmlNode child in node.ChildNodes )
                    FormatXml( child, text, level + 1 );
                text.AppendFormat( "{0}</{1}>{2}", 
                    new String( ' ', INDENT*level), node.Name, Environment.NewLine );
            }           

        }

        private static string FormatAttributes( XmlNode node )
        {
            StringBuilder builder = new StringBuilder();
            foreach( XmlAttribute attr in node.Attributes )
                builder.AppendFormat( " {0}='{1}'", attr.Name, attr.Value );
            
            return builder.ToString();
        }
    }

    internal class XmlDocVisitor : NodeVisitor
    {
        public XmlDocVisitor()
        {
            this.names = new Stack();
            this.doc = new XmlDocument();
            XmlNode root = this.doc.AppendChild( this.doc.CreateElement( "doc" ) );
            this.membersNode = root.AppendChild( this.doc.CreateElement( "members" ) );
            
        }
        /// <summary>
        /// Visits the file itself
        /// </summary>
        /// <param name="header"></param>
        public override void Visit( Header header )
        {
            header.Elements.Accept( this );
        }

        /// <summary>
        /// Visits a list of file elements
        /// </summary>
        /// <param name="list"></param>
        public override void Visit( FileElementList list )
        {
            FileElementList iter = list;
            while( iter != null )
            {
                iter.Head.Accept( this );
                iter = iter.Tail;
            }
        }

        /// <summary>
        /// Visits a namespace
        /// </summary>
        /// <param name="ns"></param>
        public override void Visit( NamespaceFileElement ns )
        {
            this.names.Push( ns.Namespace.Name );
            ns.Namespace.Elements.Accept( this );
            this.names.Pop();           
        }

        /// <summary>
        /// Visits a class element
        /// </summary>
        /// <param name="cl"></param>
        public override void Visit( ClassFileElement cl )
        {
            this.names.Push( cl.Class.ClassDecl.Name );
            this.WriteMember( "T", cl.Class.DocComment );

            cl.Class.ClassDecl.Members.Accept( this );

            this.names.Pop();

        }

        /// <summary>
        /// Visits a list of class members
        /// </summary>
        /// <param name="m"></param>
        public override void Visit( MemberList m )
        {
            MemberList iter = m;
            while( iter != null )
            {
                iter.Head.Accept( this );
                iter = iter.Tail;
            }
        }

        /// <summary>
        /// Visits a method declaration
        /// </summary>
        /// <param name="item"></param>
        public override void Visit( MethodDeclItem item )
        {
            this.names.Push( item.MethodDecl.Name );
            this.WriteMember( "M", item.DocComment );
            this.names.Pop();
                         
        }

        public XmlDocument Doc 
        {
            get{ return this.doc; }
        }

        private void WriteMember( string type, DocComment docComment )
        {
            string[] types = new string[ this.names.Count ];
            this.names.CopyTo( types, 0 );
            Array.Reverse( types );
            string typeString = type + ":" + string.Join( ".", types );
            XmlNode newNode = this.membersNode.AppendChild( this.doc.CreateElement( "member" ) );
            newNode.Attributes.Append( this.doc.CreateAttribute( "name" ) );
            newNode.Attributes[ "name" ].InnerText = typeString;

            if ( docComment != null )
                newNode.InnerXml = docComment.Comment;
        }

        private Stack names;   
        private XmlNode membersNode;
        private XmlDocument doc;

    }
}
