using System;

namespace Ankh
{
    public enum VSInstance
    {
        None = 0,
        VS2005, //  8.0
        VS2008, //  9.0
        VS2010, // 10.0
        VS2012, // 11.0
        VS2013, // 12.0
        No_VS13,
        VS2015, // 14.0
        VS2017, // 15.0
        VS2019, // 16.0
        VS2022  // 17.0
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
