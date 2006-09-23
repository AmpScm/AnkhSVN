using System;
using System.Text;
using System.IO;

namespace Ankh
{
    /// <summary>
    /// Provides a <see cref="OutputPaneTextWriter"/> linked to the visual studio output pane
    /// </summary>
    public interface IOutputPaneProvider
    {
        /// <summary>
        /// Gets a <see cref="OutputPaneTextWriter"/> linked to the visual studio output pane
        /// </summary>
        OutputPaneTextWriter OutputPaneWriter { get;}
    }
}
