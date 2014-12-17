using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;

namespace Ankh.Scc.ProjectMap
{
	sealed class SvnSccProject : SccProject
	{
		readonly SccProjectData _projectData;
		public SvnSccProject(SccProjectData projectData)
			: base(projectData.ProjectFile, projectData.SccProject)
		{
			_projectData = projectData;
		}

		public override void NotifyGlyphChanged()
		{
            // NOT: base.NotifyGlyphChanged();
			_projectData.NotifyGlyphsChanged();
		}
	}
}
