#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Copyright © 2005 Bill Menees.  All rights reserved.
	Bill@Menees.com

	$History: RecentItemList.cs $
	
	*****************  Version 3  *****************
	User: Bill         Date: 3/15/06    Time: 9:49p
	Updated in $/CSharp/Menees/Components
	Fixed some places where StreamReader was used with the default UTF8
	encoding, which caused extended ASCII characters like © to get stripped
	out of the display text.
	
	*****************  Version 2  *****************
	User: Bill         Date: 11/06/05   Time: 12:57p
	Updated in $/CSharp/Menees/Components
	Renamed and refactored for .NET 2.0.

-----------------------------------------------------------------------------*/

#endregion

#region Using Directives

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

#endregion

namespace Ankh.Diff
{
    /// <summary>
    /// Used to manage a list of recent items (e.g., files, projects, directories).
    /// </summary>
    [ToolboxBitmap(typeof(RecentItemList), "Images.RecentItemList.bmp"), DefaultEvent("ItemClick")]
    public partial class RecentItemList : Component
    {
        #region Constructors

        public RecentItemList()
        {
            InitializeComponent();
        }

        public RecentItemList(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        #endregion

        #region Public Properties

        [Browsable(true), DefaultValue(10), Category("Behavior"),
        Description("The maximum number of recent items to manage.")]
        public int MaxItems
        {
            get { return m_iMaxItems; }
            set
            {
                m_iMaxItems = Math.Max(0, value);
                CropItemsAndUpdate(false);
            }
        }

        [Browsable(true), DefaultValue(c_strDefaultSection), Category("Behavior"),
        Description("The section under the FormSaver's base key where recent item settings should be saved.  This must be non-empty.")]
        public string SettingsSection
        {
            get { return m_strSection; }
            set
            {
                string strSection = value.Trim();

                //The section name must be non-empty because 
                //OnSaveSettings needs to delete the section 
                //subkey tree, and we don't want it to delete 
                //the base key instead.
                if (strSection.Length == 0)
                {
                    throw new ArgumentException("The section name must be non-empty.", "SettingsSection");
                }

                m_strSection = strSection;
            }
        }

        [Browsable(true), DefaultValue(null), Category("Helper Objects"),
        Description("The FormSaver object used to save and load settings.")]
        public FormSaver FormSaver
        {
            get { return m_FormSaver; }
            set
            {
                if (m_FormSaver != value)
                {
                    //Detach from old events
                    if (m_FormSaver != null)
                    {
                        m_FormSaver.InternalLoadSettings -= m_LoadHandler;
                        m_FormSaver.InternalSaveSettings -= m_SaveHandler;
                    }

                    m_FormSaver = value;

                    //Attach to new events
                    if (m_FormSaver != null)
                    {
                        //Create event handlers
                        if (m_LoadHandler == null) m_LoadHandler = new SettingsEventHandler(OnLoadSettings);
                        if (m_SaveHandler == null) m_SaveHandler = new SettingsEventHandler(OnSaveSettings);

                        //Attach to the internal events so we're assured of getting
                        //called before the public events.  This ensures that normal 
                        //MainForm.FormSave_LoadSettings event handlers fire after
                        //the recent items have been loaded, which forms sometimes
                        //need to check if they want to reload the last item used.
                        m_FormSaver.InternalLoadSettings += m_LoadHandler;
                        m_FormSaver.InternalSaveSettings += m_SaveHandler;
                    }
                }
            }
        }

        [Browsable(true), DefaultValue(null), Category("Helper Objects"),
        Description("The MenuItem that should contain the recent items.")]
        public ToolStripMenuItem MenuItem
        {
            get { return m_Menu; }
            set
            {
                if (m_Menu != value)
                {
                    //Delete any menu items on the old menu
                    DeleteMenuItems();

                    m_Menu = value;

                    //Build the new menu
                    UpdateMenu();
                }
            }
        }

        [Browsable(true), DefaultValue(null), Category("Helper Objects"),
        Description("The ContextMenu that should contain the recent items.")]
        public ContextMenuStrip ContextMenu
        {
            get { return m_ContextMenu; }
            set
            {
                if (m_ContextMenu != value)
                {
                    DeleteMenuItems();

                    m_ContextMenu = value;

                    UpdateMenu();
                }
            }
        }

        [Browsable(false), Description("The current number of items.")]
        public int Count
        {
            get
            {
                return m_Items.Count;
            }
        }

        [Browsable(false), Description("Gets the item for the specified index.")]
        public string this[int iIndex]
        {
            get
            {
                return m_Items[iIndex];
            }
        }

        [Browsable(false), Description("Gets or sets the list of items as a string array.")]
        public string[] Items
        {
            get
            {
                string[] arItems = new string[m_Items.Count];
                for (int i = 0; i < m_Items.Count; i++)
                    arItems[i] = m_Items[i];

                return arItems;
            }
            set
            {
                //Get rid of the old menu items and items
                DeleteMenuItems();
                m_Items.Clear();
                m_mapItemToStrings.Clear();

                //Add the items
                string[] arItems = value;
                for (int i = 0; i < arItems.Length; i++)
                {
                    if (arItems[i].Length > 0)
                    {
                        m_Items.Add(arItems[i]);
                    }
                }

                //Rebuild the menu
                CropItemsAndUpdate(true);
            }
        }

        #endregion

        #region Public Methods

        public void Add(string strItemName)
        {
            Add(strItemName, null);
        }

        public void Add(string strItemName, string[] arStrings)
        {
            if (strItemName == null) throw new ArgumentNullException("strItemName");

            strItemName = strItemName.Trim();
            if (strItemName.Length > 0)
            {
                Remove(strItemName);
                m_Items.Insert(0, strItemName);
                m_mapItemToStrings[strItemName] = arStrings;
                CropItemsAndUpdate(true);
            }
        }

        public bool Remove(string strItemName)
        {
            if (strItemName == null) throw new ArgumentNullException("strItemName");

            bool bResult = false;
            strItemName = strItemName.Trim();

            int iIndex = IndexOf(strItemName);
            if (iIndex >= 0)
            {
                m_Items.RemoveAt(iIndex);
                m_mapItemToStrings.Remove(strItemName);
                bResult = true;
                UpdateMenu();
            }

            return bResult;
        }

        public int IndexOf(string strItemName)
        {
            int iResult = -1;

            //Note: We can't use m_Items.IndexOf because it is case-sensitive.

            for (int i = 0; i < m_Items.Count; i++)
            {
                if (String.Compare(strItemName, m_Items[i], true) == 0)
                {
                    iResult = i;
                    break;
                }
            }

            return iResult;
        }

        public void Clear()
        {
            m_Items.Clear();
            m_mapItemToStrings.Clear();
            UpdateMenu();
        }

        public void Load(SettingsKey BaseKey)
        {
            using (SettingsKey Section = GetSectionKey(BaseKey, false))
            {
                if (Section != null)
                {
                    int iItem = 0;
                    while (true)
                    {
                        string strItem = Section.GetString(iItem.ToString(), "");
                        if (strItem.Length == 0)
                        {
                            break;
                        }

                        m_Items.Add(strItem);

                        //Load any custom strings if they exist.
                        using (SettingsKey StringsKey = Section.FindSubKey(iItem.ToString() + "_Strings"))
                        {
                            if (StringsKey != null)
                            {
                                int iCount = StringsKey.GetInt("Count", 0);
                                string[] arStrings = new string[iCount];
                                for (int iString = 0; iString < iCount; iString++)
                                {
                                    arStrings[iString] = StringsKey.GetString(iString.ToString(), "");
                                }
                                m_mapItemToStrings[strItem] = arStrings;
                            }
                        }

                        iItem++;
                    }
                }
            }

            UpdateMenu();
        }

        public void Save(SettingsKey BaseKey)
        {
            //Clear out any old settings.
            using (SettingsKey Section = GetSectionKey(BaseKey, false))
            {
                if (Section != null)
                {
                    Section.Dispose();
                    BaseKey.DeleteSubKey(m_strSection);
                }
            }

            //Recreate the section key.
            using (SettingsKey Section = GetSectionKey(BaseKey, true))
            {
                if (Section != null)
                {
                    int iNumItems = m_Items.Count;
                    for (int iItem = 0; iItem < iNumItems; iItem++)
                    {
                        string strItemName = m_Items[iItem];
                        Section.SetString(iItem.ToString(), strItemName);

                        //If they attached custom strings to the item, then save those too.
                        string[] arStrings;
                        m_mapItemToStrings.TryGetValue(strItemName, out arStrings);
                        if (arStrings != null)
                        {
                            using (SettingsKey StringsKey = Section.GetSubKey(iItem.ToString() + "_Strings"))
                            {
                                int iCount = arStrings.Length;
                                StringsKey.SetInt("Count", iCount);

                                for (int iString = 0; iString < iCount; iString++)
                                {
                                    StringsKey.SetString(iString.ToString(), arStrings[iString]);
                                }
                            }
                        }
                    }
                }
            }
        }

        public string[] GetStringsForItem(string strItemName)
        {
            string[] arStrings = m_mapItemToStrings[strItemName];
            return arStrings;
        }

        #endregion

        #region Public Events

        [Browsable(true), Category("Action"),
        Description("Called when a \"recent item\" menu item is clicked.")]
        public event ItemClickEventHandler ItemClick;

        #endregion

        #region Private Methods

        private void CropItemsAndUpdate(bool bUpdate)
        {
            while (m_Items.Count > m_iMaxItems && m_Items.Count > 0)
            {
                bUpdate = true;
                string strItemName = m_Items[m_Items.Count - 1];
                m_Items.RemoveAt(m_Items.Count - 1);
                m_mapItemToStrings.Remove(strItemName);
            }

            if (bUpdate)
            {
                UpdateMenu();
            }
        }

        private void AddMenuItems()
        {
            if (m_Menu != null)
            {
                AddMenuItems(m_Menu.DropDownItems);
            }
            if (m_ContextMenu != null)
            {
                AddMenuItems(m_ContextMenu.Items);
            }
        }

        private void AddMenuItems(ToolStripItemCollection clItems)
        {
            if (!DesignMode && clItems != null)
            {
                int iNumItems = m_Items.Count;
                if (iNumItems > 0)
                {
                    for (int i = 0; i < iNumItems; i++)
                    {
                        string strItemName = m_Items[i];
                        string strText = String.Format("{0}{1}:  {2}", (i < 9) ? "&" : "", i + 1, strItemName);
                        string[] arStrings;
                        m_mapItemToStrings.TryGetValue(strItemName, out arStrings);
                        ToolStripMenuItem Item = new RecentItemMenuItem(strText, new EventHandler(OnMenuItemClick), strItemName, arStrings);
                        clItems.Add(Item);
                    }
                }
                else
                {
                    //Add a disabled dummy item with no Click handler.
                    ToolStripMenuItem None = new ToolStripMenuItem("<None>");
                    None.Enabled = false;
                    clItems.Add(None);
                }
            }
        }

        private void DeleteMenuItems()
        {
            if (m_Menu != null)
            {
                DeleteMenuItems(m_Menu.DropDownItems);
            }
            if (m_ContextMenu != null)
            {
                DeleteMenuItems(m_ContextMenu.Items);
            }
        }

        private void DeleteMenuItems(ToolStripItemCollection clItems)
        {
            if (!DesignMode && clItems != null)
            {
                clItems.Clear();
            }
        }

        private void UpdateMenu()
        {
            DeleteMenuItems();
            AddMenuItems();
        }

        private void OnMenuItemClick(object sender, System.EventArgs e)
        {
            RecentItemMenuItem MI = sender as RecentItemMenuItem;
            if (ItemClick != null && MI != null)
            {
                ItemClick(this, new ItemClickEventArgs(MI.ItemName, MI.Strings));
            }
        }

        private SettingsKey GetSectionKey(SettingsKey Base, bool bForceCreation)
        {
            SettingsKey Section;
            if (bForceCreation)
            {
                Section = Base.GetSubKey(m_strSection);
            }
            else
            {
                Section = Base.FindSubKey(m_strSection);
            }

            return Section;
        }

        private void OnLoadSettings(object sender, SettingsEventArgs e)
        {
            if (!DesignMode)
            {
                Load(e.SettingsKey);
            }
        }

        private void OnSaveSettings(object sender, SettingsEventArgs e)
        {
            if (!DesignMode)
            {
                Save(e.SettingsKey);
            }
        }

        #endregion

        #region Private Types

        private class RecentItemMenuItem : ToolStripMenuItem
        {
            public RecentItemMenuItem(string strText, EventHandler eh, string strItemName, string[] arStrings)
                : base(strText, null, eh)
            {
                m_strItemName = strItemName;
                m_arStrings = arStrings;
            }

            public string ItemName { get { return m_strItemName; } }
            public string[] Strings { get { return m_arStrings; } }

            private string m_strItemName;
            private string[] m_arStrings;
        }

        #endregion

        #region Private Data Members

        private int m_iMaxItems = 10;
        private FormSaver m_FormSaver = null;
        private List<string> m_Items = new List<string>();
        private Dictionary<string, string[]> m_mapItemToStrings = new Dictionary<string, string[]>();
        private ToolStripMenuItem m_Menu = null;
        private SettingsEventHandler m_LoadHandler = null;
        private SettingsEventHandler m_SaveHandler = null;
        private string m_strSection = c_strDefaultSection;
        private ContextMenuStrip m_ContextMenu = null;

        private const string c_strDefaultSection = "Recent Items";

        #endregion
    }
}
