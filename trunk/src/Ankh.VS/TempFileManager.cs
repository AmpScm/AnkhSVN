using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.IO;

namespace Ankh.VS
{
    [GlobalService(typeof(IAnkhTempFileManager))]
    class TempFileManager : AnkhService, IAnkhTempFileManager
    {
        TempFileCollection _tempFileCollection;

        public TempFileManager(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public TempFileCollection TempFileCollection
        {
            get { return _tempFileCollection ?? (_tempFileCollection = new TempFileCollection()); }
        }

        #region IAnkhTempFileManager Members

        public string GetTempFile()
        {
            string name = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            using (FileStream fs = File.Create(name))
            {
            }
            TempFileCollection.AddFile(name, false);
            return name;            
        }

        public string GetTempFile(string extension)
        {
            string name = Path.ChangeExtension(
                Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")),
                extension);
            using (FileStream fs = File.Create(name))
            {
            }
            TempFileCollection.AddFile(name, false);
            return name;            
        }
        #endregion
    }
}
