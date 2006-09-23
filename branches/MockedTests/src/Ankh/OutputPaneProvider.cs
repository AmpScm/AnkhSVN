using System;
using System.Text;
using System.IO;
using EnvDTE;

namespace Ankh
{
    [Service(typeof(IOutputPaneProvider))]
    public sealed class OutputPaneProvider:IOutputPaneProvider
    {
        public OutputPaneProvider(IServiceProvider serviceProvider)
        {
            _DTE dte = (_DTE)serviceProvider.GetService(typeof(_DTE));
            this.writer = new OutputPaneWriter(dte, "AnkhSVN");
        }

        public OutputPaneTextWriter OutputPaneWriter
        {
            get { return this.writer; }
        }

        private OutputPaneTextWriter writer;
    }
}
