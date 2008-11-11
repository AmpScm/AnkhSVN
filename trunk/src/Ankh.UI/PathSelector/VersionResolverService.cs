using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc.UI;
using System.Collections;
using SharpSvn;
using Ankh.Scc;

namespace Ankh.UI.PathSelector
{
    [GlobalService(typeof(IAnkhRevisionResolver))]
    sealed class VersionResolverService : AnkhService, IAnkhRevisionResolver
    {
        readonly List<IAnkhRevisionProvider> _providers = new List<IAnkhRevisionProvider>();

        public VersionResolverService(IAnkhServiceProvider context)
            : base(context)
        {
            _providers.Add(new StandardVersionResolverService(this));
        }

        #region IAnkhRevisionResolver Members

        /// <summary>
        /// Registers the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        public void RegisterExtension(IAnkhRevisionProvider extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            _providers.Add(extension);
        }

        #endregion

        #region IAnkhRevisionProvider Members

        /// <summary>
        /// Gets a list of AnkhRevisions for the specified origin
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public IEnumerable<AnkhRevisionType> GetRevisionTypes(Ankh.Scc.SvnOrigin origin)
        {
            Hashtable ht = new Hashtable();
            foreach (IAnkhRevisionProvider p in _providers)
            {
                foreach (AnkhRevisionType rt in p.GetRevisionTypes(origin))
                {
                    if (!ht.Contains(rt))
                        yield return rt;

                    ht.Add(rt, rt.UniqueName);
                }
            }
        }

        /// <summary>
        /// Resolves the specified revision.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="revision">The revision.</param>
        /// <returns></returns>
        public AnkhRevisionType Resolve(SvnOrigin origin, SharpSvn.SvnRevision revision)
        {
            if (revision == null)
                throw new ArgumentNullException("revision");

            foreach (IAnkhRevisionProvider p in _providers)
            {
                AnkhRevisionType r = p.Resolve(origin, revision);

                if (r != null)
                    return r;
            }

            switch (revision.RevisionType)
            {
                case SvnRevisionType.Number:
                    ExplicitRevisionType ert = new ExplicitRevisionType(Context, origin);
                    ert.CurrentValue = revision;
                    return ert;
            }

            return null;
        }

        public AnkhRevisionType Resolve(SvnOrigin origin, AnkhRevisionType revision)
        {
            if (revision == null)
                throw new ArgumentNullException("revision");

            foreach (IAnkhRevisionProvider p in _providers)
            {
                AnkhRevisionType r = p.Resolve(origin, revision);

                if (r != null)
                    return r;
            }

            return Resolve(origin, revision.CurrentValue);
        }

        #endregion

        sealed class StandardVersionResolverService : AnkhService, IAnkhRevisionProvider
        {
            static readonly SimpleRevisionType _head = new SimpleRevisionType(SvnRevision.Head, VersionStrings.HeadVersion);
            static readonly SimpleRevisionType _working = new SimpleRevisionType(SvnRevision.Working, VersionStrings.WorkingVersion);
            static readonly SimpleRevisionType _base = new SimpleRevisionType(SvnRevision.Base, VersionStrings.BaseVersion);
            static readonly SimpleRevisionType _committed = new SimpleRevisionType(SvnRevision.Committed, VersionStrings.CommittedVersion);
            static readonly SimpleRevisionType _previous = new SimpleRevisionType(SvnRevision.Previous, VersionStrings.PreviousVersion);

            public StandardVersionResolverService(VersionResolverService context)
                : base(context)
            {

            }
            #region IAnkhRevisionProvider Members

            public IEnumerable<AnkhRevisionType> GetRevisionTypes(Ankh.Scc.SvnOrigin origin)
            {
                if (origin == null)
                    throw new ArgumentNullException("origin");

                SvnPathTarget pt = origin.Target as SvnPathTarget;
                bool isPath = (pt != null);

                yield return _head;

                if (isPath)
                {
                    SvnItem item = GetService<IFileStatusCache>()[pt.FullPath];

                    if (item.IsVersioned)
                    {
                        yield return _working;
                        yield return _base;
                    }
                    if (item.HasCopyableHistory)
                    {
                        yield return _committed;
                        yield return _previous;
                    }
                    else
                        yield break;
                }

                yield return new DateRevisionType(this, origin);
                yield return new ExplicitRevisionType(this, origin);
            }

            public AnkhRevisionType Resolve(SvnOrigin origin, SharpSvn.SvnRevision revision)
            {
                if (revision == null)
                    throw new ArgumentNullException("revision");

                switch (revision.RevisionType)
                {
                    case SvnRevisionType.Head:
                        return _head;
                    case SvnRevisionType.Base:
                        return _base;
                    case SvnRevisionType.Committed:
                        return _committed;
                    case SvnRevisionType.Previous:
                        return _previous;
                    case SvnRevisionType.Working:
                        return _working;
                }

                return null;
            }

