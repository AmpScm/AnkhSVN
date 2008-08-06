using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.PendingChanges;
using Ankh.UI.Services;
using Ankh.Scc;
using Ankh.UI.MergeWizard;
using System.Reflection;

namespace Ankh.UI
{
    public class AnkhUIModule : Module
    {
        public AnkhUIModule(AnkhRuntime runtime)
            : base(runtime)
        {

        }

        public override void OnPreInitialize()
        {
            Assembly thisAssembly = typeof(AnkhUIModule).Assembly;

            Runtime.CommandMapper.LoadFrom(thisAssembly);
            Runtime.LoadServices(Container, thisAssembly);
        }

        public override void OnInitialize()
        {
            
        }
    }
}
