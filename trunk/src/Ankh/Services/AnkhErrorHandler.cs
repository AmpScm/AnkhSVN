// $Id$
//
// Copyright 2003-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Serialization;

using SharpSvn;

using Ankh.UI;
using Ankh.VS;
using Ankh.Commands;
using System.Text;
using System.Runtime.InteropServices;

namespace Ankh.Services
{
    /// <summary>
    /// Encapsulates error handling functionality.
    /// </summary>
    [GlobalService(typeof(IAnkhErrorHandler), AllowPreRegistered = true)]
    class AnkhErrorHandler : AnkhService, IAnkhErrorHandler
    {
        const string _errorReportMailAddress = "error@ankhsvn.tigris.org";
        const string _errorReportSubject = "Exception";
        readonly HandlerDelegator Handler;

        public AnkhErrorHandler(IAnkhServiceProvider context)
            : base(context)
        {
            Handler = new HandlerDelegator(this);
        }

        public bool IsEnabled(Exception ex)
        {
#if DEBUG
            return false;
#else
            return true;
#endif
        }

        /// <summary>
        /// Handles an exception.
        /// </summary>
        /// <param name="ex"></param>
        public void OnError(Exception ex)
        {
            if (ex == null)
                return;

            Handler.Invoke(ex, null);
        }

        public void OnError(Exception ex, BaseCommandEventArgs commandArgs)
        {
            if (ex == null)
                return;
            else if (commandArgs == null)
                OnError(ex);
            else
                Handler.Invoke(ex, new ExceptionInfo(commandArgs));
        }

        /// <summary>
        /// Send a non-specific report.
        /// </summary>
        public void SendReport()
        {
            System.Collections.Specialized.StringDictionary dict = new
                System.Collections.Specialized.StringDictionary();

            AnkhErrorMessage.SendByMail(_errorReportMailAddress, _errorReportSubject, null,
                typeof(AnkhErrorHandler).Assembly, dict);
        }

        sealed class ExceptionInfo
        {
            readonly BaseCommandEventArgs _commandArgs;
            public ExceptionInfo(BaseCommandEventArgs e)
            {
                _commandArgs = e;
            }

            public BaseCommandEventArgs CommandArgs
            {
                get { return _commandArgs; }
            }
        }

        sealed class HandlerDelegator : AnkhService
        {
            AnkhErrorHandler _handler;
            public HandlerDelegator(AnkhErrorHandler context)
                : base(context)
            {
                _handler = context;
            }

            IWin32Window Owner
            {
                get { return GetService<IUIService>().GetDialogOwnerWindow(); }
            }

            public void Invoke(Exception ex, ExceptionInfo info)
            {
                try
                {
                    // BH: Uses reflection to find the best match based on the exception??

                    Type t = typeof(HandlerDelegator);
                    MethodInfo method = t.GetMethod("DoHandle", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { ex.GetType(), typeof(ExceptionInfo) }, null);

                    if (method != null)
                        method.Invoke(this, new object[] { ex, info });
                    else
                        DoHandle(ex, info);
                }
                catch (Exception x)
                {
                    Debug.WriteLine(x);
                }
            }


            private void DoHandle(ProgressRunnerException ex, ExceptionInfo info)
            {
                // we're only interested in the inner exception - we know where the 
                // outer one comes from
                Invoke(ex.InnerException, info);
            }

