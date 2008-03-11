using System;
using System.Collections.Generic;
using System.Text;

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
        }

        /// <summary>
        /// Called when <see cref="AnkhRuntime.Start"/> is called
        /// </summary>
        public override void OnInitialize()
        {
            //throw new NotImplementedException();
        }
    }
}
