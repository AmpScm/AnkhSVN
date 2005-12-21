using System;
using System.Collections;

namespace Ankh.Solution
{
    /// <summary>
    /// Part of a project tree for a parsed solution
    /// </summary>
    public class ParsedSolutionItem
    {
		/// <summary>
		/// Constructor
		/// </summary>
        public ParsedSolutionItem()
        {
            this.children=new ArrayList();
        }

		/// <summary>
		/// Find the direct child with the given name
		/// </summary>
		/// <param name="childName">Name to search for</param>
		/// <returns>The desired child item, or null</returns>
        public ParsedSolutionItem GetChild(string childName)
        {
            foreach(ParsedSolutionItem child in this.children)
            {
                if(child.Name==childName)
                {
                    return child;
                }
            }

            return null;
		}

		/// <summary>
		/// The set of child items
		/// </summary>
		public IList Children
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get{ return this.children; }
		}

		/// <summary>
		/// File name of this item
		/// </summary>
		public string FileName
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get{ return this.fileName; }
			[System.Diagnostics.DebuggerStepThrough()]
			set{ this.fileName=value; }
		}

		/// <summary>
		/// Item name
		/// </summary>
		public string Name
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get{ return this.name; }
			[System.Diagnostics.DebuggerStepThrough()]
			set{ this.name=value; }
		}

		/// <summary>
		/// Parent item
		/// </summary>
		public ParsedSolutionItem Parent
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get{ return this.parent; }
			[System.Diagnostics.DebuggerStepThrough()]
			set{ this.parent=value; }
		}
		
		private ArrayList children;
		string fileName;
		string name;
		ParsedSolutionItem parent;
    }
}
