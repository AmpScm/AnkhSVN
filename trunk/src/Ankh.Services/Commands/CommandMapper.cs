﻿using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;

namespace Ankh.Commands
{
    public sealed class CommandMapper : AnkhService
    {
        readonly Dictionary<AnkhCommand, CommandMapItem> _map;
        readonly AnkhCommandContext _commandContext;
        

        public CommandMapper(IAnkhServiceProvider context)
            : base(context)
        {
            _map = new Dictionary<AnkhCommand, CommandMapItem>();
        }
        public CommandMapper(IAnkhServiceProvider context, AnkhCommandContext commandContext)
            : this(context)
        {
            _commandContext = commandContext;
        }

        public bool PerformUpdate(AnkhCommand command, CommandUpdateEventArgs e)
        {
            EnsureLoaded();
            CommandMapItem item;

            if (_map.TryGetValue(command, out item))
            {
                if (!item.AlwaysAvailable && !e.State.SccProviderActive)
                    e.Enabled = false; 
                else
                    try
                    {
                        item.OnUpdate(e);
                    }
                    catch (Exception ex)
                    {
                        IAnkhErrorHandler handler = Context.GetService<IAnkhErrorHandler>();

                        if (handler != null)
                        {
                            handler.OnError(ex);
                            return false;
                        }

                        throw;
                    }

                if (item.HideWhenDisabled && !e.Enabled)
                    e.Visible = false;

                return item.IsHandled;
            }
            else if (_defined.Contains(command))
            {
                e.Enabled = e.Visible = false;
                return true;
            }

            return false;
        }

