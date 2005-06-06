// $Id$
using System;

namespace Utils
{
    /// <summary>
    /// An attribute that describes a library
    /// </summary>
    [AttributeUsage( AttributeTargets.Assembly, AllowMultiple=true )]
    public class VersionAttribute : Attribute
    {
        public VersionAttribute() : this( "", 0, 0, 0 )
        {
            // empty
        }

        public VersionAttribute( string name, int major, int minor, int patchLevel )
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
            set{ this.major = value; }
        }

        public int Minor
        {
            get{ return this.minor; }
            set{ this.major = value; }
        }

        public int PatchLevel
        {
            get{ return this.patchLevel; }
            set{ this.patchLevel = value; }
        }

        public string Tag
        {
            get{ return this.tag; }
            set{ this.tag = value; }
        }

        public string CustomText
        {
            get{ return this.customText; }
            set{ this.customText = value; }
        }

        public override string ToString()
        {
            if ( this.CustomText != null )
                return String.Format( "{0}{1}", this.Name, this.CustomText );
            else
                return String.Format( "{0} {1}.{2}.{3}{4}", this.name, 
                    this.major, this.minor, this.patchLevel, this.tag );
        }


        private string customText = null;
        private string name;
        private int major;
        private int minor;
        private int patchLevel;
        private string tag = "";
    }
}
