using NUnit.Framework;
using System.IO;
using System.Text;
using NSvn.Common;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests Client::ClientPropSet
    /// </summary>
    [TestFixture]
    public class PropSetTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractWorkingCopy();
        }

        /// <summary>
        ///Attempts to Set Properties on a file
        /// </summary>
        [Test]
        public void TestSetProp()
        {  
            string filePath = Path.Combine( this.WcPath, "Form.cs" );
             
            byte[] propval = Encoding.UTF8.GetBytes ( "baa" );
            Client.PropSet( new Property( "moo", propval ), filePath, false );
            Assertion.AssertEquals( "PropSet didn't work!", 
                "baa", this.RunCommand( "svn", "propget moo " + filePath ).Trim() );
        }
   
        /// <summary>
        ///Attempts to set Properties on a directory recursively. 
        /// </summary>
        [Test]
        public void TestSetPropRecursivly()
        {  
            string filePath = Path.Combine( this.WcPath, "Form.cs" );
            
            byte[] propval = Encoding.UTF8.GetBytes ( "baa" );
            Client.PropSet( new Property("moo", propval), this.WcPath, true );

            Assertion.AssertEquals( "PropSet didn't work on directory!", 
                "baa", this.RunCommand( "svn", "propget moo " + this.WcPath ).Trim() );

            Assertion.AssertEquals( "PropSet didn't work on file!", 
                "baa", this.RunCommand( "svn", "propget moo " + filePath ).Trim() );
        }
   
    }

}
