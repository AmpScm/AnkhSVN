using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.Scc.UI
{
    [Flags]
    public enum DiffMode
    {
        Default=0,
        PreferExternal=1,
        PreferInternal=2
    }

    public class AnkhDiffArgs 
    {
        DiffMode _diffMode;
        string _baseFile;
        string _baseTitle;

        string _mineFile;
        string _mineTitle;

        public DiffMode Mode
        {
            get { return _diffMode; }
            set { _diffMode = value; }
        }

        public string BaseFile
        {
            get { return _baseFile; }
            set { _baseFile = value; }
        }

        public string MineFile
        {
            get { return _mineFile; }
            set { _mineFile = value; }
        }

        public string BaseTitle
        {
            get { return _baseTitle; }
            set { _baseTitle = value; }
        }

        public string MineTitle
        {
            get { return _mineTitle; }
            set { _mineTitle = value; }
        }

        public virtual bool Validate()
        {
            return !string.IsNullOrEmpty(BaseFile) && !string.IsNullOrEmpty(MineFile);
        }


    }

    public class AnkhMergeArgs : AnkhDiffArgs
    {
        string _theirsFile;
        string _theirsTitle;
        string _mergedFile;
        string _mergedTitle;

        public string TheirsFile
        {
            get { return _theirsFile; }
            set { _theirsFile = value; }
        }

        public string TheirsTitle
        {
            get { return _theirsTitle; }
            set { _theirsTitle = value; }
        }
        
        public string MergedFile
        {
            get { return _mergedFile; }
            set { _mergedFile = value; }
        }

        public string MergedTitle
        {
            get { return _mergedTitle; }
            set { _mergedTitle = value; }
        }

        public override bool Validate()
        {
            return base.Validate() && !string.IsNullOrEmpty(TheirsFile) && !string.IsNullOrEmpty(MergedFile);
        }
    }

    public interface IAnkhDiffHandler
    {
        bool RunDiff(AnkhDiffArgs args);
        bool RunMerge(AnkhMergeArgs args);

        void ReleaseDiff(int frameNumber);

        string GetTempFile(SvnTarget target, SvnRevision revision, bool withProgress);
        string GetTempFile(SvnItem target, SvnRevision revision, bool withProgress);
        string GetTitle(SvnTarget target, SvnRevision revision);
        string GetTitle(SvnItem target, SvnRevision revision);

        SvnUriTarget GetCopyOrigin(SvnItem item);


    }
}
