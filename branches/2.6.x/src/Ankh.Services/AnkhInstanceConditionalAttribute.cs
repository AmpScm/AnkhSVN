using System;

namespace Ankh
{
    public enum VSInstance
    {
        None = 0,
        VS2005,
        VS2008,
        VS2010,
        VS2012,
        VS2013,
    }

    public abstract class AnkhInstanceConditionalAttribute : Attribute
    {
        VSInstance _minVersion;
        VSInstance _maxVersion;

        protected AnkhInstanceConditionalAttribute()
        {
        }

        public VSInstance MinVersion
        {
            get { return _minVersion; }
            set { _minVersion = value; }
        }

        public VSInstance MaxVersion
        {
            get { return _maxVersion; }
            set { _maxVersion = value; }
        }

        public virtual bool Applies()
        {
            if (MinVersion != VSInstance.None)
            {
                // Calculate enum value. VS2005 = VS 8.0
                VSInstance actualVersion = (VSInstance)(VSVersion.FullVersion.Major - 8 + (int)VSInstance.VS2005);

                if (MinVersion > actualVersion)
                    return false;
            }

            if (MaxVersion != VSInstance.None)
            {
                // Calculate enum value. VS2005 = VS 8.0
                VSInstance actualVersion = (VSInstance)(VSVersion.FullVersion.Major - 8 + (int)VSInstance.VS2005);

                if (MaxVersion < actualVersion)
                    return false;
            }

            return true;
        }
    }
}
