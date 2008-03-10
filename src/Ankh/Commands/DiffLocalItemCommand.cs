// $Id$
using System.IO;
using Ankh.UI;
using EnvDTE;
using SHDocVw;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Shows differences compared to local text base.
    /// </summary>
    [VSNetCommand(AnkhCommand.DiffLocalItem,
        "DiffLocalItem",
         Text = "Di&ff...",
         Tooltip = "Show differences compared to local text base.",
         Bitmap = ResourceBitmaps.Diff),
         VSNetItemControl("", Position = 1)]
    public class DiffLocalItem : LocalDiffCommandBase
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;


            SaveAllDirtyDocuments(context);

            using (context.StartOperation("Diffing"))
            {
                string diff = this.GetDiff(e.Selection, context);
                if (diff != null)
                {
                    // convert it to HTML and store in a temp file
                    DiffHtmlModel model = new DiffHtmlModel(diff);
                    string html = model.GetHtml();
                    string htmlFile = Path.GetTempFileName();
                    using (StreamWriter w = new StreamWriter(htmlFile, false, System.Text.Encoding.Default))
                        w.Write(html);

                    // the Start Page window is a web browser
                    Window browserWindow = ((IDTEContext)e.Context).DTE.Windows.Item(
                        Constants.vsWindowKindWebBrowser);
                    WebBrowser browser = (WebBrowser)browserWindow.Object;

                    // have it show the html
                    object url = "file://" + htmlFile;
                    object nullObject = null;
                    browser.Navigate2(ref url, ref nullObject, ref nullObject,
                        ref nullObject, ref nullObject);
                    browserWindow.Activate();
                }
            }
        }


        #endregion
    }
}