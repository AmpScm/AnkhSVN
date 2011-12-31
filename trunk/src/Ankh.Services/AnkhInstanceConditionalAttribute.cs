using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ankh
{
    public enum VSInstance
    {
        None = 0,
        VS2005,
        VS2008,
        VS2010,
        VS11
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
                switch (MinVersion)
                {
                    case VSInstance.VS2005:
                        break;
                    case VSInstance.VS2008:
                        if (VSVersion.VS2005)
                            return false;
                        break;
                    case VSInstance.VS2010:
                        if (!VSVersion.VS2010OrLater)
                            return false;
                        break;
                    case VSInstance.VS11:
                        if (!VSVersion.VS11OrLater)
                            return false;
                        break;
                    default:
                        break;
                }
            }

            if (MaxVersion != VSInstance.None)
            {
                switch (MaxVersion)
                {
                    case VSInstance.VS2005:
                        if (!VSVersion.VS2005)
                            return false;
                        break;
                    case VSInstance.VS2008:
                        if (VSVersion.VS2010OrLater)
                            return false;
                        break;
                    case VSInstance.VS2010:
                        if (VSVersion.VS11OrLater)
                            return false;
                        break;
                    case VSInstance.VS11:
                        break;
                    default:
                        break;
                }
            }

            return true;
        }
    }
}
