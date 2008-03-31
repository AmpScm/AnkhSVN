using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.VS
{
    public interface IAnkhSolutionSettings
    {
        /// <summary>
        /// Gets or sets the full path of the solution root
        /// </summary>
        /// <remarks>The project root is stored as a relative path from the solution file</remarks>
        string ProjectRoot { get; set; }


        /// <summary>
        /// Gets the solution filename.
        /// </summary>
        /// <value>The solution filename.</value>
        string SolutionFilename { get; }
    }
}
