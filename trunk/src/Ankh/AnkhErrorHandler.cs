using System;

using System.Windows.Forms;
using Utils;
using System.Reflection;
using System.Diagnostics;
using Ankh.UI;
using System.IO;
using System.Collections;
using Ankh.Configuration;
using System.Xml.Serialization;
using SharpSvn;
using Ankh.ContextServices;
using Ankh.Xml;
using System.Windows.Forms.Design;


namespace Ankh
{
    /// <summary>
    /// Encapsulates error handling functionality.
    /// </summary>
    class AnkhErrorHandler : AnkhService, IAnkhErrorHandler
    {
        public AnkhErrorHandler(IAnkhServiceProvider context)
            : base(context)
        {
        }

        /// <summary>
        /// Handles an exception.
        /// </summary>
        /// <param name="ex"></param>
        public void OnError(Exception ex)
        {
            try
            {
                // BH: Uses reflection to find the best match based on the exception??

                Type t = typeof(AnkhErrorHandler);
                MethodInfo method = t.GetMethod("DoHandle", new Type[] { ex.GetType() });

                if (method != null)
                    method.Invoke(this, new object[] { ex });
                else
                    DoHandle(ex);
            }
            catch (Exception x)
            {
                Debug.WriteLine(x);
            }
        }

        /// <summary>
        /// Send a non-specific report.
        /// </summary>
        public void SendReport()
        {
            System.Collections.Specialized.StringDictionary dict = new
                System.Collections.Specialized.StringDictionary();

            Utils.ErrorMessage.SendByMail(ErrorReportMailAddress, ErrrorReportSubject, null,
                typeof(AnkhErrorHandler).Assembly, dict);
        }

        public void Write(string message, Exception ex, TextWriter writer)
        {
            writer.WriteLine(message);
            string exceptionMessage = GetNestedMessages(ex);
            writer.WriteLine(exceptionMessage);
        }

        private void DoHandle(ProgressRunnerException ex)
        {
            // we're only interested in the inner exception - we know where the 
            // outer one comes from
            OnError(ex.InnerException);
        }

