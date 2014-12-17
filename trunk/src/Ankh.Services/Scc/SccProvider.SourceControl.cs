using System;
using System.Collections.Generic;
using EnvDTEScc = EnvDTE.SourceControl;
using EnvDTEScc2 = EnvDTE80.SourceControl2;
using CheckoutOptions = EnvDTE80.vsSourceControlCheckOutOptions;
using SDTE = Microsoft.VisualStudio.Shell.Interop.SDTE;

namespace Ankh.Scc
{
    partial class SccProvider : EnvDTEScc2, EnvDTEScc
    {
        bool EnvDTEScc.CheckOutItem(string itemName)
        {
            return ((EnvDTEScc2)this).CheckOutItem2(itemName, CheckoutOptions.vsSourceControlCheckOutOptionLocalVersion);
        }

        bool EnvDTEScc.CheckOutItems(ref object[] itemNames)
        {
            return ((EnvDTEScc2)this).CheckOutItems2(ref itemNames, CheckoutOptions.vsSourceControlCheckOutOptionLocalVersion);
        }

        bool EnvDTEScc2.CheckOutItem(string itemName)
        {
            return ((EnvDTEScc2)this).CheckOutItem2(itemName, CheckoutOptions.vsSourceControlCheckOutOptionLocalVersion);
        }

        bool EnvDTEScc2.CheckOutItems(ref object[] itemNames)
        {
            return ((EnvDTEScc2)this).CheckOutItems2(ref itemNames, CheckoutOptions.vsSourceControlCheckOutOptionLocalVersion);
        }

        bool EnvDTEScc2.CheckOutItem2(string itemName, CheckoutOptions flags)
        {
            object[] items = new string[] { itemName };

            return ((EnvDTEScc2)this).CheckOutItems2(ref items, flags);
        }

        bool EnvDTEScc2.CheckOutItems2(ref object[] itemNames, CheckoutOptions flags)
        {
            try
            {
                return CheckOutItems((string[])itemNames, flags);
            }
            catch(Exception)
            {
                return false;
            }
        }

        [CLSCompliant(false)]
        protected virtual bool CheckOutItems(string[] itemNames, CheckoutOptions flags)
        {
            return true;
        }

        EnvDTE.DTE EnvDTEScc2.DTE
        {
            get { return GetService<EnvDTE.DTE>(typeof(SDTE)); }
        }

        EnvDTE.DTE EnvDTEScc.DTE
        {
            get { return GetService<EnvDTE.DTE>(typeof(SDTE)); }
        }

        void EnvDTEScc.ExcludeItem(string projectFile, string itemName)
        {
            ExcludeItems(projectFile, new string[] { itemName });
        }

        void EnvDTEScc.ExcludeItems(string projectFile, ref object[] itemNames)
        {
            ExcludeItems(projectFile, (string[])itemNames);
        }

        void EnvDTEScc2.ExcludeItem(string projectFile, string itemName)
        {
            ExcludeItems(projectFile, new string[] { itemName });
        }

        void EnvDTEScc2.ExcludeItems(string projectFile, ref object[] itemNames)
        {
            ExcludeItems(projectFile, (string[])itemNames);
        }

        protected virtual void ExcludeItems(string projectFile, string[] itemNames)
        {

        }

        void EnvDTEScc2.UndoExcludeItem(string projectFile, string itemName)
        {
            try
            {
                ExcludeItems(projectFile, new string[] { itemName });
            }
            catch { }
        }

        void EnvDTEScc2.UndoExcludeItems(string projectFile, ref object[] itemNames)
        {
            try
            {
                ExcludeItems(projectFile, (string[])itemNames);
            }
            catch { }
        }

        protected virtual void UndoExcludeItems(string projectFile, string[] itemNames)
        {

        }

        [CLSCompliant(false)]
        public virtual EnvDTE80.SourceControlBindings GetBindings(string ItemPath)
        {
            return null;
        }

        public virtual bool IsItemCheckedOut(string path)
        {
            return true;
        }

        public virtual bool IsItemUnderSCC(string path)
        {
            return true;
        }

        public virtual bool IsSccExcluded(string path)
        {
            return false;
        }

        [CLSCompliant(false)]
        public EnvDTE.DTE Parent
        {
            get { return GetService<EnvDTE.DTE>(typeof(SDTE)); }
        }
    }
}
