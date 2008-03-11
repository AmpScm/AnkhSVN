using System;
using System.Collections.Generic;
using System.Text;

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
            Runtime.CommandMapper.LoadFrom(typeof(AnkhUIModule).Assembly);

            // TODO: Provide services
        }

        public override void OnInitialize()
        {
            
        }
    }
}
