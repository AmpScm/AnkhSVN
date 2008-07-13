﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Ids;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Ankh.Commands;

namespace Ankh.Scc
{
    public class AnkhSccModule : Module
    {
        AnkhSccProvider _sccProvider;
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

            AnkhSccProvider service = _sccProvider = new AnkhSccProvider(Context);

            Container.AddService(typeof(AnkhSccProvider), service, true);

            Container.AddService(typeof(IAnkhSccService), service);
            Container.AddService(typeof(IProjectFileMapper), service);
            Container.AddService(typeof(IAnkhProjectDocumentTracker), new ProjectTracker(Context));
            Container.AddService(typeof(IAnkhOpenDocumentTracker), new OpenDocumentTracker(Context));
            Container.AddService(typeof(IPendingChangesManager), new PendingChangeManager(Context));

            ProjectNotifier notifier = new ProjectNotifier(this);
            Container.AddService(typeof(IProjectNotifier), notifier);
            Container.AddService(typeof(IFileStatusMonitor), notifier);

            // We declare the Scc provider as a delayed create service to allow delayed registration as primary scc
            Container.AddService(typeof(ITheAnkhSvnSccProvider), _sccProvider, true);            
        }

        /// <summary>
        /// Called when <see cref="AnkhRuntime.Start"/> is called
        /// </summary>
        public override void OnInitialize()
        {
            EnsureService<IStatusImageMapper>();
            EnsureService<IFileStatusCache>();

            _sccProvider.TryRegisterSccProvider();
        }
    }
}
