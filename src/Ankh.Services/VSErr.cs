using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Ankh
{
    public static class VSErr
    {
        /// <summary>
        /// Checks if a HRESULT is a success return code.
        /// </summary>
        [DebuggerStepThrough]
        public static bool Succeeded(int hr)
        {
            return (hr >= 0);
        }

        /// <summary>
        /// Checks if a HRESULT is an error return code.
        /// </summary>
        /// <param name="hr">The HRESULT to test.</param>
        /// <returns>true if hr represents an error, false otherwise.</returns>
        [DebuggerStepThrough]
        public static bool Failed(int hr)
        {
            return (hr < 0);
        }

        /// <summary>
        /// Checks if the parameter is a success or failure HRESULT and throws an exception in case
        /// of failure.
        /// </summary>
        [DebuggerStepThrough]
        public static void ThrowOnFailure(int hr)
        {
            if (VSErr.Failed(hr))
                Marshal.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Checks if the parameter is a success or failure HRESULT and throws an exception if it is a
        /// failure that is not included in the array of expected failures.
        /// </summary>
        /// <param name="hr"></param>
        /// <param name="expectedHRFailure"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static int ThrowOnFailure(int hr, params int[] expectedHRFailure)
        {
            if (VSErr.Failed(hr))
            {
                if (expectedHRFailure != null)
                    foreach (int i in expectedHRFailure)
                    {
                        if (i == hr)
                            return i;
                    }
                Marshal.ThrowExceptionForHR(hr);
            }
            return S_OK;
        }

        // VS HRESULTS

        /// <summary>VS specific error HRESULT for "Project already exists".</summary>
        public const int VS_E_PROJECTALREADYEXISTS = unchecked((int)0x80041FE0);
        /// <summary>VS specific error HRESULT for "Package not loaded".</summary>
        public const int VS_E_PACKAGENOTLOADED = unchecked((int)0x80041FE1);
        /// <summary>VS specific error HRESULT for "Project not loaded".</summary>
        public const int VS_E_PROJECTNOTLOADED = unchecked((int)0x80041FE2);
        /// <summary>VS specific error HRESULT for "Solution not open".</summary>
        public const int VS_E_SOLUTIONNOTOPEN = unchecked((int)0x80041FE3);
        /// <summary>VS specific error HRESULT for "Solution already open".</summary>
        public const int VS_E_SOLUTIONALREADYOPEN = unchecked((int)0x80041FE4);
        /// <summary>VS specific error HRESULT for "Project configuration failed".</summary>
        public const int VS_E_PROJECTMIGRATIONFAILED = unchecked((int)0x80041FE5);
        /// <summary>VS specific error HRESULT for "Incompatible document data".</summary>
        public const int VS_E_INCOMPATIBLEDOCDATA = unchecked((int)0x80041FEA);
        /// <summary>VS specific error HRESULT for "Unsupported format".</summary>
        public const int VS_E_UNSUPPORTEDFORMAT = unchecked((int)0x80041FEB);
        /// <summary>VS specific error HRESULT for "Wizard back button pressed".</summary>
        public const int VS_E_WIZARDBACKBUTTONPRESS = unchecked((int)0x80041fff);
        /// <summary>VS specific success HRESULT for "Project forwarded".</summary>
        public const int VS_S_PROJECTFORWARDED = unchecked((int)0x41ff0);
        /// <summary>VS specific success HRESULT for "Toolbox marker".</summary>
        public const int VS_S_TBXMARKER = unchecked((int)0x41ff1);


        // OLE HRESULTS - may be returned by OLE or related VS methods
        public const int
        OLE_E_OLEVERB = unchecked((int)0x80040000),
        OLE_E_ADVF = unchecked((int)0x80040001),
        OLE_E_ENUM_NOMORE = unchecked((int)0x80040002),
        OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003),
        OLE_E_NOCONNECTION = unchecked((int)0x80040004),
        OLE_E_NOTRUNNING = unchecked((int)0x80040005),
        OLE_E_NOCACHE = unchecked((int)0x80040006),
        OLE_E_BLANK = unchecked((int)0x80040007),
        OLE_E_CLASSDIFF = unchecked((int)0x80040008),
        OLE_E_CANT_GETMONIKER = unchecked((int)0x80040009),
        OLE_E_CANT_BINDTOSOURCE = unchecked((int)0x8004000A),
        OLE_E_STATIC = unchecked((int)0x8004000B),
        OLE_E_PROMPTSAVECANCELLED = unchecked((int)0x8004000C),
        OLE_E_INVALIDRECT = unchecked((int)0x8004000D),
        OLE_E_WRONGCOMPOBJ = unchecked((int)0x8004000E),
        OLE_E_INVALIDHWND = unchecked((int)0x8004000F),
        OLE_E_NOT_INPLACEACTIVE = unchecked((int)0x80040010),
        OLE_E_CANTCONVERT = unchecked((int)0x80040011),
        OLE_E_NOSTORAGE = unchecked((int)0x80040012);

        public const int
        OLECMDERR_E_FIRST = unchecked((int)0x80040100),
        OLECMDERR_E_NOTSUPPORTED = unchecked((int)0x80040100),
        OLECMDERR_E_DISABLED = unchecked((int)0x80040101),
        OLECMDERR_E_NOHELP = unchecked((int)0x80040102),
        OLECMDERR_E_CANCELED = unchecked((int)0x80040103),
        OLECMDERR_E_UNKNOWNGROUP = unchecked((int)0x80040102);

        // OLE DISP HRESULTS - may be returned by OLE DISP or related VS methods 

        public const int
        DISP_E_UNKNOWNINTERFACE = unchecked((int)0x80020001),
        DISP_E_MEMBERNOTFOUND = unchecked((int)0x80020003),
        DISP_E_PARAMNOTFOUND = unchecked((int)0x80020004),
        DISP_E_TYPEMISMATCH = unchecked((int)0x80020005),
        DISP_E_UNKNOWNNAME = unchecked((int)0x80020006),
        DISP_E_NONAMEDARGS = unchecked((int)0x80020007),
        DISP_E_BADVARTYPE = unchecked((int)0x80020008),
        DISP_E_EXCEPTION = unchecked((int)0x80020009),
        DISP_E_OVERFLOW = unchecked((int)0x8002000A),
        DISP_E_BADINDEX = unchecked((int)0x8002000B),
        DISP_E_UNKNOWNLCID = unchecked((int)0x8002000C),
        DISP_E_ARRAYISLOCKED = unchecked((int)0x8002000D),
        DISP_E_BADPARAMCOUNT = unchecked((int)0x8002000E),
        DISP_E_PARAMNOTOPTIONAL = unchecked((int)0x8002000F),
        DISP_E_BADCALLEE = unchecked((int)0x80020010),
        DISP_E_NOTACOLLECTION = unchecked((int)0x80020011),
        DISP_E_DIVBYZERO = unchecked((int)0x80020012),
        DISP_E_BUFFERTOOSMALL = unchecked((int)0x80020013);


        /// <summary>HRESULT for FALSE (not an error).</summary>
        public const int S_FALSE = 0x00000001;
        /// <summary>HRESULT for generic success.</summary>
        public const int S_OK = 0x00000000;
        /// <summary>Error HRESULT for a client abort.</summary>
        public const int UNDO_E_CLIENTABORT = unchecked((int)0x80044001);
        /// <summary>Error HRESULT for out of memory.</summary>
        public const int E_OUTOFMEMORY = unchecked((int)0x8007000E);
        /// <summary>Error HRESULT for an invalid argument.</summary>
        public const int E_INVALIDARG = unchecked((int)0x80070057);
        /// <summary>Error HRESULT for a generic failure.</summary>
        public const int E_FAIL = unchecked((int)0x80004005);
        /// <summary>Error HRESULT for the request of a not implemented interface.</summary>
        public const int E_NOINTERFACE = unchecked((int)0x80004002);
        /// <summary>Error HRESULT for the call to a not implemented method.</summary>
        public const int E_NOTIMPL = unchecked((int)0x80004001);
        /// <summary>Error HRESULT for an unexpected condition.</summary>
        public const int E_UNEXPECTED = unchecked((int)0x8000FFFF);
        /// <summary>Error HRESULT for a null or invalid pointer.</summary>
        public const int E_POINTER = unchecked((int)0x80004003);
        /// <summary>Error HRESULT for an invalid HANDLE.</summary>
        public const int E_HANDLE = unchecked((int)0x80070006);
        /// <summary>Error HRESULT for an abort.</summary>
        public const int E_ABORT = unchecked((int)0x80004004);
        /// <summary>Error HRESULT for an access denied.</summary>
        public const int E_ACCESSDENIED = unchecked((int)0x80070005);
        /// <summary>Error HRESULT for a pending condition.</summary>
        public const int E_PENDING = unchecked((int)0x8000000A);
    }
}
