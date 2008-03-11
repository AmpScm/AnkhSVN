using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using AnkhSvn.Ids;

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
            Runtime.CommandMapper.LoadFrom(typeof(AnkhSccModule).Assembly);
            
            AnkhSccProvider service = new AnkhSccProvider(Context);
            Container.AddService(typeof(AnkhSccProvider), service, true);
            Container.AddService(typeof(IAnkhSccService), service, true);
            Container.AddService(typeof(IAnkhProjectDocumentTracker), new ProjectDocumentTracker(Context));

            IVsRegisterScciProvider rscp = (IVsRegisterScciProvider)GetService(typeof(IVsRegisterScciProvider));
            if (rscp != null)
            {
                rscp.RegisterSourceControlProvider(AnkhId.SccProviderGuid);
            }
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
