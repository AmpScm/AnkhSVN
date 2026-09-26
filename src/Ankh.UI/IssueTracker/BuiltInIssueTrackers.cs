// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.Scc;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using SharpSvn;

namespace Ankh.UI.IssueTracker
{
    /// <summary>
    /// Built-in issue connectors shipped with AnkhSVN. External connectors are
    /// still discovered through the original registration mechanism.
    /// </summary>
    public static class BuiltInIssueTrackerConnectors
    {
        public static IEnumerable<IssueRepositoryConnector> Create(IAnkhServiceProvider context)
        {
            yield return new GenericBugtraqConnector(context);
            yield return new LocalSvnIssuesConnector(context);
        }
    }

    internal interface IBuiltInIssueRepositoryPersistence
    {
        void Persist(SvnItem projectRoot);
    }

    internal interface IIssueRepositoryCommitUi
    {
        bool ShowIssueBox { get; }
        string IssueLabel { get; }
        bool NumericIssueIds { get; }
    }

    internal interface IValidatingIssueConfigurationPage
    {
        bool IsComplete { get; }
    }

    #region Generic bugtraq

    internal sealed class GenericBugtraqSettings : IssueRepositorySettings
    {
        internal const string Connector = "Generic Bugtraq";

        internal GenericBugtraqSettings()
            : base(Connector)
        {
            MessagePattern = "Issue #%BUGID%";
            Label = "Issue:";
            NumericIssueIds = true;
            Append = true;
        }

        internal string UrlTemplate { get; set; }
        internal string MessagePattern { get; set; }
        internal string Label { get; set; }
        internal string LogRegex { get; set; }
        internal bool WarnIfNoIssue { get; set; }
        internal bool NumericIssueIds { get; set; }
        internal bool Append { get; set; }

        internal bool IsComplete
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UrlTemplate)
                    && UrlTemplate.IndexOf("%BUGID%", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        public override Uri RepositoryUri
        {
            get { return new Uri("bugtraq://standard/"); }
        }

        public override string RepositoryId
        {
            get { return "standard"; }
        }

        internal static GenericBugtraqSettings FromProject(IAnkhServiceProvider context)
        {
            GenericBugtraqSettings settings = new GenericBugtraqSettings();
            if (context == null)
                return settings;

            IProjectCommitSettings commit = context.GetService<IProjectCommitSettings>();
            if (commit != null)
            {
                settings.UrlTemplate = commit.RawIssueTrackerUri;
                settings.MessagePattern = string.IsNullOrEmpty(commit.RawIssueTrackerMessage)
                    ? settings.MessagePattern
                    : commit.RawIssueTrackerMessage;
                settings.Label = string.IsNullOrEmpty(commit.IssueLabel)
                    ? settings.Label
                    : commit.IssueLabel;
                settings.WarnIfNoIssue = commit.WarnIfNoIssue;
                settings.NumericIssueIds = commit.NummericIssueIds;
                settings.Append = commit.AppendIssueTrackerMessage;
            }

            IAnkhSolutionSettings solution = context.GetService<IAnkhSolutionSettings>();
            ISvnClientPool pool = context.GetService<ISvnClientPool>();
            if (solution != null && pool != null && !string.IsNullOrEmpty(solution.ProjectRoot))
            {
                using (SvnPoolClient client = pool.GetNoUIClient())
                {
                    string value;
                    if (client.TryGetProperty(
                        solution.ProjectRoot,
                        SvnPropertyNames.BugTrackLogRegex,
                        out value))
                    {
                        settings.LogRegex = value;
                    }
                }
            }

            return settings;
        }
    }

    internal sealed class GenericBugtraqConnector : IssueRepositoryConnector
    {
        readonly IAnkhServiceProvider _context;

        internal GenericBugtraqConnector(IAnkhServiceProvider context)
        {
            _context = context;
        }

        public override string Name
        {
            get { return GenericBugtraqSettings.Connector; }
        }

        public override IssueRepositoryConfigurationPage ConfigurationPage
        {
            // Configuration controls are owned/disposed by each setup dialog.
            // Return a fresh page so reopening or switching back to this
            // connector never reuses a disposed WinForms control.
            get { return new GenericBugtraqConfigurationPage(_context); }
        }

        public override IssueRepository Create(IssueRepositorySettings settings)
        {
            GenericBugtraqSettings values = settings as GenericBugtraqSettings;
            if (values == null)
                values = GenericBugtraqSettings.FromProject(_context);

            return new GenericBugtraqRepository(_context, values);
        }
    }

