// $Id$
using System;

namespace NSvn.Common
{
    /// <summary>
    /// An attribute that describes a library linked into NSvn.Core.
    /// </summary>
    [AttributeUsage( AttributeTargets.Assembly, AllowMultiple=true )]
    public class LibraryAttribute : Attribute
    {
        public LibraryAttribute() : this( "", 0, 0, 0 )
        {
            // empty
        }

        public LibraryAttribute( string name, int major, int minor, int patchLevel )
        {
            this.name = name;
            this.major = major;
            this.minor = minor;
            this.patchLevel = patchLevel;
        }

        public string Name
        {
            get{ return this.name; }
            set{ this.name = value; }
        }

        public int Major
        {
            get{ return this.major; }
            set{ this.major = major; }
        }

        public int Minor
        {
            get{ return this.major; }
            set{ this.major = major; }
        }

        public int PatchLevel
        {
            get{ return this.patchLevel; }
            set{ this.patchLevel = patchLevel; }
        }

        public override string ToString()
        {
            return String.Format( "{0} {1}.{2}.{3}", this.name, this.major, this.minor, this.patchLevel );
        }


        private string name;
        private int major;
        private int minor;
        private int patchLevel;
    }
}
