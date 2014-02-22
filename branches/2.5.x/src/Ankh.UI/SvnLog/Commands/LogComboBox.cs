using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Scc;
using Ankh.UI.SvnInfoGrid;

namespace Ankh.UI.SvnLog.Commands
{
    [SvnCommand(AnkhCommand.SvnLogComboBox, AlwaysAvailable = true)]
    [SvnCommand(AnkhCommand.SvnLogComboBoxFill, AlwaysAvailable = true)]
    sealed class LogComboBox : ICommandHandler
    {
        LogToolWindowControl _ctrl;

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (_ctrl == null)
                _ctrl = e.GetService<LogToolWindowControl>();

            if (_ctrl == null)
            {
                e.Enabled = e.Visible = false;
                return;
            }

            IList<SvnOrigin> origins = _ctrl.Origins;
            if (origins == null || origins.Count == 0)
            {
                e.Enabled = e.Visible = false;
                return;
            }

            // Enable something like this?
            /*if (e.Command == AnkhCommand.SvnLogComboBox
                && origins.Count == 1)
            {
                e.Enabled = false;
            }*/
        }

        object _lastOrigins;
        string[] _lastNames;
        string[] GetNames()
        {
            IList<SvnOrigin> origins = _ctrl.Origins;
            if (ReferenceEquals(_lastOrigins, origins))
                return _lastNames;

            List<string> names = new List<string>();

            foreach (SvnOrigin o in origins)
            {
                names.Add(o.Target.FileName);
            }

            _lastOrigins = origins;
            return _lastNames = names.ToArray();
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (e.Command == AnkhCommand.SvnLogComboBoxFill)
            {
                e.Result = GetNames();
                return;
            }

            IList<SvnOrigin> origins = _ctrl.Origins;

            if (origins == null || origins.Count == 0)
                return;

            if (e.Argument != null)
            {
                string selected = e.Argument as string;

                if (selected != null && origins.Count > 1)
                    foreach (SvnOrigin o in origins)
                    {
                        if (selected == o.Target.FileName)
                        {
                            _ctrl.StartLog(o, _ctrl.LastStartRevision, _ctrl.LastEndRevision);
                            return;
                        }
                    }
            }
            else if (origins.Count > 1)
            {
                Uri baseUri = _ctrl.CommonBaseUri;

                if (baseUri != null)
                    e.Result = string.Format(SvnInfoStrings.NrOfFilesBelow, origins.Count, baseUri);
                else
                    e.Result = string.Format(SvnInfoStrings.NrOfFileInfo, origins.Count);
            }
            else if (origins.Count == 1)
                e.Result = _ctrl.Origins[0].Target.FileName;
        }
    }
}
