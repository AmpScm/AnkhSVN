using System;
using System.Text;

namespace Ankh
{
    /// <summary>
    /// This attribute is used for commands that just want to grab the resources of the current
    /// selection, regardless of where they are.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class VSNetItemControlAttribute : VSNetControlAttribute
    {
        public VSNetItemControlAttribute( string bar ) : base(bar)
        {
            
        }

        public override void AddControl( ICommand cmd, IContext context, string tag )
        {
            this.AddControls( Bars, cmd, context, tag );
        }

        private static readonly string[] Bars = new string[]{ 
            "Item", "Web Item", 
            "Project Node", "Cab Project Node", "Project", "Web Project Folder", 
            "Database Project", "Folder", "Web Reference Folder", "Web Folder", "DB Project Folder",
            "Script", "Solution", "WorkingCopyExplorer", "Cross Project Multi Project" };
    }
}
