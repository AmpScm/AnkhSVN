using System;
using System.IO;
using Tools;
using NUnit.Framework;

namespace Rogue.DocParser
{
	/// <summary>
	/// Summary description for TestParser.
	/// </summary>
	[TestFixture]
    public class TestParser
	{
        [Test]
        public void TestBasic()
        {
            syntax parser = new syntax();
            StreamReader reader = new StreamReader(  
                this.GetType().Assembly.GetManifestResourceStream( "Rogue.DocParser.Header2.txt") );
            Header hdr = (Header) parser.Parse( reader );
           
            hdr.Accept( new Visitor() );

            Assertion.AssertNotNull( "Syntax error", hdr );
        }

        public static void Main()
        {
            new TestParser().TestBasic();
        }
	}

    internal class Visitor : NodeVisitor
    {
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
            this.indent += 4;
            FileElementList iter = list;
            while( iter != null )
            {
                iter.Head.Accept( this );
                iter = iter.Tail;
            }
            this.indent -= 4;
        }

        /// <summary>
        /// Visits a namespace
        /// </summary>
        /// <param name="ns"></param>
        public override void Visit( NamespaceFileElement ns )
        {
            Console.Write( new String( ' ', this.indent ) );
            Console.WriteLine( "namespace " + ns.Namespace.Name );

            ns.Namespace.Elements.Accept( this );
        }

        /// <summary>
        /// Visits a class element
        /// </summary>
        /// <param name="cl"></param>
        public override void Visit( ClassFileElement cl )
        {
            Console.Write( new String( ' ', this.indent ) );
            Console.Write( cl.Class.ClassDecl.Visibility.Visibility + " class " + cl.Class.ClassDecl.Name );
            if ( cl.Class.DocComment != null )
                Console.WriteLine( " with docstring " + cl.Class.DocComment.Comment );
            else
                Console.WriteLine( "" );

            cl.Class.ClassDecl.Members.Accept( this );

        }

        /// <summary>
        /// Visits a list of class members
        /// </summary>
        /// <param name="m"></param>
        public override void Visit( MemberList m )
        {
            this.indent += 4;
            MemberList iter = m;
            while( iter != null )
            {
                iter.Head.Accept( this );
                iter = iter.Tail;
            }
            this.indent -= 4;
        }

        /// <summary>
        /// Visits a method declaration
        /// </summary>
        /// <param name="item"></param>
        public override void Visit( MethodDeclItem item )
        {
            Console.Write( new String( ' ', this.indent ) );
            Console.Write( "method " + item.MethodDecl.ReturnType + " " + item.MethodDecl.Name );
            if ( item.DocComment != null )
                Console.WriteLine( " with doccomment: " + item.DocComment.Comment );
            else
                Console.WriteLine( "" );                
        }

        private int indent = 0;
   
    }
}
