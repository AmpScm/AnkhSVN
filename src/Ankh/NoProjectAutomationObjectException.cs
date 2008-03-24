using System;
using System.Text;

namespace Ankh
{
    /// <summary>
    /// Exception type to be used when an automation object cannot be retrieved for a project.
    /// </summary>
    public class NoProjectAutomationObjectException : Exception
    {
        public NoProjectAutomationObjectException( string name )
        {
            this.projectName = name;
        }

        public NoProjectAutomationObjectException( string name, Exception innerException )
            : base( name, innerException )
        {
            this.projectName = name;
        }

        public string ProjectName
        {
            get { return projectName; }
        }

        private string projectName;
    }
}
