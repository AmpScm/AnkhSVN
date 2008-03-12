using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.PendingChanges;

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

            // Instantiate the logmessage language service
            LogMessageLanguageService ls = new LogMessageLanguageService();
            Container.AddService(typeof(LogMessageLanguageService), ls);
            ls.SetSite(Container);
        }

        public override void OnInitialize()
        {
            
        }
    }
}
