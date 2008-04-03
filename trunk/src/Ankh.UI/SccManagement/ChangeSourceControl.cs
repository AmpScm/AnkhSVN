using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Ankh.VS;
using Ankh.Scc;
using SharpSvn;
using Ankh.Selection;

namespace Ankh.UI.SccManagement
{
    public partial class ChangeSourceControl : Form
    {
        IAnkhServiceProvider _context;
        public ChangeSourceControl()
        {
            InitializeComponent();
        }

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (bindingGrid != null)
                RefreshGrid();
        }

        private void RefreshGrid()
        {
            bindingGrid.Rows.Clear();

            if (Context == null)
                return;

            IAnkhSolutionSettings settings = Context.GetService<IAnkhSolutionSettings>();
            IProjectFileMapper mapper = Context.GetService<IProjectFileMapper>();
            IFileStatusCache cache = Context.GetService<IFileStatusCache>();
            IAnkhSccService scc = Context.GetService<IAnkhSccService>();

            if (settings == null || mapper == null || cache == null || scc == null ||
                string.IsNullOrEmpty(settings.SolutionFilename))
            {
                return;
            }

            SvnItem info = cache[settings.SolutionFilename];

            Uri dirUri = info.Status.Uri;
            if(dirUri != null)
                dirUri = new Uri(dirUri, "./");

            bindingGrid.Rows.Add(
                Path.GetFileNameWithoutExtension(settings.SolutionFilename) + " (Solution)",
                info.ParentDirectory.FullPath,
                (dirUri != null) ? dirUri.ToString() : "",
                false,
                scc.IsProjectManagedRaw(null),
                "Ok");

            foreach (SvnProject project in mapper.GetAllProjects())
            {
                ISvnProjectInfo projectInfo = mapper.GetProjectInfo(project);

                if (projectInfo == null || string.IsNullOrEmpty(projectInfo.ProjectDirectory))
                    continue;

                info = cache[projectInfo.ProjectDirectory];

                bindingGrid.Rows.Add(
                    projectInfo.ProjectFullName,
                    projectInfo.ProjectDirectory,
                    info.Status.Uri,
                    false,
                    scc.IsProjectManaged(project),
                    "Ok");
            }

            SvnInfoEventArgs slnInfo = GetInfo(settings.SolutionFilename);

            if (slnInfo != null)
            {
                this.solutionRootBox.Items.Clear();

                Uri myUri = new Uri(slnInfo.Uri, "./");
                UriMap value = new UriMap(myUri, "./");
                solutionRootBox.Items.Add(value);

                if (myUri == value.Uri)
                    solutionRootBox.SelectedItem = value;

                Uri setUri = settings.ProjectRootUri;

                string dir = Path.GetDirectoryName(settings.SolutionFilename);
                string path = "";
                while(myUri != slnInfo.RepositoryRoot)
                {
                    path += "../";
                    myUri = new Uri(myUri, "../");
                    SvnInfoEventArgs dirInfo = GetInfo(Path.Combine(dir, "./" + path));

                    if (dirInfo == null || dirInfo.Uri != myUri)
                        break;

                    value = new UriMap(myUri, path);
                    solutionRootBox.Items.Add(value);
                    if (myUri == setUri)
                        solutionRootBox.SelectedItem = value;
                }
            }
                

            //bindingGrid.Rows.Add(Path.GetFileNameWithoutExtension(settings.SolutionFilename)
        }

        class UriMap
        {
            Uri _uri;
            string _value;
            public UriMap(Uri uri, string value)
            {
                _uri = uri;
                _value = value;
            }

            public override string ToString()
            {
                return _uri.ToString();
            }

            public string Value
            {
                get { return _value; }
            }

            public Uri Uri
            {
                get { return _uri; }
            }

        }

        private SvnInfoEventArgs GetInfo(string filename)
        {
            SvnInfoEventArgs solutionInfo = null;
            using (SvnClient client = Context.GetService<ISvnClientPool>().GetNoUIClient())
            {
                SvnInfoArgs a = new SvnInfoArgs();
                a.Depth = SvnDepth.Empty;
                a.ThrowOnError = false;
                client.Info(new SvnPathTarget(filename), a,
                    delegate(object sender, SvnInfoEventArgs e)
                    {
                        e.Detach();
                        solutionInfo = e;
                    });
            }

            return solutionInfo;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (Context == null)
                return;

            IAnkhSolutionSettings settings = Context.GetService<IAnkhSolutionSettings>();
            UriMap map = solutionRootBox.SelectedItem as UriMap;

            if (map == null)
                return;

            if (settings.ProjectRootUri != null && map.Uri != settings.ProjectRootUri)
            {
                settings.ProjectRoot = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(settings.SolutionFilename), map.Value));
            }
        }
    }
}
