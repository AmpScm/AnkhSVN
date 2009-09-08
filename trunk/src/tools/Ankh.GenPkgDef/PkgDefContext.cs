using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.IO;

namespace Ankh.GenPkgDef
{
    partial class PkgDefContext : RegistrationAttribute.RegistrationContext, IDisposable
    {
        readonly TextWriter _writer;
        readonly Type _componentType;

        public PkgDefContext(TextWriter tw, Type type)
        {
            _writer = tw;
            _componentType = type;
        }

        public override string CodeBase
        {
            get { return ""; }
        }

        public override string ComponentPath
        {
            get { return ""; }
        }

        public override Type ComponentType
        {
            get { return _componentType; }
        }

        public override RegistrationAttribute.Key CreateKey(string name)
        {
            return new PkgDefKey(this, name, null);
        }

        public override string EscapePath(string str)
        {
            return str;
        }

        public override string InprocServerPath
        {
            get { return "%windir%\\system32\\mscoree.dll"; }
        }

        StringWriter _sw;
        public override System.IO.TextWriter Log
        {
            get { return _sw ?? (_sw = new StringWriter()); }
        }

        public override Microsoft.VisualStudio.Shell.RegistrationMethod RegistrationMethod
        {
            get { return RegistrationMethod.Default; }
        }

        public override void RemoveKey(string name)
        {
            throw new NotImplementedException();
        }

        public override void RemoveKeyIfEmpty(string name)
        {
            throw new NotImplementedException();
        }

        public override void RemoveValue(string keyname, string valuename)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion

        internal TextWriter Tw
        {
            get { return _writer; }
        }

        static string EscapeString(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("\"", "\\\\");
        }

        internal string FormatValue(object value)
        {
            if (value is string)
                return "\"" + EscapeString((string)value) + "\"";
            else if (value is int || value is uint || value is short || value is ushort)
                return string.Format("dword:{0:X8}", value);
            else
                return FormatValue(value.ToString());
        }

        class PkgDefKey : RegistrationAttribute.Key
        {
            readonly PkgDefContext _context;
            string _name;
            PkgDefKey _parent;
            PkgDefKey _open;
            bool _wasOpen;
            public PkgDefKey(PkgDefContext context, string name, PkgDefKey parent)
            {
                if (parent != null)
                {
                    _parent = parent;
                    _parent._open = parent;
                }

                _context = context;
                _name = name;
                context._writer.WriteLine(string.Format("[$RootKey$\\{0}]", name));
            }

            internal TextWriter Tw
            {
                get { return _context.Tw; }
            }

            public override void Close()
            {
                if(_parent != null)
                    _parent._open = null;
            }

            public override RegistrationAttribute.Key CreateSubkey(string name)
            {
                if (_wasOpen)
                    throw new InvalidOperationException();

                _wasOpen = true;
                return new PkgDefKey(_context, _name + "\\" + name, this);
            }

            public override void SetValue(string valueName, object value)
            {
                if (_wasOpen)
                    throw new InvalidOperationException();
                string name = string.IsNullOrEmpty(valueName) ? "@" : FormatValue(valueName);
                Tw.WriteLine(string.Format("{0}={1}", name, FormatValue(value)));
            }

            private string FormatValue(object value)
            {
                return _context.FormatValue(value);
            }
        }

        
    }
}
