using System;
using System.IO;

namespace Ankh
{
    public abstract class OutputPaneTextWriter : TextWriter
    {
        public abstract void StartActionText(string action);
        public abstract void EndActionText();
    }
}
