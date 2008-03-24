using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.UITypeEditors
{
    /// <summary>
    /// UIType editor. Namespace referenced from Ankh.Configuration.Config, Ankh.Configuration
    /// </summary>
    public class LogMessageTypeEditor : StringTypeEditorWithTemplates
    {
        public LogMessageTypeEditor()
        {
            this.SetTitle("LogMessage");
        }

        protected override StringEditorTemplate[] GetTemplates()
        {
            return new StringEditorTemplate[]{
                        new StringEditorTemplate("#", "Comment", "Comment (#)"),
                        new StringEditorTemplate("***", "For each file", "For each file (***)"),
                        new StringEditorTemplate("%path%", "The file path", "The file path (%path%)")
                    };
        }
    }
}