            public AnkhRevisionType Resolve(SvnOrigin origin, AnkhRevisionType revision)
            {
                return Resolve(origin, revision.CurrentValue);
            }

            #endregion

            sealed class SimpleRevisionType : AnkhRevisionType
            {
                readonly SvnRevision _rev;
                readonly string _title;
                public SimpleRevisionType(SvnRevision rev, string title)
                {
                    if (rev == null)
                        throw new ArgumentNullException("rev");
                    else if (string.IsNullOrEmpty(title))
                        throw new ArgumentNullException("title");

                    _rev = rev;
                    _title = title;
                }

                /// <summary>
                /// Gets the current value.
                /// </summary>
                /// <value>The current value.</value>
                public override SvnRevision CurrentValue
                {
                    get { return _rev; }
                    set { throw new InvalidOperationException(); }
                }

                /// <summary>
                /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
                /// </summary>
                /// <returns>
                /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
                /// </returns>
                public override string ToString()
                {
                    return _title;
                }

                /// <summary>
                /// Gets a unique name for this revision type
                /// </summary>
                /// <value>The unique typename</value>
                public override string UniqueName
                {
                    get
                    {
                        return _rev.RevisionType.ToString();
                    }
                }
            }
        }
        sealed class DateRevisionType : AnkhRevisionType
        {
            readonly SvnOrigin _origin;
            readonly IAnkhServiceProvider _context;
            DateTime _date;

            public DateRevisionType(IAnkhServiceProvider context, SvnOrigin origin)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                else if (origin == null)
                    throw new ArgumentNullException("origin");

                _context = context;
                _origin = origin;
                _date = DateTime.Today;
            }

            public override SvnRevision CurrentValue
            {
                get { return _date != DateTime.MinValue ? new SvnRevision(_date) : null; }
                set
                {
                    if (value == null || value.RevisionType != SvnRevisionType.Time)
                        _date = DateTime.MinValue;
                    else
                        _date = value.Time;
                }
            }

            public override string ToString()
            {
                return VersionStrings.DateVersion;
            }

            public override bool HasUI
            {
                get
                {
                    return true;
                }
            }

            DateSelector _sel;

            public override System.Windows.Forms.Control InstantiateUIIn(System.Windows.Forms.Panel parentPanel, EventArgs e)
            {
                if (_sel != null)
                    throw new InvalidOperationException();

                _sel = new DateSelector();
                parentPanel.Controls.Add(_sel);
                _sel.Dock = System.Windows.Forms.DockStyle.Fill;
                _sel.Changed += new EventHandler(OnSelChanged);
                _sel.Value = _date;

                return _sel;
            }

            void OnSelChanged(object sender, EventArgs e)
            {
                if(_sel != null)
                    _date = _sel.Value;
            }

            public override System.Windows.Forms.Control CurrentControl
            {
                get
                {
                    return _sel;
                }
            }
        }

        sealed class ExplicitRevisionType : AnkhRevisionType
        {
            readonly SvnOrigin _origin;
            readonly IAnkhServiceProvider _context;
            long _rev;

            public ExplicitRevisionType(IAnkhServiceProvider context, SvnOrigin origin)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                else if (origin == null)
                    throw new ArgumentNullException("origin");

                _context = context;
                _origin = origin;
            }

            public override SvnRevision CurrentValue
            {
                get { return _rev >= 0 ? new SvnRevision(_rev) : null; }
                set
                {
                    if (value == null || value.RevisionType != SvnRevisionType.Number)
                        _rev = -1;
                    else
                        _rev = value.Revision;
                }
            }

            public override string ToString()
            {
                return VersionStrings.ExplicitVersion;
            }

            public override bool HasUI
            {
                get
                {
                    return true;
                }
            }

            RevisionSelector _sel;

            public override System.Windows.Forms.Control InstantiateUIIn(System.Windows.Forms.Panel parentPanel, EventArgs e)
            {
                if (_sel != null)
                    throw new InvalidOperationException();

                _sel = new RevisionSelector();
                _sel.Context = _context;
                _sel.SvnOrigin = _origin;
                parentPanel.Controls.Add(_sel);
                _sel.Dock = System.Windows.Forms.DockStyle.Fill;
                _sel.Changed += new EventHandler(OnVersionChanged);
                _sel.Revision = _rev;

                return _sel;
            }

            void OnVersionChanged(object sender, EventArgs e)
            {
                if (_sel != null)
                {
                    long? value = _sel.Revision;

                    if (value.HasValue)
                        _rev = value.Value;
                }
            }

            public override System.Windows.Forms.Control CurrentControl
            {
                get
                {
                    return _sel;
                }
            }
        }

    }
}
