using System;

namespace Ankh
{
    /// <summary>
    /// A control attribute that lets you put controls on *all* item bars.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class VSNetProjectItemControlAttribute : VSNetControlAttribute
	{
		public VSNetProjectItemControlAttribute( string bar ) : base( bar )
		{
		}

        public override void AddControl(ICommand cmd, IContext context, string tag)
        {
            this.AddControls( Bars, cmd, context, tag );
        }

        private static readonly string[] Bars = new string[]{ 
            "Item", "Web Item" };
	}
}
