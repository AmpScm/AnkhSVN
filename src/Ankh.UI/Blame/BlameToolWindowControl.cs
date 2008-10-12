using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SharpSvn;
using Ankh.UI.PendingChanges;

namespace Ankh.UI.Blame
{
    public partial class BlameToolWindowControl : AnkhToolWindowControl
    {
        List<BlameSection> blameSections = new List<BlameSection>();
        public BlameToolWindowControl()
        {
            InitializeComponent();
            logMessageEditor1.SkipLogLanguageService = true;
            logMessageEditor1.ReadOnly = true;
        }

        public void Init(IAnkhServiceProvider ankhContext)
        {
            //this.editorHost1.Init(this, ankhContext);
            //logMessageEditor1.ReadOnly = true;
      //      this.logMessageEditor1.Init(ankhContext, false);
            this.blameMarginControl1.Init(this, blameSections);
        }

        public void LoadFile(string projectFile, string exportedFile)
        {
            this.Text = Path.GetFileName(projectFile) + " (Annotated)"; 
//            editorHost1.LoadFile(projectFile, exportedFile);
            logMessageEditor1.OpenFile(projectFile);
            logMessageEditor1.ReplaceContents(exportedFile);

        }

        internal int GetLineHeight()
        {
            return logMessageEditor1.LineHeight;
        }


        public void AddLines(System.Collections.ObjectModel.Collection<SharpSvn.SvnBlameEventArgs> blameResult)
        {
            BlameSection section = null;
            blameSections.Clear();

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

        private void logMessageEditor1_Scroll(object sender, TextViewScrollEventArgs e)
        {
            blameMarginControl1.NotifyScroll(e.MinUnit, e.MaxUnit, e.VisibleUnits, e.FirstVisibleUnit);
        }
    }
}
