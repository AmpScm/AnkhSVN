using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.Blame
{
    public partial class BlameToolWindowControl : AnkhToolWindowControl
    {
        List<BlameSection> blameSections = new List<BlameSection>();
        public BlameToolWindowControl()
        {
            InitializeComponent();
        }

        public void Init(IAnkhServiceProvider ankhContext)
        {
            this.editorHost1.Init(this, ankhContext);
            this.blameMarginControl1.Init(this, blameSections);
        }

        public void LoadFile(string path)
        {
            editorHost1.LoadFile(path);
        }

        public void AddLine(SharpSvn.SvnBlameEventArgs e)
        {
            BlameSection section;

            if (blameSections.Count == 0)
            {
                section = new BlameSection(e);
                blameSections.Add(section);
            }
            else
            {
                section = blameSections[blameSections.Count - 1];
                if (section.Revision != e.Revision)
                {
                    section.EndLine = (int)e.LineNumber - 1;
                    section = new BlameSection(e);
                    blameSections.Add(section);
                }
            }
            blameMarginControl1.Invalidate();
        }

        internal int GetLineHeight()
        {
            return editorHost1.GetLineHeight();
        }

        internal void NotifyScroll(int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit)
        {
            blameMarginControl1.NotifyScroll(iMinUnit, iMaxUnits, iVisibleUnits, iFirstVisibleUnit);
            
        }


    }
}
