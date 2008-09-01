using System;

namespace NSvn.Common
{
    /// <summary>
    /// This assembly attribute describes the version of Subversion an assembly has been built against.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class SubversionVersionAttribute : Attribute
    {
        public SubversionVersionAttribute() : this( 0, 0, 0 )
        {
            // nothing to see here            
        }

        public SubversionVersionAttribute( int major, int minor, int patchLevel )
        {
            this.major = major;
            this.minor = minor;
            this.patchLevel = patchLevel;
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
            return String.Format( "{0}.{1}.{2}", this.major, this.minor, this.patchLevel );
        }


        private int major;
        private int minor;
        private int patchLevel;
    }
}
