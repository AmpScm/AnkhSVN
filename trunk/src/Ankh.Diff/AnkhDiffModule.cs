using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Ankh.Diff
{
    public class AnkhDiffModule : Module
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhDiffModule"/> class.
        /// </summary>
        /// <param name="runtime">The runtime.</param>
        public AnkhDiffModule(AnkhRuntime runtime)
            : base(runtime)
        {
        }

        /// <summary>
        /// Called when added to the <see cref="AnkhRuntime"/>
        /// </summary>
        public override void OnPreInitialize()
        {
            Assembly thisAssembly = typeof(AnkhDiffModule).Assembly;

            Runtime.CommandMapper.LoadFrom(thisAssembly);

            Runtime.LoadServices(Container, thisAssembly, Context);
        }

        /// <summary>
        /// Called when <see cref="AnkhRuntime.Start"/> is called
        /// </summary>
        public override void OnInitialize()
        {
            
        }
    }
}
