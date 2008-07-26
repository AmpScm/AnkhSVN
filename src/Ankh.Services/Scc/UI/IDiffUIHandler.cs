using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Ankh.Scc.UI
{
    public class UIDiffArgs 
    {
        string _leftTitle;
        string _rightTitle;

        /// <summary>
        /// Gets or sets the left title.
        /// </summary>
        /// <value>The left title.</value>
        public string LeftTitle
        {
            get { return _leftTitle; }
            set { _leftTitle = value; }
        }

        /// <summary>
        /// Gets or sets the right title.
        /// </summary>
        /// <value>The right title.</value>
        public string RightTitle
        {
            get { return _rightTitle; }
            set { _rightTitle = value; }
        }
    }

    public class UIMergeArgs : UIDiffArgs
    {
        string _originTitle;

        /// <summary>
        /// Gets or sets the origin title.
        /// </summary>
        /// <value>The origin title.</value>
        public string OriginTitle
        {
            get { return _originTitle; }
            set { _originTitle = value; }
        }
    }

    interface IDiffUIHandler
    {
        bool RunDiff(string left, string right, UIDiffArgs args);
        bool RunMerge(string left, string right, string origin, UIMergeArgs args);
    }
}
