using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.Scc.UI;

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

        delegate IVsWindowFrame Diff_OpenComparisonWindow2([In] string leftFileMoniker, [In] string rightFileMoniker, [In] string caption, [In] string Tooltip, [In] string leftLabel, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string rightLabel, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string inlineLabel, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string roles, [In] uint grfDiffOptions);
        public bool RunDiff(AnkhDiffArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            else if (!HasDiff)
                throw new InvalidOperationException();

            Diff_OpenComparisonWindow2 OpenComparisonWindow2 = GetInterfaceMethod<Diff_OpenComparisonWindow2>(_type_IVsDifferenceService, _vsDifferenceService);

            if (OpenComparisonWindow2 == null)
                return false;

            IVsWindowFrame frame = OpenComparisonWindow2(args.BaseFile, args.MineFile, args.Caption ?? args.MineTitle, "", args.BaseTitle, args.MineTitle, args.Label ?? "", null, 0);

            return (frame != null);
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

                    // This interface is implemented as .Net object, instead of COM so we have to reference the interop dll
                    // or do this the hard way with reflection...
                    // Let's do it the hard way in order not to break VS 2005-2010.

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

        delegate IVsWindowFrame Merge_OpenAndRegisterMergeWindow([In] string leftFileMoniker, [In] string rightFileMoniker, [In] string baseFileMoniker, [In] string resultFileMoniker, [In] string leftFileTag, [In] string rightFileTag, [In] string baseFileTag, [In] string resultFileTag, [In] string leftFileLabel, [In] string rightFileLabel, [In] string baseFileLabel, [In] string resultFileLabel, [In] string serverGuid, [In] string leftFileSpec, [In] string rightFileSpec, out int cookie);
        delegate void Merge_UnregisterMergeWindow([In] int cookie);
        delegate void Merge_QueryMergeWindowState([In] int cookie, out int pfState, out string errorAndWarningMsg);
        public bool RunMerge(AnkhMergeArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            else if (!HasMerge)
                throw new InvalidOperationException();

            Merge_OpenAndRegisterMergeWindow OpenAndRegisterMergeWindow = GetInterfaceMethod<Merge_OpenAndRegisterMergeWindow>(_type_IVsFileMergeService, _vsFileMergeService);
            Merge_UnregisterMergeWindow UnregisterMergeWindow = GetInterfaceMethod<Merge_UnregisterMergeWindow>(_type_IVsFileMergeService, _vsFileMergeService);

            if (OpenAndRegisterMergeWindow == null || UnregisterMergeWindow == null)
                return false;


            int cookie;

            IVsWindowFrame frame = OpenAndRegisterMergeWindow(args.TheirsFile, args.MineFile, args.BaseFile, args.MergedFile,
                                                              args.TheirsFile, args.MineFile, args.BaseFile, args.MergedFile,
                                                              args.TheirsTitle, args.MineTitle, args.BaseTitle, args.MergedTitle,
                                                              Guid.Empty.ToString(), null, null, out cookie);

            if (frame != null)
                UnregisterMergeWindow(cookie);

            return (frame != null);
        }

        #endregion

        #region Delegate helpers
        Dictionary<Type, Delegate> _interfaceMethods;
        T GetInterfaceMethod<T>(Type fromInterface, object onService) where T : class
        {
            if (fromInterface == null)
                throw new ArgumentNullException("fromInterface");
            else if (onService == null)
                throw new ArgumentNullException("onService");

            if (_interfaceMethods == null)
                _interfaceMethods = new Dictionary<Type, Delegate>();

            Type type = typeof(T);
            Delegate dlg;

            if (!_interfaceMethods.TryGetValue(type, out dlg))
            {
                string name = type.Name;
                name = name.Substring(name.IndexOf('_') + 1);

                MethodInfo method = fromInterface.GetMethod(name);

                if (method == null)
                    return default(T);

                dlg = Delegate.CreateDelegate(type, onService, method, false);
                _interfaceMethods.Add(type, dlg);
            }

            return dlg as T;
        }
        #endregion
    }
}
