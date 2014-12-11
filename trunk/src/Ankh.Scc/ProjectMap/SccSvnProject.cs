using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;

namespace Ankh.Scc.ProjectMap
{
	sealed class SccSvnProject : SccProject
	{
		readonly SccProjectData _projectData;
		public SccSvnProject(SccProjectData projectData)
			: base(projectData.ProjectFile, projectData.SccProject)
		{
			_projectData = projectData;
		}

		public override void NotifyGlyphChanged()
		{
			_projectData.NotifyGlyphsChanged();
		}
	}
}
