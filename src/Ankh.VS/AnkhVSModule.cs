using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.UI;
using Ankh.VS.Dialogs;
using Ankh.VS.SolutionExplorer;
using Ankh.VS.WebBrowser;
using Ankh.ContextServices;
using Ankh.Commands;
using Ankh.VS.Selection;
using Ankh.VS.Extenders;

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

            Container.AddService(typeof(IAnkhDialogOwner), new AnkhDialogOwner(Context));
            Container.AddService(typeof(IAnkhWebBrowser), new AnkhWebBrowser(Context));
            Container.AddService(typeof(IStatusImageMapper), new StatusImageMapper(Context));
            Container.AddService(typeof(IFileIconMapper), new FileIconMapper(Context));
            Container.AddService(typeof(IAnkhVSColor), new AnkhVSColor(Context));
            Container.AddService(typeof(IAnkhCommandStates), new CommandState(Context));
            Container.AddService(typeof(IAnkhTempFileManager), new TempFileManager(Context));

            Container.AddService(typeof(AnkhExtenderProvider), new AnkhExtenderProvider(Context));
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
