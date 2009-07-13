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

	Utilities.cs
	Copyright (c) 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	3.2.2003	Created.

-----------------------------------------------------------------------------*/

#endregion

#region Using Directives

using System;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.Reflection;
using System.Text;
using System.Drawing;
using System.Globalization;

#endregion

namespace Ankh.Diff
{
    /// <summary>
    /// Contains static helper functions.
    /// </summary>
    public sealed class Utilities
    {
        #region Public Methods And Properties

        public static void ShellExecute(IWin32Window Owner, string strFileName, string strVerb)
        {
            ProcessStartInfo SI = new ProcessStartInfo();

            SI.ErrorDialog = true;
            if (Owner != null)
            {
                SI.ErrorDialogParentHandle = Owner.Handle;
            }
            SI.FileName = strFileName;
            SI.UseShellExecute = true;
            SI.Verb = strVerb;

            try
            {
                Process.Start(SI);
            }
            catch(Exception e)
            {
                // Kills VS if it fails
                MessageBox.Show(e.ToString());
            } 
        }

        public static string DefaultFolderOpenAction
        {
            get
            {
                using (RegistryKey Key = Registry.ClassesRoot.CreateSubKey(@"Folder\Shell"))
                {
                    //The default action is in the "(default)" or unnamed value.
                    return (string)Key.GetValue("", "open");
                }
            }
        }