        private void DoHandle(SvnRepositoryHookException e)
        {
            string message;
            if (e.InnerException != null)
                message = string.Format("{1}{0}{0}{2}{0}", Environment.NewLine, e.Message, e.InnerException.Message);
            else
                message = e.Message;

            MessageBox.Show(message, "Repository hook failed", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }


        private void DoHandle(SvnWorkingCopyLockException ex)
        {
            MessageBox.Show("Your working copy appear to be locked. " + NL +
                "Run Cleanup to amend the situation.",
                "Working copy locked", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void DoHandle(SvnAuthorizationException ex)
        {
            MessageBox.Show(
                "You failed to authorize against the remote repository. ",
                "Authorization failed", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void DoHandle(SvnAuthenticationException ex)
        {
            MessageBox.Show(
                "You failed to authenticate against the remote repository. ",
                "Authorization failed", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void DoHandle(SvnFileSystemOutOfDateException ex)
        {
            MessageBox.Show(
                "One or more of your local resources are out of date. " +
                "You need to run Update before you can proceed with the operation",
                "Resource(s) out of date", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void DoHandle(SvnInvalidNodeKindException ex)
        {
            MessageBox.Show(
                "One or more of the resources selected are not valid targets for this operation" +
                Environment.NewLine +
                "(Are you trying to commit a child of a newly added, but not committed resource?)",
                "Illegal target for this operation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void DoHandle(SvnException ex)
        {
            if (ex.SubversionErrorCode == LockedFileErrorCode)
            {
                MessageBox.Show(
                    ex.Message + NL + NL +
                    "Avoid versioning files that can be locked by VS.NET. " +
                    "These include *.ncb, *.projdata etc." + NL +
                    "See the AnkhSVN FAQ for more details.",
                    "File exclusively locked",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                ShowErrorDialog(ex, false, false);
            }
        }



        private void DoHandle(Exception ex)
        {
            DoLogException(ex);
            ShowErrorDialog(ex, true, true);
        }

        public void LogException(Exception ex, string message, params object[] args)
        {
            IAnkhOperationLogger logger = Context.GetService<IAnkhOperationLogger>();

            if (logger != null)
            {
                logger.WriteLine(string.Format(message, args));
                logger.WriteLine(ex.ToString());
            }
        }

        private void DoLogException(Exception ex)
        {
            ErrorItems errorItems = this.LoadErrorItems();

            errorItems.Add(new ErrorItem(ex));

            errorItems.Serialize(this.ErrorFile);
        }


        private ErrorItems LoadErrorItems()
        {
            this.EnsureErrorLogFile();

            return ErrorItems.Deserialize(this.ErrorFile);
        }

        /// <summary>
        /// Make sure the error log file exists and contains a valid serialized object.
        /// </summary>
        /// <returns></returns>
        private void EnsureErrorLogFile()
        {
            if (File.Exists(this.ErrorFile))
            {
                return;
            }

            // Create an empty file containing no items.
            ErrorItems items = new ErrorItems();
            items.Serialize(this.ErrorFile);
        }

        private string ErrorFile
        {
            [DebuggerStepThrough]
            get { return Path.Combine(Path.GetTempPath(), "AnkhErrors.txt");/* this.context.ConfigLoader.ConfigDir, ErrorLogFile );*/ }
        }

        private void ShowErrorDialog(Exception ex, bool showStackTrace, bool internalError)
        {
            string stackTrace = GetNestedStackTraces(ex);
            string message = GetNestedMessages(ex);
            System.Collections.Specialized.StringDictionary additionalInfo =
                new System.Collections.Specialized.StringDictionary();

            using (ErrorDialog dlg = new ErrorDialog())
            {
                dlg.ErrorMessage = message;
                dlg.ShowStackTrace = showStackTrace;
                dlg.StackTrace = stackTrace;
                dlg.InternalError = internalError;

                IUIService ui = GetService<IUIService>();

                DialogResult dr;
                if (ui != null)
                    dr = ui.ShowDialog(dlg);
                else
                    dr = dlg.ShowDialog();

                if (dr == DialogResult.Retry)
                {
                    Utils.ErrorMessage.SendByMail(ErrorReportMailAddress,
                        ErrrorReportSubject, ex, typeof(AnkhErrorHandler).Assembly, additionalInfo);
                }
            }
        }

        private static string GetNestedStackTraces(Exception ex)
        {
            if (ex == null)
                return String.Empty;
            else
                return ex.StackTrace + NL + NL + GetNestedStackTraces(ex.InnerException);
        }

        private static string GetNestedMessages(Exception ex)
        {
            if (ex == null)
                return String.Empty;
            else
                return ex.Message + NL + NL + GetNestedMessages(ex.InnerException);
        }

        private static readonly string NL = Environment.NewLine;
        private const int LockedFileErrorCode = 720032;
        private const string ErrorReportUrl = "http://ankhsvn.com/error/report.aspx";
        private const string ErrorReportMailAddress = "error@ankhsvn.tigris.org";
        private const string ErrrorReportSubject = "Exception";
        private const string ErrorLogFile = "errors.xml";
    }

    namespace Xml
    {
        /// <summary>
        /// Must be public for the sake of the XmlSerializer
        /// </summary>
        public class ErrorItem
        {
            public ErrorItem(Exception ex)
            {
                this.Message = ex.Message;
                this.StackTrace = ex.StackTrace;
                if (this.InnerException != null)
                {
                    this.InnerException = new ErrorItem(ex.InnerException);
                }
                this.Source = ex.Source;
                this.Time = DateTime.Now;
            }

            public ErrorItem()
            {
            }

            public string Message;
            public string StackTrace;
            public string Source;
            public ErrorItem InnerException;
            public DateTime Time;

        }

        /// <summary>
        /// Must be public for the sake of the XmlSerializer
        /// </summary>
        public class ErrorItems
        {
            public ErrorItems(ErrorItem[] items)
            {
                this.Items = items;
            }

            public ErrorItems()
            {
                this.Items = new ErrorItem[0];
            }

            public void Add(ErrorItem item)
            {
                ErrorItem[] items = new ErrorItem[this.Items.Length + 1];
                this.Items.CopyTo(items, 0);
                items[this.Items.Length] = item;

                this.Items = items;
            }

            public void Serialize(string errorFile)
            {
                using (StreamWriter writer = new StreamWriter(errorFile))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ErrorItems));
                    serializer.Serialize(writer, this);
                }
            }

            public ErrorItem[] Items;

            public static ErrorItems Deserialize(string errorFile)
            {
                using (StreamReader reader = new StreamReader(errorFile))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ErrorItems));
                    return (ErrorItems)serializer.Deserialize(reader);
                }
            }
        }
    }
}