    internal sealed class GenericBugtraqConfigurationPage :
        IssueRepositoryConfigurationPage,
        IValidatingIssueConfigurationPage
    {
        readonly GenericBugtraqConfigControl _control;
        GenericBugtraqSettings _settings;

        internal GenericBugtraqConfigurationPage(IAnkhServiceProvider context)
        {
            _settings = GenericBugtraqSettings.FromProject(context);
            _control = new GenericBugtraqConfigControl(_settings);
            _control.Changed += delegate
            {
                _settings = _control.BuildSettings();
                ConfigurationPageChanged(new ConfigPageEventArgs { IsComplete = IsComplete });
            };
        }

        public override IWin32Window Window
        {
            get { return _control; }
        }

        public override IssueRepositorySettings Settings
        {
            get
            {
                _settings = _control.BuildSettings();
                return _settings;
            }
            set
            {
                GenericBugtraqSettings generic = value as GenericBugtraqSettings;
                if (generic != null)
                {
                    _settings = generic;
                    _control.LoadSettings(_settings);
                }

                ConfigurationPageChanged(new ConfigPageEventArgs { IsComplete = IsComplete });
            }
        }

        public bool IsComplete
        {
            get { return _control.BuildSettings().IsComplete; }
        }
    }

    internal sealed class GenericBugtraqConfigControl : UserControl
    {
        readonly TextBox _url = new TextBox();
        readonly TextBox _message = new TextBox();
        readonly TextBox _label = new TextBox();
        readonly TextBox _regex = new TextBox();
        readonly CheckBox _warn = new CheckBox();
        readonly CheckBox _numeric = new CheckBox();
        readonly CheckBox _append = new CheckBox();

        internal GenericBugtraqConfigControl(GenericBugtraqSettings settings)
        {
            Dock = DockStyle.Fill;
            AutoScroll = true;

            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(8)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            AddRow(table, 0, "Issue URL:", _url);
            AddRow(table, 1, "Message pattern:", _message);
            AddRow(table, 2, "Issue label:", _label);
            AddRow(table, 3, "Log regex:", _regex);

            _warn.Text = "Remind me to enter an issue ID";
            _warn.AutoSize = true;
            _numeric.Text = "Issue IDs are numeric";
            _numeric.AutoSize = true;
            _append.Text = "Append issue reference to the bottom of the commit message";
            _append.AutoSize = true;

            table.Controls.Add(_warn, 1, 4);
            table.Controls.Add(_numeric, 1, 5);
            table.Controls.Add(_append, 1, 6);

            Label note = new Label
            {
                AutoSize = true,
                MaximumSize = new System.Drawing.Size(560, 0),
                Text = "Uses standard Subversion bugtraq:* properties, so the same settings are compatible with TortoiseSVN. The URL must contain %BUGID%."
            };
            table.Controls.Add(note, 0, 7);
            table.SetColumnSpan(note, 2);

            Controls.Add(table);

            foreach (TextBox box in new[] { _url, _message, _label, _regex })
                box.TextChanged += delegate { OnChanged(); };
            foreach (CheckBox box in new[] { _warn, _numeric, _append })
                box.CheckedChanged += delegate { OnChanged(); };

            LoadSettings(settings);
        }

        internal event EventHandler Changed;

        static void AddRow(TableLayoutPanel table, int row, string label, Control control)
        {
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Label l = new Label
            {
                Text = label,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(3, 7, 8, 3)
            };
            control.Dock = DockStyle.Fill;
            control.Margin = new Padding(3);
            table.Controls.Add(l, 0, row);
            table.Controls.Add(control, 1, row);
        }

        void OnChanged()
        {
            EventHandler handler = Changed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        internal void LoadSettings(GenericBugtraqSettings settings)
        {
            if (settings == null)
                settings = new GenericBugtraqSettings();

            _url.Text = settings.UrlTemplate ?? "";
            _message.Text = settings.MessagePattern ?? "";
            _label.Text = settings.Label ?? "";
            _regex.Text = settings.LogRegex ?? "";
            _warn.Checked = settings.WarnIfNoIssue;
            _numeric.Checked = settings.NumericIssueIds;
            _append.Checked = settings.Append;
        }

        internal GenericBugtraqSettings BuildSettings()
        {
            return new GenericBugtraqSettings
            {
                UrlTemplate = _url.Text.Trim(),
                MessagePattern = _message.Text,
                Label = _label.Text,
                LogRegex = _regex.Text,
                WarnIfNoIssue = _warn.Checked,
                NumericIssueIds = _numeric.Checked,
                Append = _append.Checked
            };
        }
    }

