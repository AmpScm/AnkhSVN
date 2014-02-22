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

	NumericTextBox.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	11.1.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

#region Using Statements

using System;
using System.Windows.Forms;
using System.ComponentModel; //For Browsable
using System.Globalization;
using System.Drawing;
using System.Security.Permissions;
using System.Runtime.InteropServices;

#endregion

namespace Ankh.Diff
{
    /// <summary>
    /// A textbox for editing numeric values.
    /// </summary>
    public sealed class NumericTextBox : TextBox
    {
        #region Constructors

        public NumericTextBox()
        {
            //Change the default text
            base.Text = "0";
        }

        #endregion

        #region Changed Inherited Properties

        //Hide the inherited properties we don't want users to access at design time.

        [Browsable(false), ReadOnly(true)]
        public override string Text { get { return base.Text; } set { base.Text = value; } }
        [Browsable(false), ReadOnly(true)]
        public override bool Multiline { get { return false; } set { throw new NotSupportedException(); } }
        [Browsable(false), ReadOnly(true)]
        public new ScrollBars ScrollBars { get { return ScrollBars.None; } set { throw new NotSupportedException(); } }
        [Browsable(false), ReadOnly(true)]
        public new bool AcceptsReturn { get { return false; } set { throw new NotSupportedException(); } }
        [Browsable(false), ReadOnly(true)]
        public new bool AcceptsTab { get { return false; } set { throw new NotSupportedException(); } }
        [Browsable(false), ReadOnly(true)]
        public new string[] Lines { get { return new string[0]; } set { throw new NotSupportedException(); } }
        [Browsable(false), ReadOnly(true)]
        public new bool WordWrap { get { return false; } set { throw new NotSupportedException(); } }
        [Browsable(false), ReadOnly(true)]
        public new char PasswordChar { get { return '\0'; } set { throw new NotSupportedException(); } }

        #endregion

        #region Custom Properties

        //Expose our custom properties
        [Browsable(true), DefaultValue(0.0), Category("Behavior"), RefreshProperties(RefreshProperties.All),
        Description("The lower bound for Value (if MinValue or MaxValue is non-zero).")]
        public double MinValue
        {
            get
            {
                return m_dMinValue;
            }
            set
            {
                if (value != m_dMinValue)
                {
                    m_dMinValue = value;
                    ValidateValue();
                }
            }
        }

        [Browsable(true), DefaultValue(0.0), Category("Behavior"), RefreshProperties(RefreshProperties.All),
        Description("The upper bound for Value (if MinValue or MaxValue is non-zero).")]
        public double MaxValue
        {
            get
            {
                return m_dMaxValue;
            }
            set
            {
                if (value != m_dMaxValue)
                {
                    m_dMaxValue = value;
                    ValidateValue();
                }
            }
        }

        [Browsable(true), DefaultValue(0.0), Category("Appearance"),
        RefreshProperties(RefreshProperties.All),
        Description("The current numeric value.")]
        public double Value
        {
            get
            {
                return m_dValue;
            }
            set
            {
                if (value != m_dValue)
                {
                    m_dValue = value;
                    ValidateValue();
                }
            }
        }

        [Browsable(true), DefaultValue(false), Category("Behavior"),
        RefreshProperties(RefreshProperties.All),
        Description("Whether floating point (i.e. non-integer) values should be allowed.")]
        public bool AllowFloat
        {
            get
            {
                return m_bAllowFloat;
            }
            set
            {
                if (value != m_bAllowFloat)
                {
                    m_bAllowFloat = value;
                    ValidateValue();
                }
            }
        }

        [Browsable(true), DefaultValue(false), Category("Behavior"),
        RefreshProperties(RefreshProperties.All),
        Description("Whether negative numbers should be allowed.")]
        public bool AllowNegative
        {
            get
            {
                return m_bAllowNegative;
            }
            set
            {
                if (value != m_bAllowNegative)
                {
                    m_bAllowNegative = value;
                    ValidateValue();
                }
            }
        }

        [Browsable(true), DefaultValue(false), Category("Behavior"),
        Description("Whether arbitrary input should be allowed (e.g. alpha characters, pasted text).  If false only digits, the negative sign, and the decimal separator are allowed.")]
        public bool AllowAllInput
        {
            get
            {
                return m_bAllowAllInput;
            }
            set
            {
                m_bAllowAllInput = value;
            }
        }

        [Browsable(true), DefaultValue(true), Category("Behavior"),
        Description("Whether IntValue should be rounded or truncated from Value.")]
        public bool RoundIntValue
        {
            get
            {
                return m_bRoundInt;
            }
            set
            {
                m_bRoundInt = value;
            }
        }

        [Browsable(true), DefaultValue(false), Category("Behavior"),
        Description("Whether a beep should occur on invalid input.")]
        public bool BeepOnError
        {
            get
            {
                return m_bBeepOnError;
            }
            set
            {
                m_bBeepOnError = value;
            }
        }

