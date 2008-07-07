#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Copyright © 2005 Bill Menees.  All rights reserved.
	Bill@Menees.com

	$History: FormSaver.cs $
	
	*****************  Version 3  *****************
	User: Bill         Date: 3/05/08    Time: 8:32p
	Updated in $/CSharp/Menees/Components
	Made it not resize fixed sized dialogs.
	
	*****************  Version 2  *****************
	User: Bill         Date: 11/06/05   Time: 12:57p
	Updated in $/CSharp/Menees/Components
	Renamed and refactored for .NET 2.0.

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	5.28.2002	Created.

	BMenees	12.4.2002	Changed so the LoadSettings event is always called
						by Load even if the registry keys didn't already exist.
						Now the base and section keys are forcibly created.
						This allows programs to consolidate loading and
						initialization logic easier.

	BMenees	3.13.2003	Changed Load to call ResumeLayout without passing in
						false.  Passing false caused child dialogs using FormSave
						components to not get laid out correctly.

	BMenees	7.1.2003	Added internal events that fire before/after the public
						load/save events.  This allows RecentFiles to ensure it
						gets loaded before any forms get their LoadSettings
						callback.  That way forms can safely get the last file
						that was loaded.
						
	BMenees	7.1.2003	Added logic to make sure the form is somewhere on the
						virtual screen.

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
using System.ComponentModel.Design;

#endregion

namespace Ankh.Diff
{
    [ToolboxBitmap(typeof(FormSaver), "Images.FormSaver.bmp")]
    public partial class FormSaver : Component
    {
        #region Constructors

        public FormSaver()
        {
            InitializeComponent();
        }

        public FormSaver(IContainer Container)
        {
            Container.Add(this);
            InitializeComponent();
        }

        public FormSaver(ContainerControl Container)
        {
            InitializeComponent();
            ContainerControl = Container;
        }

        #endregion

        #region Public Properties

        [Browsable(true), DefaultValue(true), Category("Behavior"),
        Description("Whether the settings should automatically load when the form loads.")]
        public bool AutoLoad
        {
            get { return m_bAutoLoad; }
            set { m_bAutoLoad = value; }
        }

        [Browsable(true), DefaultValue(true), Category("Behavior"),
        Description("Whether the settings should automatically save when the form closes.")]
        public bool AutoSave
        {
            get { return m_bAutoSave; }
            set { m_bAutoSave = value; }
        }

        [Browsable(true), DefaultValue(false), Category("Behavior"),
        Description("Whether the form should be allowed to load minimized.")]
        public bool AllowLoadMinimized
        {
            get { return m_bAllowLoadMinimized; }
            set { m_bAllowLoadMinimized = value; }
        }

        [Browsable(true), DefaultValue(""), Category("Behavior"),
        Description("The base path where this application's settings should be saved.")]
        public string BaseSettingsPath
        {
            get { return m_strBaseKey; }
            set { m_strBaseKey = value.Trim(); }
        }

        [Browsable(true), DefaultValue(c_strFormLayout), Category("Behavior"),
        Description("The section under the base setting's path where form layout settings should be saved.  This can be empty.")]
        public string FormLayoutSettingsSection
        {
            get { return m_strSection; }
            set { m_strSection = value.Trim(); }
        }

        [Browsable(false), DefaultValue(null), Category("Helper Objects"),
        Description("The form to save the settings for.  This must be set.")]
        public ContainerControl ContainerControl
        {
            get
            {
                return m_ContainerControl;
            }
            set
            {
                m_ContainerControl = value;
                Form = m_ContainerControl as Form;
            }
        }