    internal sealed class GenericBugtraqRepository :
        IssueRepository,
        IBuiltInIssueRepositoryPersistence,
        IIssueRepositoryCommitUi
    {
        readonly IAnkhServiceProvider _context;
        readonly GenericBugtraqSettings _settings;
        GenericBugtraqView _view;

        internal GenericBugtraqRepository(
            IAnkhServiceProvider context,
            GenericBugtraqSettings settings)
            : base(GenericBugtraqSettings.Connector)
        {
            _context = context;
            _settings = settings ?? new GenericBugtraqSettings();
        }

        public override string Label
        {
            get { return string.IsNullOrEmpty(_settings.Label) ? "Issue" : _settings.Label; }
        }

        public override Uri RepositoryUri
        {
            get { return _settings.RepositoryUri; }
        }

        public override string RepositoryId
        {
            get { return _settings.RepositoryId; }
        }

        public override IWin32Window Window
        {
            get { return _view ?? (_view = new GenericBugtraqView(this, _settings)); }
        }

        public override void NavigateTo(string issueId)
        {
            if (string.IsNullOrWhiteSpace(issueId) || _context == null)
                return;

            IProjectCommitSettings commit = _context.GetService<IProjectCommitSettings>();
            Uri uri = commit == null ? null : commit.GetIssueTrackerUri(issueId.Trim());

            if (uri == null && !string.IsNullOrEmpty(_settings.UrlTemplate))
            {
                string raw = _settings.UrlTemplate.Replace(
                    "%BUGID%",
                    Uri.EscapeDataString(issueId.Trim()));

                Uri.TryCreate(raw, UriKind.Absolute, out uri);
            }

            if (uri != null)
            {
                IAnkhWebBrowser browser = _context.GetService<IAnkhWebBrowser>();
                if (browser != null)
                    browser.Navigate(uri);
            }
        }

        public void Persist(SvnItem projectRoot)
        {
            if (_context == null || projectRoot == null)
                return;

            ISvnClientPool pool = _context.GetService<ISvnClientPool>();
            if (pool == null)
                return;

            using (SvnPoolClient client = pool.GetNoUIClient())
            {
                SetOrDelete(client, projectRoot.FullPath, SvnPropertyNames.BugTrackUrl, _settings.UrlTemplate);
                SetOrDelete(client, projectRoot.FullPath, SvnPropertyNames.BugTrackMessage, _settings.MessagePattern);
                SetOrDelete(client, projectRoot.FullPath, SvnPropertyNames.BugTrackLabel, _settings.Label);
                SetOrDelete(client, projectRoot.FullPath, SvnPropertyNames.BugTrackLogRegex, _settings.LogRegex);
                client.SetProperty(projectRoot.FullPath, SvnPropertyNames.BugTrackWarnIfNoIssue, _settings.WarnIfNoIssue ? "true" : "false");
                client.SetProperty(projectRoot.FullPath, SvnPropertyNames.BugTrackNumber, _settings.NumericIssueIds ? "true" : "false");
                client.SetProperty(projectRoot.FullPath, SvnPropertyNames.BugTrackAppend, _settings.Append ? "true" : "false");
            }

            ISvnStatusCache cache = _context.GetService<ISvnStatusCache>();
            if (cache != null)
                cache.MarkDirty(projectRoot.FullPath);
        }

        static void SetOrDelete(SvnClient client, string path, string name, string value)
        {
            if (string.IsNullOrEmpty(value))
                client.DeleteProperty(path, name);
            else
                client.SetProperty(path, name, value);
        }

        bool IIssueRepositoryCommitUi.ShowIssueBox
        {
            get { return !string.IsNullOrEmpty(_settings.MessagePattern); }
        }

        string IIssueRepositoryCommitUi.IssueLabel
        {
            get { return Label; }
        }

        bool IIssueRepositoryCommitUi.NumericIssueIds
        {
            get { return _settings.NumericIssueIds; }
        }
    }

    internal sealed class GenericBugtraqView : UserControl
    {
        readonly GenericBugtraqRepository _repository;
        readonly TextBox _issueId = new TextBox();

