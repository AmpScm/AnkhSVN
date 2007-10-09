using System;
using NUnit.Framework;
using System.IO;
using Tools;


namespace Rogue.DocParser
{
	[TestFixture]
	public class TestLexer
	{
        [Test] 
        [Ignore("Maybe later")]
        public void TestHeader1()
        {
            
            string[] names = new string[]{ "PLAINCOMMENT", "NAMESPACE", "IDENTIFIER", 
                                             "LBRACE", "DOCCOMMENT", "STARTTAG", "DOCCOMMENTTEXT", 
                                             "ENDTAG", "PUBLIC", 
                                             "CLASS", "IDENTIFIER", "COLON", "IDENTIFIER",
                                             "LBRACE", "RBRACE", "SEMICOLON", "RBRACE" };
            Lexer lexer = new tokens();
            lexer.Start( new StreamReader( 
                this.GetType().Assembly.GetManifestResourceStream( "Rogue.DocParser.Header1.txt") ) );
            foreach( string name in names )
            {
                TOKEN token = lexer.Next();
                Console.WriteLine( token.yyname() + " = '" +token.yytext + "'" );
                Assertion.AssertEquals( "Index: " + Array.IndexOf( names, name ), 
                    name, token.yyname() );                
            }
                       
        }

        public static void Main()
        {}
            

	}
}
