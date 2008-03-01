// $Id$
using System;
using EnvDTE;

using Ankh.UI;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;
using System.Diagnostics;

namespace Ankh.Commands
{
    /// <summary>
    /// Base class for ICommand instances
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        /// Get the status of the command
        /// </summary>
        //[Obsolete("Please implement Update(CommandUpdateEventArgs)")]
        public virtual vsCommandStatus QueryStatus(IContext context)
        {
            return (vsCommandStatus)999;
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        //[Obsolete("Please implement OnExecute(CommandEventArgs)")]
        public virtual void Execute(IContext context, string parameters)
        {
            throw new NotImplementedException();
        }

        public virtual void OnUpdate(CommandUpdateEventArgs e)
        {
            EnvDTE.vsCommandStatus status = QueryStatus(e.Context);

            if (status == (vsCommandStatus)999)
                return; // Not implemented value; see above

            if ((status & EnvDTE.vsCommandStatus.vsCommandStatusEnabled) == 0)
                e.Enabled = false;

            if ((status & EnvDTE.vsCommandStatus.vsCommandStatusLatched) != 0)
                e.Latched = true;

            if ((status & EnvDTE.vsCommandStatus.vsCommandStatusNinched) != 0)
                e.Ninched = true;

            if ((status & EnvDTE.vsCommandStatus.vsCommandStatusInvisible) != 0)
                e.Visible = false;
        }

        public virtual void OnExecute(CommandEventArgs e)
        {
            Execute(e.Context, e.Argument as string);
        }

        /// <summary>
        /// The EnvDTE.Command instance corresponding to this command.
        /// </summary>
        public EnvDTE.Command Command
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return this.command;
            }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                this.command = value;
            }
        }

        /// <summary>
        /// Gets whether the Shift key was down when the current window message was send
        /// </summary>
        public static bool Shift
        {
            get
            {
                return (0 != System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift);
            }
        }

        protected void SaveAllDirtyDocuments(IContext context)
        {
            context.DTE.ExecuteCommand("File.SaveAll", "");
        }

        protected const vsCommandStatus Enabled =
            vsCommandStatus.vsCommandStatusEnabled |
            vsCommandStatus.vsCommandStatusSupported;

        protected const vsCommandStatus Disabled =
            vsCommandStatus.vsCommandStatusSupported;

        protected static XslTransform GetTransform(IContext context, string name)
        {
            // is the file already there?
            string configDir = Path.GetDirectoryName(context.ConfigLoader.ConfigPath);
            string path = Path.Combine(configDir, name);

            if (!File.Exists(path))
                CreateTransformFile(path, name);

            Debug.Assert(File.Exists(path));

            XPathDocument doc = new XPathDocument(new StreamReader(path));

            // TODO: Transforms should be cached as a dynamic assembly is created
            // which stays in memory until the appdomain closes
            XslTransform transform = new XslTransform();
            transform.Load(doc);

            return transform;
        }

        protected static void CreateTransformFile(string path, string name)
        {
            // get the embedded resource and copy it to path
            string resourceName = "Ankh.Commands." + name;
            Stream ins =
                typeof(CommandBase).Assembly.GetManifestResourceStream(resourceName);
            int len;
            byte[] buffer = new byte[4096];
            using (FileStream outs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                while ((len = ins.Read(buffer, 0, 4096)) > 0)
                {
                    outs.Write(buffer, 0, len);
                }
            }
        }

        private EnvDTE.Command command;
    }
}