using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Collections;
using Microsoft.VisualStudio.OLE.Interop;
using IServiceProvider = System.IServiceProvider;

namespace Ankh.Selection
{
	public interface ISelectionContext
	{
		ICollection<string> GetSelectedFiles();
        ICollection<string> GetSelectedFiles(bool recursive);
        ICollection<SvnItem> GetSelectedSvnItems();
		ICollection<SvnItem> GetSelectedSvnItems(bool recursive);
	}

	/// <summary>
	/// 
	/// </summary>
	class SelectionContext : IVsSelectionEvents, IDisposable, ISelectionContext
	{
		IServiceProvider _environment;
		StatusCache _cache;
		uint _cookie;

		uint _currentItem;
		IVsHierarchy _currentHierarchy;
		IVsMultiItemSelect _currentSelection;
		ISelectionContainer _currentContainer;
		string[] _filenames;
        string[] _filenamesRecursive;
		SvnItem[] _svnItems;
        SvnItem[] _svnItemsRecursive;

		public SelectionContext(IServiceProvider environment, StatusCache cache)
		{
			if (environment == null)
				throw new ArgumentNullException("environment");
			else if(cache == null)
				throw new ArgumentNullException("cache");

			_environment = environment;
			_cache = cache;

			IVsMonitorSelection monitor = (IVsMonitorSelection)environment.GetService(typeof(IVsMonitorSelection));

			if (monitor != null)
				Marshal.ThrowExceptionForHR(monitor.AdviseSelectionEvents(this, out _cookie));
			else
				_environment = null;
		}

		public void Dispose()
		{
			if (_environment != null)
			{
				IVsMonitorSelection monitor = (IVsMonitorSelection)_environment.GetService(typeof(IVsMonitorSelection));
				_environment = null;

				Marshal.ThrowExceptionForHR(monitor.UnadviseSelectionEvents(_cookie));
			}
		}

		#region IVsSelectionEvents Members

		public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
		{
			/// Some global state change which might change UI cueues
			return VSConstants.S_OK;
		}

		public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
		{
			// Some property changed
			return VSConstants.S_OK;
		}

		public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld,
				IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
		{
			// The current selection changed; store for future reference
			_currentHierarchy = pHierNew;
			_currentItem = itemidNew;
			_currentSelection = pMISNew;
			_currentContainer = pSCNew;

			ClearCache();

			return VSConstants.S_OK;
		}

		private void ClearCache()
		{
			_filenames = null;
			_svnItems = null;
            _filenamesRecursive = null;
            _svnItemsRecursive = null;
		}

		#endregion

		protected class SelectionItem : IEquatable<SelectionItem>
		{
			readonly IVsHierarchy _hierarchy;
			readonly uint _id;

			public SelectionItem(IVsHierarchy hierarchy, uint id)
			{
				_hierarchy = hierarchy;
				_id = id;
			}

			public IVsHierarchy Hierarchy
			{
				get { return _hierarchy; }
			}

			public uint Id
			{
				get { return _id; }
			}

            #region IEquatable<SelectionItem> Members

            public bool Equals(SelectionItem other)
            {
                if (other == null)
                    return false;

                return (other.Id == Id) && (other.Hierarchy == Hierarchy);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SelectionItem);
            }

            public override int GetHashCode()
            {
                return _hierarchy.GetHashCode() ^ _id.GetHashCode();
            }

            #endregion
        }

        protected IEnumerable<SelectionItem> GetSelectedItems(bool recursive)
        {
            return recursive ? GetSelectedItemsRecursive() : GetSelectedItems();
        }
        
		protected IEnumerable<SelectionItem> GetSelectedItems()
		{
			if (_currentSelection != null)
			{
				uint nItems;
				int withinSingleHierarchy;
				Marshal.ThrowExceptionForHR(_currentSelection.GetSelectionInfo(out nItems, out withinSingleHierarchy));

				uint flags = 0;

				bool singleHierarchy = (withinSingleHierarchy != 0);

				if (singleHierarchy && _currentHierarchy != null)
					flags = (uint)__VSGSIFLAGS.GSI_fOmitHierPtrs; // Don't marshal the hierarchy for every item

				VSITEMSELECTION[] items = new VSITEMSELECTION[nItems];

				Marshal.ThrowExceptionForHR(_currentSelection.GetSelectedItems(flags, nItems, items));

				for (int i = 0; i < nItems; i++)
				{
					yield return new SelectionItem(singleHierarchy ? _currentHierarchy : items[i].pHier, items[i].itemid);
				}
			}
			else if (_currentHierarchy != null)
			{
				yield return new SelectionItem(_currentHierarchy, _currentItem);
			}
            else if (_currentContainer == null)
            { 
                // No selection, no hierarchy.... -> no selection!
            }
            else if (_currentItem == VSConstants.VSITEMID_ROOT)
            {
                // This is the case in the solution explorer when only the solution is selected

                // TODO: Check if the container is the Solution Explorer

                IVsSolution2 s2 = null;

                if (_environment != null)
                    s2 = (IVsSolution2)_environment.GetService(typeof(SVsSolution));

                if (s2 != null)
                {
                    IVsHierarchy h = s2 as IVsHierarchy;

                    yield return new SelectionItem(h, _currentItem);
                }
            }
		}

