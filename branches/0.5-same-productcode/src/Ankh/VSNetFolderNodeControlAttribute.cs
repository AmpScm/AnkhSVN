// $Id$
using System;

namespace Ankh
{
    /// <summary>
    /// A control attribute that lets you put controls on *all* folder bars.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class VSNetFolderNodeControlAttribute : VSNetControlAttribute
    {
        public VSNetFolderNodeControlAttribute( string bar ) : base(bar)
        {
            // empty
        }

        public override void AddControl(ICommand cmd, AnkhContext context, string tag)
        {
            this.AddControls( Bars, cmd, context, tag );
        }


        private static readonly string[] Bars = new string[]{ "Folder", "Web Reference Folder" };
    }
}
