﻿using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using SharpSvn;
using Ankh.VS;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Ids;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogShowChanges)]
    class ShowChanges : ICommandHandler
    {
        TempFileCollection _collection = new TempFileCollection();
        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
            long min = long.MaxValue;
            long max = long.MinValue;

            foreach (LogItem item in e.Selection.GetSelection<LogItem>())
            {
                min = Math.Min(min, item.RawData.Revision);
                max = Math.Max(max, item.RawData.Revision);
            }

            SvnRevisionRange range = new SvnRevisionRange(min-1, max);

            using (SvnClient client = e.GetService<ISvnClientPool>().GetClient())
            {
                string htmlFile = Path.GetTempFileName();
                _collection.AddFile(htmlFile, false);

                using (MemoryStream ms = new MemoryStream())
                using(StreamReader reader = new StreamReader(ms))
                {
                    client.Diff(new SvnUriTarget(e.GetService<IAnkhSolutionSettings>().ProjectRootUri), range, ms);
                    ms.Flush();
                    ms.Position = 0;

                    DiffHtmlModel model = new DiffHtmlModel(reader.ReadToEnd());
                    
                    using(FileStream fs = File.OpenWrite(htmlFile))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(model.GetHtml());
                    }
                }

                IAnkhWebBrowser browser = e.Context.GetService<IAnkhWebBrowser>();
                BrowserArgs args = new BrowserArgs();
                args.CreateFlags = __VSCREATEWEBBROWSER.VSCWB_AutoShow |
                    __VSCREATEWEBBROWSER.VSCWB_NoHistory |
                    __VSCREATEWEBBROWSER.VSCWB_StartCustom |
                    __VSCREATEWEBBROWSER.VSCWB_OptionDisableStatusBar;
                args.BaseCaption = "Subversion";
                browser.Navigate(new Uri("file:///" + htmlFile), args);
            }
        }
    }
}