        protected IEnumerable<SelectionItem> GetSelectedItemsRecursive()
        {
            Dictionary<SelectionItem, SelectionItem> ticked = new Dictionary<SelectionItem, SelectionItem>();
            foreach (SelectionItem si in GetSelectedItems())
            {
                if (ticked.ContainsKey(si))
                    continue;

                ticked.Add(si, si);
                yield return si;

                foreach (SelectionItem i in GetDescendants(si, ticked))
                {
                    yield return i;
                }
            }
        }

        private static uint GetItemIdFromObject(object pvar)
        {
            if (pvar == null) return VSConstants.VSITEMID_NIL;
            if (pvar is int) return (uint)(int)pvar;
            if (pvar is uint) return (uint)pvar;
            if (pvar is short) return (uint)(short)pvar;
            if (pvar is ushort) return (uint)(ushort)pvar;
            if (pvar is long) return (uint)(long)pvar;
            return VSConstants.VSITEMID_NIL;
        }

        private IEnumerable<SelectionItem> GetDescendants(SelectionItem si, Dictionary<SelectionItem, SelectionItem> previous)
        {
            if (si.Hierarchy == null)
                yield break;

            Guid hierarchyId = typeof(IVsHierarchy).GUID;
            IntPtr hierPtr;
            uint id;

            int hr = si.Hierarchy.GetNestedHierarchy(si.Id, ref hierarchyId, out hierPtr, out id);

            if (hr == VSConstants.S_OK && hierPtr != IntPtr.Zero)
            {
                IVsHierarchy nestedHierarchy = Marshal.GetObjectForIUnknown(hierPtr) as IVsHierarchy;
                Marshal.Release(hierPtr); // we are responsible to release the refcount on the out IntPtr parameter
                if (nestedHierarchy != null)
                {
                    // Display name and type of the node in the Output Window
                    SelectionItem i = new SelectionItem(nestedHierarchy, id);

                    if (previous.ContainsKey(i))
                        yield break;

                    previous.Add(i, i);

                    yield return i;

                    foreach (SelectionItem ii in GetDescendants(i, previous))
                    {
                        yield return ii;
                    }
                }
            }
            
            // Note: There is a bug with firstchild on solutions pre vs2008, in that it contains
            // all projects on the top level instead of below solution folders. But we can ignore 
            // that as we would include the projects anyway
            object child;
            Marshal.ThrowExceptionForHR(si.Hierarchy.GetProperty(si.Id, 
                (int)__VSHPROPID.VSHPROPID_FirstChild, out child));

            uint childId = GetItemIdFromObject(child);
            while (childId != VSConstants.VSITEMID_NIL)
            {
                SelectionItem i = new SelectionItem(si.Hierarchy, childId);
                if (!previous.ContainsKey(i))
                {
                    previous.Add(i, i);
                    yield return i;
                }

                foreach (SelectionItem ii in GetDescendants(i, previous))
                {
                    yield return ii;
                }

                Marshal.ThrowExceptionForHR(si.Hierarchy.GetProperty(i.Id, 
                    (int)__VSHPROPID.VSHPROPID_NextSibling, out child));

                childId = GetItemIdFromObject(child);
            }
        }
                

		#region ISelectionContext Members

        public ICollection<string> GetSelectedFiles()
        {
            return GetSelectedFiles(false);
        }

