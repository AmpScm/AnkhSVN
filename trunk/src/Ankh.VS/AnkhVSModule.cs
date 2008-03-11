using System;
using System.Collections.Generic;
using System.Text;
using Ankh.SolutionExplorer;
using Ankh.Selection;
using Ankh.Scc;

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

            SolutionExplorerWindow window = new SolutionExplorerWindow(this);

            Container.AddService(typeof(IAnkhSolutionExplorerWindow), window, true);
            Container.AddService(typeof(ISelectionContext), new SelectionContext(this, window), true);
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
