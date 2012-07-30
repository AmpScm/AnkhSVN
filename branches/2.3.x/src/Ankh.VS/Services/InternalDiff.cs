using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.Scc.UI;
using System.IO;
using SharpSvn;

namespace Ankh.VS.Services
{
    [GlobalService(typeof(IAnkhInternalDiff), AllowPreRegistered = true, MinVersion = VSInstance.VS11)]
    sealed class InternalDiff : AnkhService, IAnkhInternalDiff
    {
        public InternalDiff(IAnkhServiceProvider context)
            : base(context)
        {
        }

        #region Visual Studio 11 builtin diff
        bool _triedFindDiff;
        Type _type_IVsDifferenceService;
        object _vsDifferenceService;

        public bool HasDiff
        {
            get
            {
                if (!_triedFindDiff)
                {
                    _triedFindDiff = true;
                    _type_IVsDifferenceService = Type.GetType("Microsoft.VisualStudio.Shell.Interop.IVsDifferenceService, Microsoft.VisualStudio.Shell.Interop.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);
                    if (_type_IVsDifferenceService == null)
                        return false;

                    Type type_SVsDifferenceService = Type.GetType("Microsoft.VisualStudio.Shell.Interop.SVsDifferenceService, Microsoft.VisualStudio.Shell.Interop.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);

                    if (type_SVsDifferenceService == null)
                        return false;

                    object service = GetService(type_SVsDifferenceService);
                    if (service == null || (!_type_IVsDifferenceService.IsInstanceOfType(service) && !Marshal.IsComObject(service)))
                        return false;

                    _vsDifferenceService = service;
                }

                return (_vsDifferenceService != null);
            }
        }

        string _tempPath;
        string TempPath
        {
            get { return _tempPath ?? (_tempPath = SvnTools.GetNormalizedFullPath(Path.GetTempPath())); }
        }

        delegate IVsWindowFrame OpenComparisonWindow2([In] string leftFileMoniker, [In] string rightFileMoniker, [In] string caption, [In] string Tooltip, [In] string leftLabel, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string rightLabel, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string inlineLabel, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string roles, [In] uint grfDiffOptions);
        OpenComparisonWindow2 _ocw2;
        public bool RunDiff(AnkhDiffArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            else if (!HasDiff)
                throw new InvalidOperationException();

            if (_ocw2 == null)
                _ocw2 = GetInterfaceDelegate<OpenComparisonWindow2>(_type_IVsDifferenceService, _vsDifferenceService);

            if (_ocw2 == null)
                return false;

            uint flags = 0;

            if (TempPath != null)
            {
                if (SvnItem.IsBelowRoot(args.BaseFile, TempPath))
                    flags |= 0x00000010; // VSDIFFOPT_LeftFileIsTemporary
                if (SvnItem.IsBelowRoot(args.MineFile, TempPath))
                    flags |= 0x00000020; // VSDIFFOPT_RightFileIsTemporary
            }

            IVsWindowFrame frame = _ocw2(args.BaseFile, args.MineFile, args.Caption ?? args.MineTitle, "", args.BaseTitle, args.MineTitle, args.Label ?? "", null, flags);

            if (frame != null)
            {
                GC.KeepAlive(new DiffMergeInstance(this, frame));
                return true;
            }

            return false;
        }
        #endregion

        #region Visual Studio 11 builtin merge
        bool _triedFindMerge;
        Type _type_IVsFileMergeService;
        object _vsFileMergeService;

        public bool HasMerge
        {
            get
            {
                if (!_triedFindMerge)
                {
                    _triedFindMerge = true;

                    _type_IVsFileMergeService = Type.GetType("Microsoft.VisualStudio.Shell.Interop.IVsFileMergeService, Microsoft.VisualStudio.Shell.Interop.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);
                    if (_type_IVsFileMergeService == null)
                        return false;

                    Type type_SVsFileMergeService = Type.GetType("Microsoft.VisualStudio.Shell.Interop.SVsFileMergeService, Microsoft.VisualStudio.Shell.Interop.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);

                    if (type_SVsFileMergeService == null)
                        return false;

                    object service = GetService(type_SVsFileMergeService);
                    if (service == null || (!_type_IVsFileMergeService.IsInstanceOfType(service) && !Marshal.IsComObject(service)))
                        return false;

                    _vsFileMergeService = service;
                }

                return (_vsFileMergeService != null);
            }
        }

        delegate IVsWindowFrame OpenAndRegisterMergeWindow([In] string leftFileMoniker, [In] string rightFileMoniker, [In] string baseFileMoniker, [In] string resultFileMoniker, [In] string leftFileTag, [In] string rightFileTag, [In] string baseFileTag, [In] string resultFileTag, [In] string leftFileLabel, [In] string rightFileLabel, [In] string baseFileLabel, [In] string resultFileLabel, [In] string serverGuid, [In] string leftFileSpec, [In] string rightFileSpec, out int cookie);
        internal delegate void UnregisterMergeWindow([In] int cookie);
        internal delegate void QueryMergeWindowState([In] int cookie, out int pfState, out string errorAndWarningMsg);
        OpenAndRegisterMergeWindow _ormw;
        UnregisterMergeWindow _umw;
        QueryMergeWindowState _qmws;
        public bool RunMerge(AnkhMergeArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            else if (!HasMerge)
                throw new InvalidOperationException();

            if (_ormw == null)
            {
                _ormw = GetInterfaceDelegate<OpenAndRegisterMergeWindow>(_type_IVsFileMergeService, _vsFileMergeService);
                _umw = GetInterfaceDelegate<UnregisterMergeWindow>(_type_IVsFileMergeService, _vsFileMergeService);
                _qmws = GetInterfaceDelegate<QueryMergeWindowState>(_type_IVsFileMergeService, _vsFileMergeService);
            }

            if (_ormw == null || _umw == null || _qmws == null)
                return false;

            int cookie;

            IVsWindowFrame frame = _ormw(args.TheirsFile, args.MineFile, args.BaseFile, args.MergedFile,
                                         Path.GetFileName(args.TheirsFile), Path.GetFileName(args.MineFile),
                                         Path.GetFileName(args.BaseFile), Path.GetFileName(args.MergedFile),
                                         args.TheirsTitle, args.MineTitle, args.BaseTitle, args.MergedTitle,
                                         Guid.Empty.ToString(), null, null, out cookie);

            if (frame != null)
            {
                GC.KeepAlive(new DiffMergeInstance(this, frame, _umw, _qmws, cookie));

                return true;
            }

            return false;
        }

        #endregion
    }
}