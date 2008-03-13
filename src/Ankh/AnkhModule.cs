using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc;
using Ankh.UI;
using Ankh.Extenders;
using Ankh.ContextServices;
using Utils.Services;

namespace Ankh
{
    public class AnkhModule : Module
    {
        IContext _context;
        public AnkhModule(AnkhRuntime runtime)
            : base(runtime)
        {
        }

        public override void OnPreInitialize()
        {
            Runtime.CommandMapper.LoadFrom(typeof(AnkhModule).Assembly);

            Container.AddService(typeof(IAnkhDialogOwner), new AnkhDialogOwner(Context), true);

            Container.AddService(typeof(IWorkingCopyOperations), new WorkingCopyOperations(Context), true);

            Container.AddService(typeof(ISvnClientPool), new AnkhSvnClientPool(Context), true);

            // Ensure old context behaviour
            _context = GetService<IContext>();
            if (_context == null)
            {
                _context = new OldAnkhContext(GetService<IAnkhPackage>());
                Container.AddService(typeof(IContext), _context, true);
            }

            // TODO: Register services
            if(null == Container.GetService(typeof(IFileStatusCache)))
                Container.AddService(typeof(IFileStatusCache), new StatusCache(Context));
            Container.AddService(typeof(IStatusImageMapper), new StatusImages.TempStatusImageMapper());
            Container.AddService(typeof(AnkhExtenderProvider), new AnkhExtenderProvider(Context));


#if !DEBUG
            Container.AddService(typeof(IAnkhErrorHandler), new ErrorHandler(Context));
#endif
        }

        public override void OnInitialize()
        {
            EnsureService<IAnkhErrorHandler>();
            
            //throw new NotImplementedException();
        }
    }
}