        internal GenericBugtraqView(
            GenericBugtraqRepository repository,
            GenericBugtraqSettings settings)
        {
            _repository = repository;
            Dock = DockStyle.Fill;

            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(12)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            Label intro = new Label
            {
                AutoSize = true,
                MaximumSize = new System.Drawing.Size(700, 0),
                Text = "Generic Bugtraq uses the standard SVN bugtraq properties. It cannot enumerate issues from the remote tracker, but you can open an issue by ID and AnkhSVN will use the configured URL template."
            };
            table.Controls.Add(intro, 0, 0);
            table.SetColumnSpan(intro, 2);

            Label url = new Label
            {
                AutoSize = true,
                Text = "URL: " + (settings.UrlTemplate ?? "(not configured)"),
                Margin = new Padding(3, 10, 3, 10)
            };
            table.Controls.Add(url, 0, 1);
            table.SetColumnSpan(url, 2);

            table.Controls.Add(new Label
            {
                Text = "Issue ID:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            }, 0, 2);

            _issueId.Dock = DockStyle.Fill;
            table.Controls.Add(_issueId, 1, 2);

            Button open = new Button
            {
                Text = "Open Issue",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            open.Click += delegate { _repository.NavigateTo(_issueId.Text); };
            table.Controls.Add(open, 1, 3);

            Controls.Add(table);
        }
    }

    #endregion

    #region Local SVN issues

    internal sealed class LocalSvnIssuesSettings : IssueRepositorySettings
    {
        internal const string Connector = "Local SVN Issues";
        internal const string DefaultPath = ".ankh/issues.xml";

        internal LocalSvnIssuesSettings(string relativePath)
            : base(Connector)
        {
            RelativePath = string.IsNullOrWhiteSpace(relativePath)
                ? DefaultPath
                : relativePath.Trim();
        }

        internal string RelativePath { get; private set; }

        public override Uri RepositoryUri
        {
            get { return new Uri("local-svn://issues/"); }
        }

        public override string RepositoryId
        {
            get { return RelativePath; }
        }
    }

    internal sealed class LocalSvnIssuesConnector : IssueRepositoryConnector
    {
        readonly IAnkhServiceProvider _context;

        internal LocalSvnIssuesConnector(IAnkhServiceProvider context)
        {
            _context = context;
        }

        public override string Name
        {
            get { return LocalSvnIssuesSettings.Connector; }
        }

        public override IssueRepositoryConfigurationPage ConfigurationPage
        {
            // Configuration controls are owned/disposed by each setup dialog.
            // Return a fresh page so Change Tracker can be reopened safely.
            get { return new LocalSvnIssuesConfigurationPage(); }
        }

        public override IssueRepository Create(IssueRepositorySettings settings)
        {
            LocalSvnIssuesSettings local = settings as LocalSvnIssuesSettings;
            if (local == null)
                local = new LocalSvnIssuesSettings(
                    settings == null ? null : settings.RepositoryId);

            return new LocalSvnIssuesRepository(_context, local);
        }
    }

    internal sealed class LocalSvnIssuesConfigurationPage :
        IssueRepositoryConfigurationPage,
        IValidatingIssueConfigurationPage
    {
        readonly LocalSvnIssuesConfigControl _control =
            new LocalSvnIssuesConfigControl(LocalSvnIssuesSettings.DefaultPath);

        internal LocalSvnIssuesConfigurationPage()
        {
            _control.Changed += delegate
            {
                ConfigurationPageChanged(new ConfigPageEventArgs { IsComplete = IsComplete });
            };
        }

        public override IWin32Window Window
        {
            get { return _control; }
        }

        public override IssueRepositorySettings Settings
        {
            get { return new LocalSvnIssuesSettings(_control.RelativePath); }
            set
            {
                string path = value == null ? null : value.RepositoryId;
                _control.RelativePath = string.IsNullOrEmpty(path)
                    ? LocalSvnIssuesSettings.DefaultPath
                    : path;
                ConfigurationPageChanged(new ConfigPageEventArgs { IsComplete = IsComplete });
            }
        }

        public bool IsComplete
        {
            get { return LocalIssueStore.IsValidRelativePath(_control.RelativePath); }
        }
    }

    internal sealed class LocalSvnIssuesConfigControl : UserControl
    {
        readonly TextBox _path = new TextBox();

        internal LocalSvnIssuesConfigControl(string path)
        {
            Dock = DockStyle.Fill;

            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(8)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            Label intro = new Label
            {
                AutoSize = true,
                MaximumSize = new System.Drawing.Size(560, 0),
                Text = "Stores a small issue list inside the working copy so it can be versioned and shared through SVN. No external service is required."
            };
            table.Controls.Add(intro, 0, 0);
            table.SetColumnSpan(intro, 2);

            table.Controls.Add(new Label
            {
                Text = "Issue file:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            }, 0, 1);
            _path.Dock = DockStyle.Fill;
            table.Controls.Add(_path, 1, 1);

            Label note = new Label
            {
                AutoSize = true,
                MaximumSize = new System.Drawing.Size(560, 0),
                Text = "The path is relative to the solution working-copy root. The default is .ankh/issues.xml."
            };
            table.Controls.Add(note, 0, 2);
            table.SetColumnSpan(note, 2);

            Controls.Add(table);

            _path.TextChanged += delegate
            {
                EventHandler handler = Changed;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            };
            RelativePath = path;
        }

        internal event EventHandler Changed;

        internal string RelativePath
        {
            get { return _path.Text.Trim(); }
            set { _path.Text = value ?? ""; }
        }
    }

    internal sealed class LocalSvnIssuesRepository :
        IssueRepository,
        IBuiltInIssueRepositoryPersistence,
        IIssueRepositoryCommitUi
    {
        static readonly Regex IssueRegex = new Regex(
            @"(?i)\b(?:issue\s*#?|#)(?<id>\d+)\b",
            RegexOptions.CultureInvariant | RegexOptions.Compiled);

        readonly IAnkhServiceProvider _context;
        readonly LocalSvnIssuesSettings _settings;
        readonly LocalIssueStore _store;
        LocalSvnIssuesView _view;

        internal LocalSvnIssuesRepository(
            IAnkhServiceProvider context,
            LocalSvnIssuesSettings settings)
            : base(LocalSvnIssuesSettings.Connector)
        {
            _context = context;
            _settings = settings ?? new LocalSvnIssuesSettings(null);
            _store = new LocalIssueStore(context, _settings.RelativePath);
        }

        public override string Label
        {
            get { return "Local SVN Issues"; }
        }

        public override Uri RepositoryUri
        {
            get { return _settings.RepositoryUri; }
        }

        public override string RepositoryId
        {
            get { return _settings.RepositoryId; }
        }

        public override Regex IssueIdRegex
        {
            get { return IssueRegex; }
        }

        public override IWin32Window Window
        {
            get { return _view ?? (_view = new LocalSvnIssuesView(_context, this, _store)); }
        }

        public override void NavigateTo(string issueId)
        {
            LocalSvnIssuesView view = (LocalSvnIssuesView)Window;
            view.SelectIssue(issueId, true);
        }

        public override void PreCommit(PreCommitArgs args)
        {
            base.PreCommit(args);

            if (args == null || string.IsNullOrWhiteSpace(args.IssueText))
                return;

            args.CommitMessage = AppendIssueReference(args.CommitMessage, args.IssueText);
            args.IssueText = null;
            args.SkipIssueVerify = true;
        }

        internal static string AppendIssueReference(string message, string issueText)
        {
            string clean = (issueText ?? "").Trim();
            if (clean.Length == 0)
                return message;

            string reference = "Issue #" + clean;
            string existing = message ?? "";

            if (existing.IndexOf(reference, StringComparison.OrdinalIgnoreCase) >= 0)
                return existing;

            if (existing.Length == 0)
                return reference;

            return existing.TrimEnd() + Environment.NewLine + reference;
        }

        public void Persist(SvnItem projectRoot)
        {
            _store.EnsureFileVersioned();
        }

        bool IIssueRepositoryCommitUi.ShowIssueBox
        {
            get { return true; }
        }

        string IIssueRepositoryCommitUi.IssueLabel
        {
            get { return "Issue:"; }
        }

        bool IIssueRepositoryCommitUi.NumericIssueIds
        {
            get { return true; }
        }
    }

    internal sealed class LocalIssueRecord
    {
        internal int Id { get; set; }
        internal string Status { get; set; }
        internal string Title { get; set; }
        internal string Description { get; set; }
        internal DateTime CreatedUtc { get; set; }
        internal DateTime UpdatedUtc { get; set; }

        internal LocalIssueRecord Clone()
        {
            return (LocalIssueRecord)MemberwiseClone();
        }
    }

    internal sealed class LocalIssueStore
    {
        readonly IAnkhServiceProvider _context;
        readonly string _relativePath;

        internal LocalIssueStore(IAnkhServiceProvider context, string relativePath)
        {
            _context = context;
            _relativePath = relativePath;
        }

        internal string FullPath
        {
            get
            {
                if (_context == null)
                    return null;

                IAnkhSolutionSettings settings =
                    _context.GetService<IAnkhSolutionSettings>();
                if (settings == null || string.IsNullOrEmpty(settings.ProjectRoot))
                    return null;

                return ResolvePath(settings.ProjectRoot, _relativePath);
            }
        }

        internal static bool IsValidRelativePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || Path.IsPathRooted(path))
                return false;

            string normalized = path.Replace('/', '\\');
            string[] parts = normalized.Split(
                new[] { '\\' },
                StringSplitOptions.RemoveEmptyEntries);

            return parts.Length > 0
                && !parts.Any(p => p == "." || p == "..")
                && path.IndexOf(':') < 0;
        }

        internal static string ResolvePath(string root, string relativePath)
        {
            if (string.IsNullOrEmpty(root))
                throw new ArgumentNullException("root");
            if (!IsValidRelativePath(relativePath))
                throw new ArgumentException("Issue file must be a relative path.", "relativePath");

            string rootFull = Path.GetFullPath(root)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            string full = Path.GetFullPath(Path.Combine(rootFull, relativePath));

            if (!full.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Issue file must stay under the working-copy root.", "relativePath");

            return full;
        }

        internal List<LocalIssueRecord> Load()
        {
            string file = FullPath;
            return string.IsNullOrEmpty(file)
                ? new List<LocalIssueRecord>()
                : LoadFile(file);
        }

        internal void Save(IEnumerable<LocalIssueRecord> issues)
        {
            string file = FullPath;
            if (string.IsNullOrEmpty(file))
                throw new InvalidOperationException("No solution working-copy root is available.");

            SaveFile(file, issues);
            EnsureFileVersioned();
        }

        internal void EnsureFileVersioned()
        {
            string file = FullPath;
            if (string.IsNullOrEmpty(file))
                return;

            if (!File.Exists(file))
                SaveFile(file, new LocalIssueRecord[0]);

            if (_context == null)
                return;

            ISvnStatusCache cache = _context.GetService<ISvnStatusCache>();
            SvnItem item = cache == null ? null : cache[file];

            if (item == null || !item.IsVersioned)
            {
                ISvnClientPool pool = _context.GetService<ISvnClientPool>();
                if (pool != null)
                {
                    using (SvnPoolClient client = pool.GetNoUIClient())
                    {
                        SvnAddArgs args = new SvnAddArgs
                        {
                            AddParents = true,
                            Depth = SvnDepth.Empty,
                            ThrowOnError = false
                        };

                        if (!client.Add(file, args) && args.LastException != null)
                            throw args.LastException;
                    }
                }
            }

            if (cache != null)
            {
                cache.MarkDirty(file);
                cache.MarkDirty(Path.GetDirectoryName(file));
            }
        }

        internal static List<LocalIssueRecord> LoadFile(string file)
        {
            List<LocalIssueRecord> result = new List<LocalIssueRecord>();
            if (!File.Exists(file))
                return result;

            XmlDocument document = new XmlDocument();
            document.Load(file);

            XmlNodeList nodes = document.SelectNodes("/issues/issue");
            if (nodes == null)
                return result;

            foreach (XmlElement element in nodes)
            {
                int id;
                if (!int.TryParse(element.GetAttribute("id"), out id))
                    continue;

                DateTime created;
                DateTime updated;
                DateTime.TryParse(
                    element.GetAttribute("createdUtc"),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out created);
                DateTime.TryParse(
                    element.GetAttribute("updatedUtc"),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out updated);

                XmlNode title = element.SelectSingleNode("title");
                XmlNode description = element.SelectSingleNode("description");

                result.Add(new LocalIssueRecord
                {
                    Id = id,
                    Status = string.IsNullOrEmpty(element.GetAttribute("status"))
                        ? "Open"
                        : element.GetAttribute("status"),
                    Title = title == null ? "" : title.InnerText,
                    Description = description == null ? "" : description.InnerText,
                    CreatedUtc = created,
                    UpdatedUtc = updated
                });
            }

            return result.OrderBy(i => i.Id).ToList();
        }

        internal static void SaveFile(
            string file,
            IEnumerable<LocalIssueRecord> issues)
        {
            string directory = Path.GetDirectoryName(file);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            XmlDocument document = new XmlDocument();
            XmlElement root = document.CreateElement("issues");
            document.AppendChild(root);

            foreach (LocalIssueRecord issue in (issues ?? Enumerable.Empty<LocalIssueRecord>())
                .OrderBy(i => i.Id))
            {
                XmlElement element = document.CreateElement("issue");
                element.SetAttribute("id", issue.Id.ToString(CultureInfo.InvariantCulture));
                element.SetAttribute("status", string.IsNullOrEmpty(issue.Status) ? "Open" : issue.Status);
                element.SetAttribute("createdUtc", issue.CreatedUtc.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));
                element.SetAttribute("updatedUtc", issue.UpdatedUtc.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));

                XmlElement title = document.CreateElement("title");
                title.InnerText = issue.Title ?? "";
                element.AppendChild(title);

                XmlElement description = document.CreateElement("description");
                description.InnerText = issue.Description ?? "";
                element.AppendChild(description);

                root.AppendChild(element);
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineChars = Environment.NewLine
            };
            using (XmlWriter writer = XmlWriter.Create(file, settings))
                document.Save(writer);
        }
    }

    internal sealed class LocalSvnIssuesView : UserControl
    {
        readonly IAnkhServiceProvider _context;
        readonly LocalSvnIssuesRepository _repository;
        readonly LocalIssueStore _store;
        readonly SmartListView _list = new SmartListView();
        readonly Button _edit = new Button();
        readonly Button _toggle = new Button();
        List<LocalIssueRecord> _issues = new List<LocalIssueRecord>();

        internal LocalSvnIssuesView(
            IAnkhServiceProvider context,
            LocalSvnIssuesRepository repository,
            LocalIssueStore store)
        {
            _context = context;
            _repository = repository;
            _store = store;
            Dock = DockStyle.Fill;

            FlowLayoutPanel actions = new FlowLayoutPanel
            {
                Name = "localIssueActions",
                Dock = DockStyle.Top,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(4)
            };

            Button add = new Button
            {
                Name = "newLocalIssueButton",
                Text = "New Issue...",
                AutoSize = true
            };
            _edit.Name = "editLocalIssueButton";
            _edit.Text = "Edit...";
            _edit.AutoSize = true;
            _toggle.Name = "toggleLocalIssueButton";
            _toggle.Text = "Close";
            _toggle.AutoSize = true;
            Button refresh = new Button
            {
                Name = "refreshLocalIssuesButton",
                Text = "Refresh",
                AutoSize = true
            };

            add.Click += delegate { AddIssue(); };
            _edit.Click += delegate { EditIssue(); };
            _toggle.Click += delegate { ToggleIssue(); };
            refresh.Click += delegate { Reload(); };

            actions.Controls.Add(add);
            actions.Controls.Add(_edit);
            actions.Controls.Add(_toggle);
            actions.Controls.Add(refresh);

            SmartColumn id = new SmartColumn(_list, "ID", 65, "Id");
            id.Sorter = new LocalIssueIdComparer();
            SmartColumn status = new SmartColumn(_list, "Status", 90, "Status");
            SmartColumn title = new SmartColumn(_list, "Title", 360, "Title");
            SmartColumn updated = new SmartColumn(_list, "Updated", 145, "Updated");

            foreach (SmartColumn column in new[] { id, status, title, updated })
                _list.AllColumns.Add(column);
            _list.Columns.AddRange(new ColumnHeader[] { id, status, title, updated });
            _list.SortColumns.Add(id);
            _list.Dock = DockStyle.Fill;
            _list.MultiSelect = false;
            _list.AllowColumnReorder = true;
            _list.SelectedIndexChanged += delegate { UpdateActions(); };
            _list.DoubleClick += delegate { EditIssue(); };

            Controls.Add(_list);
            Controls.Add(actions);

            Reload();
        }

        LocalIssueRecord SelectedIssue
        {
            get
            {
                if (_list.SelectedItems.Count != 1)
                    return null;

                return _list.SelectedItems[0].Tag as LocalIssueRecord;
            }
        }

        void UpdateActions()
        {
            LocalIssueRecord issue = SelectedIssue;
            _edit.Enabled = issue != null;
            _toggle.Enabled = issue != null;
            _toggle.Text = issue != null
                && string.Equals(issue.Status, "Closed", StringComparison.OrdinalIgnoreCase)
                ? "Reopen"
                : "Close";
        }

        internal void Reload()
        {
            try
            {
                _issues = _store.Load();
                _list.BeginUpdate();
                try
                {
                    _list.Items.Clear();
                    foreach (LocalIssueRecord issue in _issues)
                    {
                        SmartListViewItem item = new SmartListViewItem(_list)
                        {
                            Tag = issue
                        };
                        item.SetValues(
                            issue.Id.ToString(CultureInfo.InvariantCulture),
                            issue.Status,
                            issue.Title,
                            issue.UpdatedUtc.Ticks == 0
                                ? ""
                                : issue.UpdatedUtc.ToLocalTime().ToString("g"));
                        _list.Items.Add(item);
                    }
                }
                finally
                {
                    _list.EndUpdate();
                }
                UpdateActions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Local SVN Issues",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        void AddIssue()
        {
            int next = _issues.Count == 0 ? 1 : _issues.Max(i => i.Id) + 1;
            LocalIssueRecord issue = new LocalIssueRecord
            {
                Id = next,
                Status = "Open",
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };

            if (!EditRecord(issue))
                return;

            _issues.Add(issue);
            SaveAndReload(issue.Id);
        }

        void EditIssue()
        {
            LocalIssueRecord selected = SelectedIssue;
            if (selected == null)
                return;

            LocalIssueRecord edited = selected.Clone();
            if (!EditRecord(edited))
                return;

            int index = _issues.FindIndex(i => i.Id == edited.Id);
            if (index >= 0)
                _issues[index] = edited;

            SaveAndReload(edited.Id);
        }

        bool EditRecord(LocalIssueRecord issue)
        {
            using (LocalIssueEditDialog dialog = new LocalIssueEditDialog(issue))
            {
                if (_context != null)
                    return dialog.ShowDialog(_context, this) == DialogResult.OK;

                // Unit-test/design-time fallback. Runtime instances are hosted
                // by Ankh and therefore have a service context.
                return ((Form)dialog).ShowDialog(this) == DialogResult.OK;
            }
        }

        void ToggleIssue()
        {
            LocalIssueRecord selected = SelectedIssue;
            if (selected == null)
                return;

            selected.Status = string.Equals(
                selected.Status,
                "Closed",
                StringComparison.OrdinalIgnoreCase)
                ? "Open"
                : "Closed";
            selected.UpdatedUtc = DateTime.UtcNow;
            SaveAndReload(selected.Id);
        }

        void SaveAndReload(int selectId)
        {
            try
            {
                _store.Save(_issues);
                Reload();
                SelectIssue(selectId.ToString(CultureInfo.InvariantCulture), false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Local SVN Issues",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        internal void SelectIssue(string issueId, bool edit)
        {
            int id;
            if (!int.TryParse(issueId, out id))
                return;

            foreach (ListViewItem item in _list.Items)
            {
                LocalIssueRecord issue = item.Tag as LocalIssueRecord;
                if (issue != null && issue.Id == id)
                {
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();

                    if (edit)
                        EditIssue();
                    return;
                }
            }
        }

        sealed class LocalIssueIdComparer : IComparer<ListViewItem>
        {
            public int Compare(ListViewItem x, ListViewItem y)
            {
                LocalIssueRecord left = x == null ? null : x.Tag as LocalIssueRecord;
                LocalIssueRecord right = y == null ? null : y.Tag as LocalIssueRecord;

                if (ReferenceEquals(left, right))
                    return 0;
                if (left == null)
                    return -1;
                if (right == null)
                    return 1;
                return left.Id.CompareTo(right.Id);
            }
        }
    }

    internal sealed class LocalIssueEditDialog : VSDialogForm
    {
        readonly LocalIssueRecord _issue;
        readonly TextBox _title = new TextBox();
        readonly ComboBox _status = new ComboBox();
        readonly TextBox _description = new TextBox();

        internal LocalIssueEditDialog(LocalIssueRecord issue)
        {
            _issue = issue;
            Text = "Issue #" + issue.Id;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            Width = 560;
            Height = 420;

            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(10)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            table.Controls.Add(new Label { Text = "Title:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            _title.Dock = DockStyle.Fill;
            table.Controls.Add(_title, 1, 0);

            table.Controls.Add(new Label { Text = "Status:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
            _status.DropDownStyle = ComboBoxStyle.DropDownList;
            _status.Items.AddRange(new object[] { "Open", "Closed" });
            _status.Dock = DockStyle.Left;
            table.Controls.Add(_status, 1, 1);

            table.Controls.Add(new Label { Text = "Description:", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top }, 0, 2);
            _description.Multiline = true;
            _description.ScrollBars = ScrollBars.Vertical;
            _description.Dock = DockStyle.Fill;
            table.Controls.Add(_description, 1, 2);

            FlowLayoutPanel buttons = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill
            };
            Button ok = new Button { Text = "OK", DialogResult = DialogResult.OK, AutoSize = true };
            Button cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, AutoSize = true };
            buttons.Controls.Add(ok);
            buttons.Controls.Add(cancel);
            table.Controls.Add(buttons, 1, 3);

            Controls.Add(table);
            AcceptButton = ok;
            CancelButton = cancel;

            _title.Text = issue.Title ?? "";
            _description.Text = issue.Description ?? "";
            _status.SelectedItem = string.Equals(issue.Status, "Closed", StringComparison.OrdinalIgnoreCase)
                ? "Closed"
                : "Open";

            ok.Click += delegate(object sender, EventArgs e)
            {
                if (string.IsNullOrWhiteSpace(_title.Text))
                {
                    MessageBox.Show(
                        this,
                        "Enter an issue title.",
                        "Local SVN Issues",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    DialogResult = DialogResult.None;
                    return;
                }

                _issue.Title = _title.Text.Trim();
                _issue.Description = _description.Text;
                _issue.Status = Convert.ToString(_status.SelectedItem, CultureInfo.InvariantCulture);
                if (_issue.CreatedUtc.Ticks == 0)
                    _issue.CreatedUtc = DateTime.UtcNow;
                _issue.UpdatedUtc = DateTime.UtcNow;
            };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // VSDialogForm themes during OnLoad, but some native child controls
            // do not have handles until the dialog is actually shown. Apply a
            // second recursive pass here so text boxes, combo boxes, buttons,
            // labels and the native caption all receive the current VS theme.
            ApplyVisibleTheme();
        }

        internal bool ApplyVisibleTheme()
        {
            return IssueTrackerThemeLogic.ThemeEmbeddedControl(
                Context,
                this,
                true);
        }

    }

    #endregion
}