        public bool Execute(AnkhCommand command, CommandEventArgs e)
        {
            EnsureLoaded();
            CommandMapItem item;

            if (_map.TryGetValue(command, out item))
            {
                try
                {
                    CommandUpdateEventArgs u = new CommandUpdateEventArgs(command, e.Context);
                    item.OnUpdate(u);
                    if (u.Enabled)
                    {
                        item.OnExecute(e);
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    IAnkhErrorHandler handler = Context.GetService<IAnkhErrorHandler>();

                    if (handler != null)
                    {
                        handler.OnError(ex);
                        return true; // If we return false VS shows another error box!
                    }

                    throw;

                }

                return item.IsHandled;
            }            

            return false;
        }

        public bool TryGetParameterList(AnkhCommand command, out string definition)
        {
            EnsureLoaded();

            CommandMapItem item;

            if (_map.TryGetValue(command, out item))
            {
                definition = item.ArgumentDefinition;

                return !string.IsNullOrEmpty(definition);
            }
            else
                definition = null;

            return false;
        }

        /// <summary>
        /// Gets the <see cref="CommandMapItem"/> for the specified command
        /// </summary>
        /// <param name="command"></param>
        /// <returns>The <see cref="CommandMapItem"/> or null if the command is not valid</returns>
        public CommandMapItem this[AnkhCommand command]
        {
            get
            {
                CommandMapItem item;

                if (_map.TryGetValue(command, out item))
                    return item;
                else
                {
                    item = new CommandMapItem(command);

                    _map.Add(command, item);

                    return item;
                }
            }
        }

        readonly List<Assembly> _assembliesToLoad = new List<Assembly>();
        readonly List<Assembly> _assembliesLoaded = new List<Assembly>();
        readonly HybridCollection<AnkhCommand> _defined = new HybridCollection<AnkhCommand>();

        public void LoadFrom(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            if (!_assembliesToLoad.Contains(assembly) && !_assembliesLoaded.Contains(assembly))
                _assembliesToLoad.Add(assembly);
        }

        private void EnsureLoaded()
        {            
            if(_assembliesToLoad.Count == 0)
                return;

            if (_defined.Count == 0)
            {
                foreach (AnkhCommand cmd in Enum.GetValues(typeof(AnkhCommand)))
                {
                    if (cmd <= AnkhCommand.CommandFirst)
                        continue;

                    _defined.Add(cmd);
                }                
            }

            while (_assembliesToLoad.Count > 0)
            {
                Assembly asm = _assembliesToLoad[0];
                _assembliesToLoad.RemoveAt(0);
                _assembliesLoaded.Add(asm);
                foreach (Type type in asm.GetTypes())
                {
                    if (!type.IsClass || type.IsAbstract)
                        continue;

                    if (!typeof(ICommandHandler).IsAssignableFrom(type))
                        continue;

                    ICommandHandler instance = null;

                    foreach (CommandAttribute cmdAttr in type.GetCustomAttributes(typeof(CommandAttribute), false))
                    {
                        if (cmdAttr.Context != _commandContext)
                            continue;

                        CommandMapItem item = this[cmdAttr.Command];

                        if (item != null)
                        {
                            if (instance == null)
                            {
                                instance = (ICommandHandler)Activator.CreateInstance(type);

                                IComponent component = instance as IComponent;

                                if (component != null)
                                    component.Site = CommandSite;
                            }

                            Debug.Assert(item.ICommand == null || item.ICommand == instance, string.Format("No previous ICommand registered on the CommandMapItem for {0}", cmdAttr.Command));

                            item.ICommand = instance; // hooks all events in compatibility mode
                            item.AlwaysAvailable = cmdAttr.AlwaysAvailable;
                            item.HideWhenDisabled = cmdAttr.HideWhenDisabled;
                            item.ArgumentDefinition = cmdAttr.ArgumentDefinition;
                        }
                    }
                }
            }
        }

        CommandMapperSite _commandMapperSite;
        CommandMapperSite CommandSite
        {
            get { return _commandMapperSite ?? (_commandMapperSite = new CommandMapperSite(this)); }
        }

        sealed class CommandMapperSite : AnkhService, ISite
        {
            readonly CommandMapper _mapper;
            readonly Container _container = new Container();

            public CommandMapperSite(CommandMapper context)
                : base(context)
            {
                _mapper = context;
            }

            public IComponent Component
            {
                get { return _mapper; }
            }

            public IContainer Container
            {
                get { return _container; }
            }

            public bool DesignMode
            {
                get { return false; }
            }

            public string Name
            {
                get { return "CommandMapper"; }
                set { throw new InvalidOperationException();  }
            }            
        }

        [CLSCompliant(false)]
        public int QueryStatus(AnkhContext context, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (cCmds != 1 || prgCmds == null)
                return -1;

            TextQueryType textQuery = TextQueryType.None;
            string oldText = null;

            if (pCmdText != IntPtr.Zero)
            {
                // VS Want's some text from us for either the statusbar or the command text
                OLECMDTEXTF textType = GetFlags(pCmdText);

                switch (textType)
                {
                    case OLECMDTEXTF.OLECMDTEXTF_NAME:
                        textQuery = TextQueryType.Name;
                        break;
                    case OLECMDTEXTF.OLECMDTEXTF_STATUS:
                        textQuery = TextQueryType.Status;
                        break;
                }

                oldText = GetText(pCmdText);
            }

            CommandUpdateEventArgs updateArgs = new CommandUpdateEventArgs((AnkhCommand)prgCmds[0].cmdID, context, textQuery, oldText);

            OLECMDF cmdf = OLECMDF.OLECMDF_SUPPORTED;

            if (PerformUpdate(updateArgs.Command, updateArgs))
            {
                if (updateArgs.Enabled)
                    cmdf |= OLECMDF.OLECMDF_ENABLED;

                if (updateArgs.Latched)
                    cmdf |= OLECMDF.OLECMDF_LATCHED;

                if (updateArgs.Ninched)
                    cmdf |= OLECMDF.OLECMDF_NINCHED;

                if (!updateArgs.Visible)
                    cmdf |= OLECMDF.OLECMDF_INVISIBLE;

                if (updateArgs.DynamicMenuEnd)
                    return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
            }

            if (textQuery != TextQueryType.None)
            {
                SetText(pCmdText, updateArgs.Text ?? updateArgs.Command.ToString());
            }

            prgCmds[0].cmdf = (uint)cmdf;

            return 0; // S_OK
        }

        #region // Interop code from: VS2008SDK\VisualStudioIntegration\Common\Source\CSharp\Project\Misc\NativeMethods.cs

        /// <summary>
        /// Gets the flags of the OLECMDTEXT structure
        /// </summary>
        /// <param name="pCmdTextInt">The structure to read.</param>
        /// <returns>The value of the flags.</returns>
        static OLECMDTEXTF GetFlags(IntPtr pCmdTextInt)
        {
            Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT pCmdText = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT)Marshal.PtrToStructure(pCmdTextInt, typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));

