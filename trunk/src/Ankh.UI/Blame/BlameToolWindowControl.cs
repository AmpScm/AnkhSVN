using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SharpSvn;

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

        public void LoadFile(string projectFile, string exportedFile)
        {
            this.Text = Path.GetFileName(projectFile) + " (Annotated)"; 
            editorHost1.LoadFile(projectFile, exportedFile);
        }

        internal int GetLineHeight()
        {
            return editorHost1.GetLineHeight();
        }

        internal void NotifyScroll(int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit)
        {
            blameMarginControl1.NotifyScroll(iMinUnit, iMaxUnits, iVisibleUnits, iFirstVisibleUnit);
            
        }


        public void AddLines(System.Collections.ObjectModel.Collection<SharpSvn.SvnBlameEventArgs> blameResult)
        {
            BlameSection section = null;

            foreach (SvnBlameEventArgs e in blameResult)
            {
                if (blameSections.Count == 0)
                {
                    section = new BlameSection(e);
                    blameSections.Add(section);
                }
                else
                {
                    if(section.Revision == e.Revision)
                        section.EndLine = (int)e.LineNumber;
                    else
                    {
                        section = new BlameSection(e);
                        blameSections.Add(section);
                    }
                }

                
            }
            blameMarginControl1.Invalidate();
        }
    }
}
