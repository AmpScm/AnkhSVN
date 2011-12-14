using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;

namespace Ankh.UI.SvnInfoGrid.Commands
{
    [Command(AnkhCommand.SvnInfoComboBox, AlwaysAvailable=true)]
    [Command(AnkhCommand.SvnInfoComboBoxFill, AlwaysAvailable = true)]
    class SvnInfoComboCommand : ICommandHandler
    {
        SvnInfoGridControl _control;
        public virtual void OnUpdate(CommandUpdateEventArgs e)
        {
            if (_control == null)
            {
                _control = e.GetService<SvnInfoGridControl>();

                if (_control == null)
                {
                    e.Enabled = false;
                    return;
                }
            }

            GC.KeepAlive(GetNames(e));
        }

        IEnumerable<SvnItem> _selection;
        string _cached;
        string[] _names;

        public string[] GetNames(BaseCommandEventArgs e)
        {
            IEnumerable<SvnItem> sel = e.Selection.GetSelectedSvnItems(false);

            if (!ReferenceEquals(sel, _selection))
            {
                List<SvnItem> full = new List<SvnItem>();
                full.AddRange(sel);

                string[] names = new string[full.Count];
                for (int i = 0; i < names.Length; i++)
                    names[i] = full[i].Name;

                _selection = sel;
                _names = names;

                if (_names.Length == 1)
                {
                    _cached = _names[0];

                    if (_control != null)
                        _control.Grid.SelectedObject = new Ankh.Scc.SvnItemData(e.Context, full[0]);
                }
                else
                {
                    _cached = null;
                    List<Ankh.Scc.SvnItemData> items = new List<Ankh.Scc.SvnItemData>();
                    for (int i = 0; i < full.Count; i++)
                        items.Add(new Ankh.Scc.SvnItemData(e.Context, full[i]));

                    _control.Grid.SelectedObjects = items.ToArray();
                }
            }

            return _names;
        }

        public virtual void OnExecute(CommandEventArgs e)
        {
            if (e.Command == AnkhCommand.SvnInfoComboBoxFill)
            {
                e.Result = GetNames(e);
                return;
            }

            if (e.Argument != null)
            {
                string value = e.Argument as string;

                if (value != null)
                {
                    // Selection changed
                    foreach (string s in _names)
                    {
                        if (s == value)
                        {
                            _cached = s;

                            // Update selection
                        }
                    }
                }
                else
                {
                    // Keyboard filter
                }
            }
            else
                e.Result = _cached;
        }
    }
}
