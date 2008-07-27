using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc.UI;
using Ankh.UI.Services;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.UI.DiffWindow
{
    public partial class DiffToolWindowControl : AnkhToolWindowControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiffControl"/> class.
        /// </summary>
        public DiffToolWindowControl()
        {
            InitializeComponent();
        }

        int _nFrame;
        protected override void OnFrameClose(EventArgs e)
        {
            base.OnFrameClose(e);

            OnClose();
        }

        protected override void OnFrameShow(FrameEventArgs e)
        {
            base.OnFrameShow(e);

            switch(e.Show)
            {
                case __FRAMESHOW.FRAMESHOW_Hidden:
                case __FRAMESHOW.FRAMESHOW_DestroyMultInst:
                case __FRAMESHOW.FRAMESHOW_WinClosed:
                    OnClose();
                    break;
            }
        }

        void OnClose()
        {
            Clear();

            if (_nFrame >= 0)
            {
                Context.GetService<IAnkhDiffHandler>().ReleaseDiff(_nFrame);
                _nFrame = -1;
            }
        }

        private void Clear()
        {
            //throw new NotImplementedException();
        }


        public void Reset(int n, AnkhDiffArgs args)
        {
            _nFrame = n;
            Clear();
        }
    }
}
