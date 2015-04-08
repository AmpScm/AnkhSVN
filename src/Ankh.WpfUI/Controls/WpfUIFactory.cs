using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Ankh.Collections;
using Ankh.Commands;
using Ankh.Scc;
using Ankh.UI;

namespace Ankh.WpfUI.Controls
{
    [GlobalService(typeof(IPendingChangeControlFactory))]
    class WpfUiFactory : AnkhService, IPendingChangeControlFactory
    {
        public WpfUiFactory(IAnkhServiceProvider context)
            : base(context)
        {

        }
        public IPendingChangeControl Create(IAnkhServiceProvider context, IContainer container)
        {
            ElementHost host = new ElementHost();

            PendingChangesUserControl puc = new PendingChangesUserControl();

            puc.Resources.MergedDictionaries.RemoveAt(VSVersion.VS2012OrLater ? 0 : 1);

            puc.Context = context;

            host.Child = puc;

            return new PendingChangeControlWrapper(context, host, puc);
        }
    }

}
