using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Presenters;
using Ankh.UI.Services;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Selection;

namespace Ankh.UI.SvnLog
{
    public partial class LogToolControl : UserControl
    {
        IAnkhUISite _site;
        SvnLogPresenter _presenter;

        public LogToolControl()
        {
            InitializeComponent();
        }

        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;

                IAnkhUISite site = value as IAnkhUISite;

                if (site != null)
                {
                    _site = site;
                    
                    if (_site.GetService<LogToolControl>() == null)
                        _site.Package.AddService(typeof(LogToolControl), this);
                    CreatePresenter(_site);
                    //foreach (PendingChangesPage page in panel1.Controls)
                    //{
                    //    page.UISite = site;
                    //}

                    //UpdateColors();
                    //UpdateCaption();
                }
            }
        }

        ISvnLogService LogService
        {
            get { return _site.GetService<ISvnLogService>();}
        }

        public ICollection<string> Target
        {
            get { return LogService.LocalTargets;}
            set 
            {
                LogService.Cancel();
                LogService.LocalTargets = value;
                logDialogView1.Reset();

                _presenter.Start(value);
            }
        }

        void CreatePresenter(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            ISelectionContext selection = context.GetService<ISelectionContext>();

            List<string> selected = new List<string>(selection.GetSelectedFiles(false));
            if(selected.Count == 0)
                return;

            ISvnLogService svc = context.GetService<ISvnLogService>();
            _presenter = new SvnLogPresenter(logDialogView1, svc);
            _presenter.Start(selected);
        }
    }
}
