using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ankh.WpfPackage
{
    sealed class AnkhWpfModule : Module
    {
        public AnkhWpfModule(AnkhRuntime runtime)
            : base(runtime)
        {
        }

        public override void OnPreInitialize()
        {
            Assembly thisAssembly = typeof(AnkhWpfModule).Assembly;

            //Runtime.CommandMapper.LoadFrom(thisAssembly);

            Runtime.LoadServices(Container, thisAssembly, Context);
        }

        public override void OnInitialize()
        {
            
        }
    }
}