            if ((pCmdText.cmdtextf & (int)OLECMDTEXTF.OLECMDTEXTF_NAME) != 0)
                return OLECMDTEXTF.OLECMDTEXTF_NAME;

            if ((pCmdText.cmdtextf & (int)OLECMDTEXTF.OLECMDTEXTF_STATUS) != 0)
                return OLECMDTEXTF.OLECMDTEXTF_STATUS;

            return OLECMDTEXTF.OLECMDTEXTF_NONE;
        }

        /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="OLECMDTEXTF.SetText"]/*' />
        /// <devdoc>
        /// Accessing the text of this structure is very cumbersome.  Instead, you may
        /// use this method to access an integer pointer of the structure.
        /// Passing integer versions of this structure is needed because there is no
        /// way to tell the common language runtime that there is extra data at the end of the structure.
        /// </devdoc>
        /// <summary>
        /// Sets the text inside the structure starting from an integer pointer.
        /// </summary>
        /// <param name="pCmdTextInt">The integer pointer to the position where to set the text.</param>
        /// <param name="text">The text to set.</param>
        static void SetText(IntPtr pCmdTextInt, string text)
        {
            Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT pCmdText = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT)Marshal.PtrToStructure(pCmdTextInt, typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));
            char[] menuText = text.ToCharArray();

            // Get the offset to the rgsz param.  This is where we will stuff our text
            //
            IntPtr offset = Marshal.OffsetOf(typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "rgwz");
            IntPtr offsetToCwActual = Marshal.OffsetOf(typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "cwActual");

            // The max chars we copy is our string, or one less than the buffer size,
            // since we need a null at the end.
            //
            int maxChars = Math.Min((int)pCmdText.cwBuf - 1, menuText.Length);

            Marshal.Copy(menuText, 0, (IntPtr)((long)pCmdTextInt + (long)offset), maxChars);

            // append a null character
            Marshal.WriteInt16((IntPtr)((long)pCmdTextInt + (long)offset + maxChars * 2), 0);

            // write out the length
            // +1 for the null char
            Marshal.WriteInt32((IntPtr)((long)pCmdTextInt + (long)offsetToCwActual), maxChars + 1);
        }

        /// <devdoc>
        /// Accessing the text of this structure is very cumbersome.  Instead, you may
        /// use this method to access an integer pointer of the structure.
        /// Passing integer versions of this structure is needed because there is no
        /// way to tell the common language runtime that there is extra data at the end of the structure.
        /// </devdoc>
        static string GetText(IntPtr pCmdTextInt)
        {
            Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT pCmdText = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT)Marshal.PtrToStructure(pCmdTextInt, typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));

            // Get the offset to the rgsz param.
            //
            IntPtr offset = Marshal.OffsetOf(typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "rgwz");

            // Punt early if there is no text in the structure.
            //
            if (pCmdText.cwActual == 0)
            {
                return "";
            }

            char[] text = new char[pCmdText.cwActual - 1];

            Marshal.Copy((IntPtr)((long)pCmdTextInt + (long)offset), text, 0, text.Length);

            StringBuilder s = new StringBuilder(text.Length);
            s.Append(text);
            return s.ToString();
        }
        #endregion        
    }
}
