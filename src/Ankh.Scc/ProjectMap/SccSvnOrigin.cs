using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc.ProjectMap
{
    public class SccSvnOrigin
    {
        readonly Dictionary<string, string> _custom = new Dictionary<string, string>();

        const string KeysName = "@Keys";
        const string EnlistName = "Enlist";
        const string UriName = "Svn.Uri";
        const string ProjectSuffix = "Svn.ProjectSuffix";
        const string GroupName = "Svn.Group";
        const string RelationName = "Svn.Relation";

        // This list contains the properties every AnkhSVN that understands SvnOrigin knows
        readonly string[] InitialProperties = { EnlistName, UriName, ProjectSuffix, GroupName, RelationName }; 
        // And this contains the list of properties that aren't custom
        readonly string[] HandledProperties = { EnlistName, UriName, ProjectSuffix, GroupName, RelationName };

        static bool InList(string needle, IEnumerable<string> haystack)
        {
            foreach(string k in haystack)
            {
                if (k == needle)
                    return true;
            }
            return false;
        }

        public void Load(IPropertyMap map)
        {
            string value;
            string properties;

            _custom.Clear();
            if (map.TryGetValue(KeysName, out properties))
            {
                foreach (string nm in properties.Split(','))
                {
                    string name = nm.Trim();

                    if (string.IsNullOrEmpty(name))
                        continue;

                    if (InList(name, HandledProperties))
                        continue;

                    if (map.TryGetValue(name, out value))
                        _custom[name] = value;
                }
            }

            if (map.TryGetValue(EnlistName, out value))
                Enlist = value;
            else
                Enlist = null;

            if (map.TryGetValue(UriName, out value))
                SvnUri = value;
            else
                SvnUri = null;

            if (map.TryGetValue(ProjectSuffix, out value))
                SvnSuffix = value;
            else
                SvnSuffix = null;

            if (map.TryGetValue(GroupName, out value))
                Group = value;
            else
                Group = null;

            if (map.TryGetValue(RelationName, out value))
                Relation = value;
            else
                Relation = null;
        }

        public void Write(IPropertyMap map)
        {
            if (!string.IsNullOrEmpty(Enlist))
                map.SetValue(EnlistName, Enlist);

            if (!string.IsNullOrEmpty(SvnUri))
                map.SetValue(UriName, SvnUri);

            if (!string.IsNullOrEmpty(SvnSuffix))
                map.SetValue(ProjectSuffix, SvnSuffix);

            if (!string.IsNullOrEmpty(Group))
                map.SetValue(GroupName, Group);

            if (!string.IsNullOrEmpty(Relation))
                map.SetValue(RelationName, Relation);

            StringBuilder sb = null;

            foreach (KeyValuePair<string, string> kv in _custom)
            {
                if (map.WrittenKey(kv.Key))
                    continue;

                if (InList(kv.Key, InitialProperties))
                    continue;

                map.SetValue(kv.Key, kv.Value);

                if (sb == null)
                    sb = new StringBuilder();
                else
                    sb.Append(',');

                sb.Append(kv.Key);
            }

            if (sb != null)
                map.SetValue(KeysName, sb.ToString());
        }

        string TrimNull(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            string v = value.Trim();

            if (string.IsNullOrEmpty(v))
                return null;

            return v;
        }

        string _enlist;
        public string Enlist
        {
            get { return _enlist; }
            set { _enlist = TrimNull(value); }
        }

        string _svnUri;
        public string SvnUri
        {
            get { return _svnUri; }
            set { _svnUri = TrimNull(value); }
        }

        string _svnSuffix;
        public string SvnSuffix
        {
            get { return _svnSuffix; }
            set { _svnSuffix = TrimNull(value); }
        }

        string _group;
        public string Group
        {
            get { return _group; }
            set { _group = TrimNull(value); }
        }

        string _relation;
        public string Relation
        {
            get { return _relation; }
            set { _relation = TrimNull(value); }
        }
    }
}
