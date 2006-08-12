using System;
using Microsoft.Office.Core;

namespace Ankh
{
    /// <summary>
    /// An attribute that lets you add controls to *all* of the VS Project bars.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class VSNetProjectNodeControlAttribute : VSNetControlAttribute
    {
        public VSNetProjectNodeControlAttribute( string commandBar ) : 
            base( commandBar )
        {
            // empty
        }
        
        public override void AddControl(ICommand cmd, IContext context, string tag)
        {
            this.AddControls( Bars, cmd, context, tag );
        }

        private static readonly string[] Bars = new string[]{ 
            "Project Node", "Cab Project Node", "Project", "Web Project Folder", 
            "Database Project" };

    }
}
