using System;
using NUnit.Framework;
using System.IO;

namespace NSvn.Tests
{
	/// <summary>
	/// Tests the Utils class
	/// </summary>
	public class UtilsTest : TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            this.ExtractWorkingCopy();
        }

        public void TestGetWorkingCopyRootedPath()
        {
            Console.WriteLine( Directory.GetCurrentDirectory() );

            string potential = Path.Combine( this.WcPath, @"bin\debug" );
            string root = Utils.GetWorkingCopyRootedPath( potential );
            Assertion.AssertEquals( "Rooted path not unwrong", @"\bin\debug", root );
 
            potential = Path.Combine( this.WcPath, "Form.cs" );
            root = Utils.GetWorkingCopyRootedPath( potential );
            Assertion.AssertEquals( "Rooted path not unwrong", @"\Form.cs", root );
        }
	}
}
