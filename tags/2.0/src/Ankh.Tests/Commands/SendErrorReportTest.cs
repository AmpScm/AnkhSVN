using System;
using NUnit.Framework;
using EnvDTE;
using Ankh.Commands;
using System.IO;
using AnkhSvn.Ids;

namespace Ankh.Tests.Commands
{
    /// <summary>
    /// Tests for the SendErrorReportCommand class.
    /// </summary>
    [TestFixture]
    public class SendErrorReportTest : NSvn.Core.Tests.TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.context = new ErrorContextBase();
            TestUtils.ToggleAnkh( false, "7.1" );
        }

        /// <summary>
        /// Test the QueryStatus method. Just make sure the command is always enabled.
        /// </summary>
        [Test]
        public void TestQueryStatus()
        {
            SendErrorReportCommand cmd = new SendErrorReportCommand();
            /*Assert.AreEqual( vsCommandStatus.vsCommandStatusEnabled | 
                             vsCommandStatus.vsCommandStatusSupported,
                cmd.QueryStatus( this.context ) );*/

        }

        /// <summary>
        /// Test the Execute method.
        /// </summary>
        [Test]
        public void TestExecute()
        {
            SendErrorReportCommand cmd = new SendErrorReportCommand();

            // just ensure the message is sent.
            cmd.OnExecute( new CommandEventArgs(AnkhCommand.SendFeedback, this.context));
            Assert.IsTrue( ((TestErrorHandler)this.context.ErrorHandler).Sent );
        }

        /// <summary>
        /// Verifies that all the attributes are present.
        /// </summary>
        [Test]
        public void TestAttributes()
        {
            SendErrorReportCommand cmd = new SendErrorReportCommand();
            Assert.IsTrue( 
                cmd.GetType().GetCustomAttributes( typeof(VSNetCommandAttribute), false ).Length == 1 );
            Assert.IsTrue( 
                cmd.GetType().GetCustomAttributes( typeof(VSNetCommandAttribute), false ).Length >= 1 );
        }

        private class ErrorContextBase : ContextBase
        {
            public ErrorContextBase()
            {
                this.errorHandler = new TestErrorHandler();
            }
        }

        private class TestErrorHandler : IAnkhErrorHandler
        {
            public bool Sent = false;

            #region IErrorHandler Members

            public void OnError(Exception ex)
            {
                // TODO:  Add TestErrorHandler.Handle implementation
            }

            public void SendReport()
            {
                this.Sent = true;
            }

            public void Write( string message, Exception ex, TextWriter writer )
            {
                // empty
            }

            #endregion

            #region IErrorHandler Members

            public void LogException( Exception exception, string message, params object[] args )
            {
                throw new Exception( "The method or operation is not implemented." );
            }

            #endregion
}


        
        private IContext context; 
    }
}