        [Browsable(false), Description("Whether the clipboard contains numeric text that can be pasted in.")]
        public bool CanPaste
        {
            get
            {
                bool bResult = m_bAllowAllInput;

                if (!bResult)
                {
                    try
                    {
                        IDataObject iData = Clipboard.GetDataObject();
                        if (iData != null)
                        {
                            if (iData.GetDataPresent(DataFormats.Text) ||
                                iData.GetDataPresent(DataFormats.UnicodeText))
                            {
                                string strText = (string)iData.GetData(DataFormats.UnicodeText);
                                try
                                {
                                    double dValue = double.Parse(strText);
                                    bResult = true;
                                }
                                catch (ArgumentNullException) { }
                                catch (FormatException) { }
                                catch (OverflowException) { }
                            }
                        }
                    }
                    catch (ExternalException)
                    {
                        bResult = false;
                    }
                }

                return bResult;
            }
        }

        [Browsable(false), Description("Returns the Value as an integer based on the RoundIntValue setting.")]
        public int IntValue
        {
            get
            {
                int iResult = 0;

                if (m_bRoundInt)
                    iResult = (int)Math.Round(m_dValue);
                else
                    iResult = (int)m_dValue;

                return iResult;
            }
        }

        #endregion

        #region Helper Functions

        private bool EnteringDuplicateChar(char ch)
        {
            bool bResult = false;
            if (base.Text.IndexOf(ch) >= 0)
            {
                if (SelectionLength > 0)
                {
                    if (SelectedText.IndexOf(ch) >= 0)
                        bResult = true;
                }
                else
                    bResult = true;
            }

            return bResult;
        }

        private void SetText(double dValue)
        {
            //Use "R" to use the max precision necessary
            string strValue = dValue.ToString("R");
            if (strValue != base.Text)
            {
                base.Text = strValue;
            }
        }

        public bool Validate()
        {
            try
            {
                m_dValue = double.Parse(base.Text);
                return ValidateValue();
            }
            catch (Exception)
            {
                SetText(m_dValue);
                return false;
            }
        }

        private bool ValidateValue()
        {
            bool bWasValid = Validate(ref m_dValue);

            if (!bWasValid)
            {
                Beep();
            }

            SetText(m_dValue);

            return bWasValid;
        }

        /// <summary>
        /// Validates the input value based on the current settings.
        /// </summary>
        /// <param name="dValue">The value to validate.  Contains the closest valid value on output.</param>
        /// <returns>True if the input was valid.</returns>
        public bool Validate(ref double dValue)
        {
            bool bWasValid = true;

            //Allow negative
            if (!m_bAllowNegative && dValue < 0)
            {
                dValue = Math.Abs(dValue);
                bWasValid = false;
            }

            //Min and Max Values
            if ((m_dMinValue != 0 || m_dMaxValue != 0) && (m_dMinValue < m_dMaxValue))
            {
                if (dValue < m_dMinValue)
                {
                    dValue = m_dMinValue;
                    bWasValid = false;
                }

                if (dValue > m_dMaxValue)
                {
                    dValue = m_dMaxValue;
                    bWasValid = false;
                }
            }

            //Allow float
            if (!m_bAllowFloat && Math.Round(dValue) != dValue)
            {
                dValue = IntValue;
                bWasValid = false;
            }

            return bWasValid;
        }

        #endregion

        #region Windows Event Handlers

        private void Beep()
        {
            if (m_bBeepOnError)
            {
                Utilities.PlaySound(SystemSound.Simple);
            }
        }

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            //Make sure registered delegates get called first.
            base.OnKeyPress(e);

            if (!m_bAllowAllInput)
            {
                char Key = e.KeyChar;
                if (Key >= ' ')
                {
                    string strSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                    char chSeparator = strSeparator.Length > 0 ? strSeparator[0] : '.';

                    if (Key == '-') //AllowNegative
                    {
                        //Only let them enter a negative sign at the beginning of the text.
                        if (!m_bAllowNegative || EnteringDuplicateChar('-') || SelectionStart != 0)
                        {
                            e.Handled = true;
                        }
                    }
                    else if (Key == chSeparator) //AllowFloat
                    {
                        if (!m_bAllowFloat || EnteringDuplicateChar(chSeparator))
                        {
                            e.Handled = true;
                        }
                    }
                    else if (Key < '0' || Key > '9') //It's another non-number
                    {
                        e.Handled = true;
                    }
                }

                if (e.Handled)
                {
                    Beep();
                }
            }
        }

        protected override void OnValidating(System.ComponentModel.CancelEventArgs e)
        {
            //Give any user delegates a chance to validate first.
            base.OnValidating(e);

            //If the user didn't cancel, then we can do our validation now.
            if (!e.Cancel)
            {
                Validate();
            }
        }

        protected override void OnLeave(System.EventArgs e)
        {
            //If CausesValidation is true, then our OnValidating
            //override will do the validation after the user has
            //had a shot at it.  If CausesValidation is false,
            //then we need to do it here because OnValidating
            //won't be called.
            if (!CausesValidation)
            {
                Validate();
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            bool bHandled = false;

            const int WM_PASTE = 0x0302;
            if (m.Msg == WM_PASTE)
            {
                //Make sure that a valid number gets pasted
                if (!CanPaste)
                {
                    bHandled = true;
                }
            }

            if (!bHandled)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_PASTE && !m_bAllowAllInput)
                {
                    Validate();
                }
            }
        }

        #endregion

        #region Private Data Members

        private double m_dValue;
        private double m_dMinValue;
        private double m_dMaxValue;

        private bool m_bAllowAllInput;
        private bool m_bAllowNegative;
        private bool m_bAllowFloat;
        private bool m_bRoundInt = true;
        private bool m_bBeepOnError;

        #endregion
    }
}
