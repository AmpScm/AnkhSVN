﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Ids;
using System.IO;
using Microsoft.VisualStudio;

namespace Ankh.VS.OutputPane
{
    [GlobalService(typeof(IOutputPaneManager))]
    class OutputPaneManager : AnkhService, IOutputPaneManager
    {
        IVsOutputWindow _window;
        Guid g = new Guid(AnkhId.AnkhOutputPaneId);

        public OutputPaneManager(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IVsOutputWindow Window
        {
            get { return _window ?? (_window = GetService<IVsOutputWindow>(typeof(SVsOutputWindow))); }
        }

        bool _created;
        void EnsureCreated()
        {
            if (_created)
                return;
            _created = true;

            ErrorHandler.ThrowOnFailure(Window.CreatePane(ref g, AnkhId.PlkProduct, 1, 0));
        }

        public void WriteToPane(string s)
        {
            EnsureCreated();

            IVsOutputWindowPane pane;
            ErrorHandler.ThrowOnFailure(Window.GetPane(ref g, out pane));
            
            ErrorHandler.ThrowOnFailure(pane.OutputString(s));
        }
    }
}
