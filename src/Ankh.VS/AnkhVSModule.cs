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
using Ankh.Ids;
using System.Reflection;

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
            Assembly thisAssembly = typeof(AnkhVSModule).Assembly;

            Runtime.CommandMapper.LoadFrom(thisAssembly);

            Runtime.LoadServices(Container, thisAssembly, Context);

        }

        /// <summary>
        /// Called when <see cref="AnkhRuntime.Start"/> is called
        /// </summary>
        public override void OnInitialize()
        {
            EnsureService<IFileStatusCache>();
            EnsureService<IStatusImageMapper>();
        }
    }
}