        public ICollection<string> GetSelectedFiles(bool recursive)
		{
            if (recursive && (_filenamesRecursive != null))
                return _filenamesRecursive;
            else if(!recursive && (_filenames != null))
				return _filenames;

			List<string> filenames = new List<string>();
			int hr;

			// Selection can be generated by several objects. 
			// E.g. the solution provider, a document, our own toolwindows..
			foreach (SelectionItem i in GetSelectedItems(recursive))
			{
				IVsSccProject2 p2 = i.Hierarchy as IVsSccProject2;

				if (p2 != null)
				{
					CALPOLESTR[] pathStr = new CALPOLESTR[1];
					CADWORD[] flags = new CADWORD[1];

					hr = p2.GetSccFiles(i.Id, pathStr, flags);

					if (hr == VSConstants.S_OK)
					{
						foreach (string file in GetFilesFor(p2, i.Id, pathStr, flags))
						{
							filenames.Add(file);
						}
						
					}
					else if (hr == VSConstants.E_NOTIMPL || hr == VSConstants.E_FAIL)
					{
						IVsProject3 p3 = p2 as IVsProject3;

						if (p3 != null)
						{
							string file;
							hr = p3.GetMkDocument(i.Id, out file);

							if (hr == VSConstants.S_OK)
								filenames.Add(file);
						}
					}
					
					Marshal.ThrowExceptionForHR(hr);
					continue;
				}

				IVsProject3 pp3 = p2 as IVsProject3;

				if (pp3 != null)
				{
					string file;
					hr = pp3.GetMkDocument(i.Id, out file);

					if (hr == VSConstants.S_OK)
						filenames.Add(file);
					continue;
				}

				IVsSolution2 solution = i.Hierarchy as IVsSolution2;

				if (solution != null)
				{
					string solutionDir, solutionFile, userFile;
					hr = solution.GetSolutionInfo(out solutionDir, out solutionFile, out userFile);
					filenames.Add(solutionFile);
					Marshal.ThrowExceptionForHR(hr);

					continue;
				}

				GC.KeepAlive(i.Hierarchy);
			}

            if(recursive)
			    return _filenamesRecursive = filenames.ToArray();
            else
                return _filenames = filenames.ToArray();
		}

        public ICollection<SvnItem> GetSelectedSvnItems()
        {
            return GetSelectedSvnItems(false);
        }

		public ICollection<SvnItem> GetSelectedSvnItems(bool recursive)
		{
            if (recursive && (_svnItemsRecursive != null))
                return _svnItemsRecursive;
            else if (!recursive && (_svnItems != null))
                return _svnItems;

			List<SvnItem> items = new List<SvnItem>();
			foreach (string file in GetSelectedFiles(recursive))
			{
				SvnItem i = _cache[file];

				if (i != null)
					items.Add(i);
			}

            if(recursive)
                return _svnItemsRecursive = items.ToArray();
            else
			    return _svnItems = items.ToArray();
		}

		#endregion

		/// <summary>
		/// Gets the files from an OLE String buffer and clears the buffer
		/// </summary>
		/// <param name="pathStr">The path STR.</param>
		/// <returns></returns>
		string[] GetFilesFromOleBuffer(CALPOLESTR[] pathStr)
		{
			int nEls = (int)pathStr[0].cElems;
			string[] files = new string[nEls];
		
			for (int i = 0; i < nEls; i++)
			{
				IntPtr pathIntPtr = Marshal.ReadIntPtr(pathStr[0].pElems, i);
				files[i] = Marshal.PtrToStringUni(pathIntPtr);

				Marshal.FreeCoTaskMem(pathIntPtr);				
			}
			if (pathStr[0].pElems != IntPtr.Zero)
				Marshal.FreeCoTaskMem(pathStr[0].pElems);

			return files;
		}

		private IEnumerable<string> GetFilesFor(IVsSccProject2 p, uint itemId, CALPOLESTR[] pathStr, CADWORD[] flags)
		{
			int n = 0;
			foreach (string file in GetFilesFromOleBuffer(pathStr))
			{
				yield return file;

				if (flags[0].cElems > n)
				{
					int flag = Marshal.ReadInt32(flags[0].pElems, n);

					if (flag != 0)
					{
						CALPOLESTR[] subPathStr = new CALPOLESTR[1];
						CADWORD[] subFlags = new CADWORD[1];

						Marshal.ThrowExceptionForHR(p.GetSccSpecialFiles(itemId, file, subPathStr, subFlags));

						foreach (string fi in GetFilesFor(p, itemId, subPathStr, subFlags))
						{
							yield return fi;
						}
					}
				}
				n++;
			}

			if(flags[0].pElems != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(flags[0].pElems);
			}
		}
	}
}