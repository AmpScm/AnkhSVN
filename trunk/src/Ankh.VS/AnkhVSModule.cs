﻿using System;
using System.Collections.Generic;
using System.Text;
using Ankh.SolutionExplorer;
using Ankh.Selection;
using Ankh.Scc;
using Ankh.UI;
using Ankh.VS.Dialogs;
using Ankh.VS.WebBrowser;

namespace Ankh.VS
{
    /// <summary>
    /// 
    /// </summary>
    public class AnkhVSModule : Module
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhVSModule"/> class.
        /// </summary>
        /// <param name="runtime">The runtime.</param>
        public AnkhVSModule(AnkhRuntime runtime)
            : base(runtime)
        {
        }

        /// <summary>
        /// Called when added to the <see cref="AnkhRuntime"/>
        /// </summary>
        public override void OnPreInitialize()
        {
            Runtime.CommandMapper.LoadFrom(typeof(AnkhVSModule).Assembly);

            SolutionExplorerWindow window = new SolutionExplorerWindow(Context);

            Container.AddService(typeof(IAnkhSolutionExplorerWindow), window);

            SelectionContext selection = new SelectionContext(Context, window);
            Container.AddService(typeof(ISelectionContext), selection);
            Container.AddService(typeof(ISccProjectWalker), selection);

            Container.AddService(typeof(IAnkhWebBrowser), new AnkhWebBrowser(Context));
            Container.AddService(typeof(IDialogRunner), new VSDialogRunner(Context));
            Container.AddService(typeof(IStatusImageMapper), new StatusImageMapper(Context);
        }

        /// <summary>
        /// Called when <see cref="AnkhRuntime.Start"/> is called
        /// </summary>
        public override void OnInitialize()
        {
            EnsureService<IFileStatusCache>();
            EnsureService<IStatusImageMapper>();

            //throw new NotImplementedException();
        }
    }
}