        public static string GetCallingAssemblyVersion()
        {
            return GetAssemblyVersion(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Gets the assembly version as a string.  Typically, you'll pass Assembly.GetExecutingAssembly().
        /// </summary>
        /// <param name="Asm">The assembly to get the version for</param>
        /// <returns></returns>
        public static string GetAssemblyVersion(Assembly Asm)
        {
            //For every attribute except AssemblyVersion, we could use
            //Assembly.GetCustomAttributes().  For the version, we have
            //to use Assembly.GetName().Version.
            AssemblyName Name = Asm.GetName();
            Version Ver = Name.Version;
            return String.Format("{0}.{1}.{2}.{3}", Ver.Major, Ver.Minor, Ver.Build, Ver.Revision);
        }

        public static void ShowError(string strMessage)
        {
            MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowError(IWin32Window Owner, string strMessage)
        {
            MessageBox.Show(Owner, strMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string StripQuotes(string strText)
        {
            return StripQuotes(strText, "\"", "\"");
        }

        public static string StripQuotes(string strText, string strQuote)
        {
            return StripQuotes(strText, strQuote, strQuote);
        }

        public static string StripQuotes(string strText, string strOpenQuote, string strCloseQuote)
        {
            int iStartIndex = 0;
            int iLength = strText.Length;

            if (strText.StartsWith(strOpenQuote))
            {
                int iQuoteLength = strOpenQuote.Length;
                iStartIndex += iQuoteLength;
                iLength -= iQuoteLength;
            }

            if (strText.EndsWith(strCloseQuote))
            {
                iLength -= strCloseQuote.Length;
            }

            return strText.Substring(iStartIndex, iLength);
        }

        public static string EnsureQuotes(string strText)
        {
            return EnsureQuotes(strText, "\"");
        }

        public static string EnsureQuotes(string strText, string strQuote)
        {
            if (!strText.StartsWith(strQuote))
            {
                strText = strQuote + strText;
            }

            if (!strText.EndsWith(strQuote))
            {
                strText += strQuote;
            }

            return strText;
        }

        public static int IndexOfNoCase(string strText, string strSubstring)
        {
            return IndexOfNoCase(strText, strSubstring, 0, strText.Length);
        }

        public static int IndexOfNoCase(string strText, string strSubstring, int iStartIndex)
        {
            return IndexOfNoCase(strText, strSubstring, iStartIndex, strText.Length - iStartIndex);
        }

        public static int IndexOfNoCase(string strText, string strSubstring, int iStartIndex, int iCount)
        {
            int iResult = CultureInfo.CurrentCulture.CompareInfo.IndexOf(strText, strSubstring, iStartIndex, iCount, CompareOptions.IgnoreCase);
            return iResult;
        }

        public static string ReplaceNoCase(string strText, string strOldValue, string strNewValue)
        {
            string strLowerText = strText.ToLower();
            string strLowerOldValue = strOldValue.ToLower();

            int iCurrentIndex = strLowerText.IndexOf(strLowerOldValue);
            if (iCurrentIndex < 0)
            {
                return strText;
            }

            int iTextLength = strText.Length;
            int iOldValueLength = strOldValue.Length;
            int iPreviousIndex = 0;
            StringBuilder SB = new StringBuilder(strText.Length);
            while (iCurrentIndex >= 0)
            {
                if (iCurrentIndex > iPreviousIndex)
                {
                    SB.Append(strText.Substring(iPreviousIndex, iCurrentIndex - iPreviousIndex));
                }
                SB.Append(strNewValue);

                iCurrentIndex += iOldValueLength;
                iPreviousIndex = iCurrentIndex;
                if ((iCurrentIndex + 1) < iTextLength)
                {
                    iCurrentIndex = strLowerText.IndexOf(strLowerOldValue, iCurrentIndex + 1);
                }
                else
                {
                    break;
                }
            }

            if (iPreviousIndex < strText.Length)
            {
                SB.Append(strText.Substring(iPreviousIndex));
            }

            string strResult = SB.ToString();
            return strResult;
        }

        public static string ReplaceControlCharacters(string strText)
        {
            return ReplaceControlCharacters(strText, ' ');
        }

        public static string ReplaceControlCharacters(string strText, char chReplacementChar)
        {
            if (strText.Length == 0)
            {
                return strText;
            }

            StringBuilder SB = new StringBuilder(strText);
            int iLength = strText.Length;
            for (int i = 0; i < iLength; i++)
            {
                if (Char.IsControl(SB[i]))
                {
                    SB[i] = chReplacementChar;
                }
            }

            return SB.ToString();
        }

        public static bool PlaySound(SystemSound eSound)
        {
            //This uses PlaySound rather than MessageBeep because MessageBeep doesn't
            //work on Windows XP (at least not for me).
            string strFileName = GetSoundFileName(eSound);
            return PlaySound(strFileName);
        }

        public static bool PlaySound(string strWavFileName)
        {
            return NativeMethods.PlaySound(strWavFileName);
        }

        public static bool PlaySound(int iFrequency, int iDuration)
        {
            return NativeMethods.Beep(iFrequency, iDuration);
        }

        public static int Compare(float fX, float fY, float fTolerance)
        {
            float fDifference = fX - fY;
            if (Math.Abs(fDifference) <= fTolerance)
            {
                return 0;
            }
            else
            {
                return Math.Sign(fDifference);
            }
        }

        public static bool CompareEqual(PointF pt1, PointF pt2, float fTolerance)
        {
            return Compare(pt1.X, pt2.X, fTolerance) == 0 && Compare(pt1.Y, pt2.Y, fTolerance) == 0;
        }

        public static bool Between(float fArg, float fLowerBound, float fUpperBound, float fTolerance)
        {
            return Compare(fArg, fLowerBound, fTolerance) >= 0 && Compare(fArg, fUpperBound, fTolerance) <= 0;
        }

        public static float GetAngle(PointF ptOrigin, PointF ptTarget)
        {
            ptTarget.X -= ptOrigin.X;
            ptTarget.Y -= ptOrigin.Y;
            double dRadians = Math.Atan2(ptTarget.Y, ptTarget.X);
            return (float)(dRadians / Math.PI * 180.0);
        }

        public static bool IsEmpty(string strValue)
        {
            return strValue == null || strValue.Length == 0;
        }

        #endregion

        #region Private Methods

        private static string GetSoundFileName(SystemSound eSound)
        {
            switch (eSound)
            {
                case SystemSound.Error: return "SystemHand";
                case SystemSound.Question: return "SystemQuestion";
                case SystemSound.Warning: return "SystemExclamation";
                case SystemSound.Information: return "SystemAsterisk";
                case SystemSound.Default: return "SystemDefault";
                default: return ".Default";
            }
        }

        private Utilities()
        {
            // Private so no one can create an instance.
        }

        #endregion
    }
}