        public override ISite Site
        {
            set
            {
                //This is used to automatically set the ContainerControl and Form
                //properties.  I got this code from the ErrorProvider component and
                //from http://www.wiredprairie.us/hardwired,ComponentContainer.aspx.

                base.Site = value;
                if (value != null)
                {
                    IDesignerHost Host = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (Host != null)
                    {
                        ContainerControl RootContainer = Host.RootComponent as ContainerControl;
                        if (RootContainer != null)
                        {
                            ContainerControl = RootContainer;
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void Load()
        {
            if (m_Form == null)
            {
                throw new ArgumentNullException("Form", "Load requires a non-null Form.");
            }
            if (Utilities.IsEmpty(m_strBaseKey))
            {
                throw new ArgumentException("Save requires BaseSettingsPath to be a non-empty string.");
            }

            if (!DesignMode)
            {
                using (SettingsKey Base = GetBaseKey())
                {
                    if (Base != null)
                    {
                        SettingsKey Section = GetSectionKey(Base);
                        {
                            if (Section != null)
                            {
                                int iLeft = Section.GetInt("Left", m_Form.Left);
                                int iTop = Section.GetInt("Top", m_Form.Top);
                                int iWidth = Section.GetInt("Width", m_Form.Width);
                                int iHeight = Section.GetInt("Height", m_Form.Height);
                                FormWindowState State = (FormWindowState)Section.GetInt("WindowState", (int)m_Form.WindowState);

                                m_Form.SuspendLayout();
                                try
                                {
                                    m_Form.Left = iLeft;
                                    m_Form.Top = iTop;
                                    if (m_Form.FormBorderStyle == FormBorderStyle.Sizable || m_Form.FormBorderStyle == FormBorderStyle.SizableToolWindow)
                                    {
                                        m_Form.Width = iWidth;
                                        m_Form.Height = iHeight;
                                    }

                                    //If the form's state isn't Normal, then it was launched from a
                                    //shortcut or command line START command to be Minimized or Maximized.
                                    //In those cases, we don't want to override the state.
                                    if (m_Form.WindowState == FormWindowState.Normal &&
                                        (m_bAllowLoadMinimized || State != FormWindowState.Minimized))
                                    {
                                        m_Form.WindowState = State;
                                    }
                                }
                                finally
                                {
                                    m_Form.ResumeLayout();
                                }

                                //Make sure the window is somewhere on one of the screens.
                                if (!SystemInformation.VirtualScreen.Contains(iLeft, iTop))
                                {
                                    Point Pt = SystemInformation.VirtualScreen.Location;
                                    Pt.Offset(20, 20);
                                    m_Form.DesktopLocation = Pt;
                                }
                            }
                        }

                        //Fire the Internal events first
                        if (InternalLoadSettings != null)
                        {
                            InternalLoadSettings(this, new SettingsEventArgs(Base));
                        }

                        if (LoadSettings != null)
                        {
                            LoadSettings(this, new SettingsEventArgs(Base));
                        }
                    }
                }
            }
        }

        public void Save()
        {
            if (m_Form == null)
            {
                throw new ArgumentNullException("Form", "Save requires a non-null Form.");
            }
            if (Utilities.IsEmpty(m_strBaseKey))
            {
                throw new ArgumentException("Save requires BaseSettingsPath to be a non-empty string.");
            }

            if (!DesignMode)
            {
                using (SettingsKey Base = GetBaseKey())
                {
                    if (Base != null)
                    {
                        using (SettingsKey Section = GetSectionKey(Base))
                        {
                            if (Section != null)
                            {
                                Section.SetInt("Left", m_iLeft);
                                Section.SetInt("Top", m_iTop);
                                Section.SetInt("Width", m_iWidth);
                                Section.SetInt("Height", m_iHeight);
                                Section.SetInt("WindowState", (int)m_eWindowState);
                            }

                            //Fire the public event first.
                            if (SaveSettings != null)
                            {
                                SaveSettings(this, new SettingsEventArgs(Base));
                            }

                            if (InternalSaveSettings != null)
                            {
                                InternalSaveSettings(this, new SettingsEventArgs(Base));
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Events

        [Browsable(true), Category("Serialization"),
        Description("Called when settings are being loaded.")]
        public event SettingsEventHandler LoadSettings;

        [Browsable(true), Category("Serialization"),
        Description("Called when settings are being saved.")]
        public event SettingsEventHandler SaveSettings;

        #endregion

        #region Internal Events

        //Called before any LoadSettings events.
        [Browsable(false)]
        internal event SettingsEventHandler InternalLoadSettings;

        //Called after any SaveSettings events.
        [Browsable(true)]
        internal event SettingsEventHandler InternalSaveSettings;

        #endregion

        #region Private Properties

        private Form Form
        {
            get { return m_Form; }
            set
            {
                if (m_Form != value)
                {
                    if (m_Form != null)
                    {
                        //Detach from old form events
                        m_Form.Load -= m_LoadHandler;
                        m_Form.Closed -= m_ClosedHandler;
                        m_Form.Resize -= m_ResizeHandler;
                        m_Form.Move -= m_MoveHandler;
                    }

                    m_Form = value;

                    if (m_Form != null)
                    {
                        //Create event handlers
                        if (m_LoadHandler == null) m_LoadHandler = new EventHandler(OnFormLoad);
                        if (m_ClosedHandler == null) m_ClosedHandler = new EventHandler(OnFormClosed);
                        if (m_ResizeHandler == null) m_ResizeHandler = new EventHandler(OnFormResize);
                        if (m_MoveHandler == null) m_MoveHandler = new EventHandler(OnFormMove);

                        //Attach to new form events
                        m_Form.Load += m_LoadHandler;
                        m_Form.Closed += m_ClosedHandler;
                        m_Form.Resize += m_ResizeHandler;
                        m_Form.Move += m_MoveHandler;

                        //Cache initial values in case the form is never moved or resized.
                        m_iLeft = m_Form.Left;
                        m_iTop = m_Form.Top;
                        m_iWidth = m_Form.Width;
                        m_iHeight = m_Form.Height;
                        m_eWindowState = m_Form.WindowState;
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private SettingsKey GetBaseKey()
        {
            if (!Utilities.IsEmpty(m_strBaseKey))
            {
                return new SettingsKey(m_strBaseKey);
            }
            else
            {
                return null;
            }
        }

        private SettingsKey GetSectionKey(SettingsKey Base)
        {
            SettingsKey Section = Base;

            if (Section != null && !Utilities.IsEmpty(m_strSection))
            {
                Section = Base.GetSubKey(m_strSection);
            }

            return Section;
        }

        private void OnFormLoad(object sender, System.EventArgs e)
        {
            if (m_bAutoLoad)
            {
                Load();
            }
        }

        private void OnFormClosed(object sender, System.EventArgs e)
        {
            if (m_bAutoSave)
            {
                Save();
            }
        }

        private void OnFormResize(object sender, System.EventArgs e)
        {
            if (m_Form.WindowState == FormWindowState.Normal)
            {
                m_iWidth = m_Form.Width;
                m_iHeight = m_Form.Height;
            }
        }

        private void OnFormMove(object sender, System.EventArgs e)
        {
            if (m_Form.WindowState == FormWindowState.Normal)
            {
                m_iLeft = m_Form.Left;
                m_iTop = m_Form.Top;
            }

            m_eWindowState = m_Form.WindowState;
        }

        #endregion

        #region Private Data Members

        private bool m_bAutoLoad = true;
        private bool m_bAutoSave = true;
        private string m_strBaseKey = c_strDefaultBaseKey;
        private string m_strSection = c_strFormLayout;
        private ContainerControl m_ContainerControl;
        private Form m_Form = null;
        private bool m_bAllowLoadMinimized = false;

        private int m_iLeft = 0;
        private int m_iTop = 0;
        private int m_iWidth = 0;
        private int m_iHeight = 0;
        private FormWindowState m_eWindowState = FormWindowState.Normal;

        private EventHandler m_LoadHandler = null;
        private EventHandler m_ClosedHandler = null;
        private EventHandler m_ResizeHandler = null;
        private EventHandler m_MoveHandler = null;

        private const string c_strDefaultBaseKey = "";
        private const string c_strFormLayout = "Form Layout";

        #endregion
    }
}