            private void DoHandle(SvnRepositoryHookException e, ExceptionInfo info)
            {
                string message;
                if (e.InnerException != null)
                    message = GetNestedMessages(e).Replace("\r", "").Replace("\n", Environment.NewLine);
                else
                    message = e.Message;

                MessageBox.Show(Owner, message, "Repository hook failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }


            private void DoHandle(SvnWorkingCopyLockException ex, ExceptionInfo info)
            {
                MessageBox.Show(Owner,
                    "Your working copy appears to be locked. " + Environment.NewLine +
                    "Run Cleanup to amend the situation.",
                    "Working copy locked", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            private void DoHandle(SvnAuthorizationException ex, ExceptionInfo info)
            {
                // TODO: Show at least some parts of the real error to help resolve it.
                MessageBox.Show(Owner,
                    "You failed to authorize against the remote repository. ",
                    "Authorization failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            private void DoHandle(SvnAuthenticationException ex, ExceptionInfo info)
            {
                MessageBox.Show(Owner,
                    "You failed to authenticate against the remote repository. ",
                    "Authentication failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            private void DoHandle(SvnFileSystemOutOfDateException ex, ExceptionInfo info)
            {
                MessageBox.Show(Owner,
                    "One or more of your local resources are out of date. " +
                    "You need to run Update before you can proceed with the operation",
                    "Resource(s) out of date", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            private void DoHandle(Exception ex, ExceptionInfo info)
            {
                _handler.ShowErrorDialog(ex, true, true, info);
            }
        }

        private void ShowErrorDialog(Exception ex, bool showStackTrace, bool internalError, ExceptionInfo info)
        {
            string stackTrace = GetNestedStackTraces(ex);
            string message = GetNestedMessages(ex);
            System.Collections.Specialized.StringDictionary additionalInfo =
                new System.Collections.Specialized.StringDictionary();


            IAnkhSolutionSettings ss = GetService<IAnkhSolutionSettings>();
            if (ss != null)
                additionalInfo.Add("VS-Version", ss.VisualStudioVersion.ToString());

            if (info != null && info.CommandArgs != null)
                additionalInfo.Add("Command", info.CommandArgs.Command.ToString());

            IAnkhPackage pkg = GetService<IAnkhPackage>();
            if (pkg != null)
                additionalInfo.Add("Ankh-Version", pkg.UIVersion.ToString());

            additionalInfo.Add("SharpSvn-Version", SharpSvn.SvnClient.SharpSvnVersion.ToString());
            additionalInfo.Add("Svn-Version", SharpSvn.SvnClient.Version.ToString());
            additionalInfo.Add("OS-Version", Environment.OSVersion.Version.ToString());

            using (ErrorDialog dlg = new ErrorDialog())
            {
                dlg.ErrorMessage = message;
                dlg.ShowStackTrace = showStackTrace;
                dlg.StackTrace = stackTrace;
                dlg.InternalError = internalError;

                if (dlg.ShowDialog(Context) == DialogResult.Retry)
                {
                    string subject = _errorReportSubject;

                    SvnException sx = ex as SvnException;

                    Exception e = ex;
                    while (sx == null && e != null)
                    {
                        e = e.InnerException;
                        sx = e as SvnException;
                    }

                    if (sx != null)
                    {
                        SvnException rc = sx.RootCause as SvnException;
                        if (rc == null || rc.SvnErrorCode == sx.SvnErrorCode)
                            subject += " (" + ErrorToString(sx) + ")";
                        else
                            subject += " (" + ErrorToString(sx) + "-" + ErrorToString(rc) + ")";
                    }

                    AnkhErrorMessage.SendByMail(_errorReportMailAddress,
                        subject, ex, typeof(AnkhErrorHandler).Assembly, additionalInfo);
                }
            }
        }

        static string ErrorToString(SvnException ex)
        {
            if (Enum.IsDefined(typeof(SvnErrorCode), ex.SvnErrorCode))
                return ex.SvnErrorCode.ToString();
            else if (ex.SvnErrorCategory == SvnErrorCategory.OperatingSystem)
            {
                // 
                int num = ex.OperatingSystemErrorCode;

                if ((num & 0x80000000) != 0x80000000)
                {
                    num = unchecked((int)(((uint)num & 0xFFFF) | 0x80070000));
                }

                Exception sysEx = Marshal.GetExceptionForHR(num);

                if (sysEx != null)
                    return "OS:" + sysEx.GetType().Name;
            }
            else if (Enum.IsDefined(typeof(SvnErrorCategory), ex.SvnErrorCategory))
                return string.Format("{0}:{1}", ex.SvnErrorCategory, ex.SvnErrorCode);

            return ((int)ex.SvnErrorCode).ToString();
        }

        private static string GetNestedStackTraces(Exception ex)
        {
            return ex.ToString();
        }

        private static string GetNestedMessages(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            while (ex != null)
            {
                sb.AppendLine(ex.Message.Trim());
                ex = ex.InnerException;
            }

            return sb.ToString();
        }
    }
}
