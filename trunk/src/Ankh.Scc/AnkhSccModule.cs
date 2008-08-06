using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Ids;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Ankh.Commands;
using System.Reflection;

namespace Ankh.Scc
{
    public class AnkhSccModule : Module
    {
        public AnkhSccModule(AnkhRuntime runtime)
            : base(runtime)
        {

        }

        /// <summary>
        /// Called when added to the <see cref="AnkhRuntime"/>
        /// </summary>
        public override void OnPreInitialize()
        {
            Assembly thisAssembly = typeof(AnkhSccModule).Assembly;

            Runtime.CommandMapper.LoadFrom(thisAssembly);

            Runtime.LoadServices(Container, thisAssembly, Context);
        }

        /// <summary>
        /// Called when <see cref="AnkhRuntime.Start"/> is called
        /// </summary>
        public override void OnInitialize()
        {
            EnsureService<IStatusImageMapper>();
            EnsureService<IFileStatusCache>();
        }
    }
}
