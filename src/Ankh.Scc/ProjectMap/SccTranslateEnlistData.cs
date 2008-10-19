using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Ankh.Scc.ProjectMap
{
    class SccTranslateEnlistData : SccTranslateData
    {
        public SccTranslateEnlistData(AnkhSccProvider provider, Guid projectId)
            : base(provider, projectId)
        {
        }
        enum SccEnlistMode
        {
            /// <summary>
            /// This project does not use enlist support
            /// </summary>
            None = 0,

            /// <summary>
            /// This project does not use enlist support but is not in the
            /// same tree as the solution; AnkhSVN stores extra information to track
            /// the reference.
            /// </summary>
            SvnStateOnly,

            /// <summary>
            /// The project requires Scc enlistment
            /// </summary>
            SccEnlistCompulsory,

            /// <summary>
            /// The project allows Scc enlistment
            /// </summary>
            SccEnlistOptional
        }

        class SccEnlistData
        {
            readonly IAnkhServiceProvider _context;
            readonly Guid _projectId;
            string _uniqueName;
            string _displayPath;
            string _uncPath;
            SccEnlistMode _enlistMode;

            public SccEnlistData(IAnkhServiceProvider context, Guid projectId)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                _context = context;
                _projectId = projectId;
            }

            public Guid ProjectGuid
            {
                get { return _projectId; }
            }

            public SccEnlistMode EnlistMode
            {
                get { return _enlistMode; }
                internal set { _enlistMode = value; }
            }

            /// <summary>
            /// Gets or sets the name of the project as stored in the solution file
            /// </summary>
            public string UniqueName
            {
                get { return _uniqueName; }
                internal set { _uniqueName = value; }
            }

            /// <summary>
            /// Gets or sets the display path of the Enlist data
            /// </summary>
            /// <remarks>In most implementations the same as the Unc path</remarks>
            public string DisplayPath
            {
                get { return _displayPath; }
                internal set { _displayPath = value; }
            }

            /// <summary>
            /// Gets or sets the real path used to access the project on disk
            /// </summary>
            public string UncPath
            {
                get { return _uncPath; }
                internal set { _uncPath = value; }
            }

            internal sealed class EnlistDataCollection : KeyedCollection<string, SccEnlistData>
            {
                public EnlistDataCollection()
                    : base(StringComparer.OrdinalIgnoreCase)
                {
                }

                protected override string GetKeyForItem(SccEnlistData item)
                {
                    return item.ProjectGuid.ToString("B");
                }
            }

            internal void LoadUserData(List<string> values)
            {
                throw new NotImplementedException();
            }

            internal bool ShouldSerialize()
            {
                throw new NotImplementedException();
            }

            internal string[] GetUserData()
            {
                return new string[0];
            }

            internal bool HasPaths()
            {
                throw new NotImplementedException();
            }
        }


    }
}
