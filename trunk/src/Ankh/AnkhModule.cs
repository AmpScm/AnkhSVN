using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Ankh.Commands;

namespace Ankh
{
    public class AnkhModule : Module
    {
        public AnkhModule(AnkhRuntime runtime)
            : base(runtime)
        {
        }

        public override void OnPreInitialize()
        {
            Assembly thisAssembly = typeof(AnkhModule).Assembly;

            Runtime.CommandMapper.LoadFrom(thisAssembly);

            Runtime.LoadServices(Container, thisAssembly, Context);
        }

        public override void OnInitialize()
        {
            EnsureService<IAnkhErrorHandler>();
            EnsureService<IAnkhCommandService>();

            CheckForUpdates.MaybePerformUpdateCheck(Context);
        }
    }
}
