using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankh
{
    [CLSCompliant(false)]
    public struct XCastUInt32
    {
        public readonly IntPtr Value;


        public XCastUInt32(UInt32 value)
        {
            Value = (IntPtr)value;
        }

        public XCastUInt32(IntPtr value)
        {
            Value = value;
        }

        public static explicit operator XCastUInt32(uint value)
        {
            return new XCastUInt32(value);
        }

        public static explicit operator XCastUInt32(IntPtr value)
        {
            return new XCastUInt32(value);
        }

        public static implicit operator IntPtr(XCastUInt32 value)
        {
            return value.Value;
        }

        public static implicit operator uint(XCastUInt32 value)
        {
            return unchecked((uint)value.Value);
        }
    }

    public struct XCastInt32
    {
        public readonly IntPtr Value;


        public XCastInt32(int value)
        {
            Value = (IntPtr)value;
        }

        public XCastInt32(IntPtr value)
        {
            Value = value;
        }

        public static explicit operator XCastInt32(int value)
        {
            return new XCastInt32(value);
        }

        public static explicit operator XCastInt32(IntPtr value)
        {
            return new XCastInt32(value);
        }

        public static implicit operator IntPtr(XCastInt32 value)
        {
            return value.Value;
        }

        public static implicit operator int(XCastInt32 value)
        {
            return unchecked((int)value.Value);
        }
    }
}
