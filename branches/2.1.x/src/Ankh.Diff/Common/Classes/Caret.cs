// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Caret.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.27.2002	Created.

	BMenees	7.1.2003	Rewritten to more completely manage the caret.  Now
						it is much more like Petzold's example in "Programming
						Microsoft Windows with C#" (ch. 6, pg. 244).
						
	BMenees	6.4.2004	Fixed a bug where repeatedly setting Visible to the same
						value would keep calling ShowCaret or HideCaret, which 
						could cause it to stay visible or hidden too long.
-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Ankh.Diff
{
    /// <summary>
    /// Encapsulates the Win32 API for Carets.
    /// It inherits from MarshalByRefObject because it
    /// wraps unmanaged resources and can't be marshaled
    /// to remote contexts by value.
    /// </summary>
    public sealed class Caret : MarshalByRefObject, IDisposable
    {
        #region Public Members

        public Caret(Control Ctrl, int iHeight)
            : this(Ctrl, 2, iHeight)
        {
        }

        public Caret(Control Ctrl, int iWidth, int iHeight)
        {
            m_Ctrl = Ctrl;
            m_Size = new Size(iWidth, iHeight);
            m_Position = Point.Empty;

            Control.GotFocus += new EventHandler(ControlGotFocus);
            Control.LostFocus += new EventHandler(ControlLostFocus);

            //If the control already has focus, then create the caret.
            if (Ctrl.Focused)
            {
                ControlGotFocus(Ctrl, EventArgs.Empty);
            }
        }

        ~Caret()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public Control Control
        {
            get
            {
                return m_Ctrl;
            }
        }

        public bool Visible
        {
            get
            {
                return m_bVisible;
            }
            set
            {
                if (m_bVisible != value)
                {
                    m_bVisible = value;

                    if (m_bCreatedCaret)
                    {
                        if (value)
                        {
                            NativeMethods.ShowCaret(m_Ctrl);
                        }
                        else
                        {
                            NativeMethods.HideCaret(m_Ctrl);
                        }
                    }
                }
            }
        }

        public Point Position
        {
            get
            {
                if (m_bCreatedCaret)
                {
                    NativeMethods.GetCaretPos(ref m_Position);
                }
                return m_Position;
            }
            set
            {
                if (m_bCreatedCaret)
                {
                    NativeMethods.SetCaretPos(value.X, value.Y);
                }
                else
                {
                    m_Position = value;
                }
            }
        }

        public Size Size
        {
            get
            {
                return m_Size;
            }
        }

        #endregion

        #region Private Members

        private void Dispose(bool bDisposing)
        {
            if (bDisposing)
            {
                if (m_Ctrl.Focused)
                {
                    ControlLostFocus(m_Ctrl, EventArgs.Empty);
                }

                m_Ctrl.GotFocus -= new EventHandler(ControlGotFocus);
                m_Ctrl.LostFocus -= new EventHandler(ControlLostFocus);

                GC.SuppressFinalize(this);
            }
            else if (m_bCreatedCaret)
            {
                //The GC called our Finalize method, so we only 
                //need to release unmanaged resources.
                NativeMethods.DestroyCaret();
            }
        }

        private void ControlGotFocus(object sender, EventArgs e)
        {
            Debug.Assert(!m_bCreatedCaret, "When the control is getting focus we shouldn't already have a caret.");

            m_bCreatedCaret = NativeMethods.CreateCaret(m_Ctrl, m_Size.Width, m_Size.Height);
            if (m_bCreatedCaret)
            {
                NativeMethods.SetCaretPos(m_Position.X, m_Position.Y);
                Visible = true;
            }
        }

        private void ControlLostFocus(object sender, EventArgs e)
        {
            if (m_bCreatedCaret)
            {
                Visible = false;
                NativeMethods.DestroyCaret();
            }
        }

        #endregion

        #region Private Data Members

        private Control m_Ctrl;
        private Size m_Size;
        private Point m_Position;
        private bool m_bVisible;
        private bool m_bCreatedCaret;

        #endregion
    }
}
