using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc;
using Ankh.UI;
using Ankh.ContextServices;
using Ankh.Commands;
using Ankh.Configuration;
using Ankh.StatusCache;
using Ankh.Selection;
using Ankh.VS;
using Ankh.Settings;
using Ankh.WorkingCopyExplorer;
using Ankh.Services;

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

            Container.AddService(typeof(IAnkhConfigurationService), new ConfigService(Context));
            Container.AddService(typeof(IAnkhCommandService), new AnkhCommandService(Context));            
            Container.AddService(typeof(IWorkingCopyOperations), new WorkingCopyOperations(Context));
            Container.AddService(typeof(ISvnClientPool), new AnkhSvnClientPool(Context));
            Container.AddService(typeof(IAnkhTaskManager), new ConflictManager(Context));
            Container.AddService(typeof(IAnkhScheduler), new AnkhScheduler(Context));
            Container.AddService(typeof(IUIShell), new UIShell(Context));
            Container.AddService(typeof(IAnkhSolutionSettings), new SolutionSettings(Context));
            Container.AddService(typeof(IProgressRunner), new ProgressRunnerService(Context));

            Container.AddService(typeof(IExplorersShell), new ExplorersShell(Context), true);

            // Ensure old context behaviour
            _context = GetService<IContext>();
            if (_context == null)
            {
                _context = new OldAnkhContext(GetService<IAnkhPackage>());
                Container.AddService(typeof(IContext), _context, true);
            }

            // TODO: Register services
            if (null == Container.GetService(typeof(IFileStatusCache)))
            {
                FileStatusCache fsc = new FileStatusCache(Context);
                Container.AddService(typeof(IFileStatusCache), fsc);
                Container.AddService(typeof(ISvnItemChange), fsc);
            }            

#if !DEBUG
            Container.AddService(typeof(IAnkhErrorHandler), new AnkhErrorHandler(Context));
#endif
        }

        public override void OnInitialize()
        {
            EnsureService<IAnkhErrorHandler>();
            EnsureService<IAnkhCommandService>();

            CheckForUpdates.MaybePerformUpdateCheck(Context);
        }
    }
}
