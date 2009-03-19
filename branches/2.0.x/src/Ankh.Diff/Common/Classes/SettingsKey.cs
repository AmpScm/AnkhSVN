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

	Copyright (c) 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	$History: SettingsKey.cs $
	
	*****************  Version 3  *****************
	User: Bill         Date: 3/05/08    Time: 8:31p
	Updated in $/CSharp/Menees/Classes
	Added support for string arrays and enums.
	
	*****************  Version 2  *****************
	User: Bill         Date: 11/06/05   Time: 12:53p
	Updated in $/CSharp/Menees/Classes
	Renamed and refactored to hide the fact that it uses the registry.

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	4.26.2003	Created.

-----------------------------------------------------------------------------*/

#endregion

#region Using Directives

using System;
using Microsoft.Win32;
using System.Drawing;
using System.Text;

#endregion

namespace Ankh.Diff
{
    public sealed class SettingsKey : IDisposable
    {
        #region Constructors

        public SettingsKey(string strKeyPath)
            : this(strKeyPath, true)
        {
        }

        public SettingsKey(string strKeyPath, bool bUserLevelSettings)
        {
            RegistryKey RootKey = bUserLevelSettings ? Registry.CurrentUser : Registry.LocalMachine;

            //Strip leading '\' because the registry classes won't work with a path that has a leading backslash.
            if (strKeyPath.StartsWith("\\"))
            {
                strKeyPath = strKeyPath.Length > 1 ? strKeyPath.Substring(1) : "";
            }

            m_Key = RootKey.CreateSubKey(strKeyPath);
        }

        private SettingsKey(RegistryKey Key)
        {
            m_Key = Key;
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                return m_Key.Name;
            }
        }

        public int SubKeyCount
        {
            get
            {
                return m_Key.SubKeyCount;
            }
        }

        public int ValueCount
        {
            get
            {
                return m_Key.ValueCount;
            }
        }

        #endregion

        #region Public Methods To Get/Set Values

        public string GetString(string strName, string strDefault)
        {
            return Convert.ToString(m_Key.GetValue(strName, strDefault));
        }

        public void SetString(string strName, string strValue)
        {
            m_Key.SetValue(strName, strValue);
        }

        public int GetInt(string strName, int iDefault)
        {
            return Convert.ToInt32(m_Key.GetValue(strName, iDefault));
        }

        public void SetInt(string strName, int iValue)
        {
            m_Key.SetValue(strName, iValue);
        }

        public bool GetBool(string strName, bool bDefault)
        {
            return GetInt(strName, bDefault ? 1 : 0) != 0;
        }

        public void SetBool(string strName, bool bValue)
        {
            SetInt(strName, bValue ? 1 : 0);
        }

        public Color GetColor(string strName, Color clrDefault)
        {
            return Color.FromArgb(GetInt(strName, clrDefault.ToArgb()));
        }

        public void SetColor(string strName, Color clrValue)
        {
            SetInt(strName, clrValue.ToArgb());
        }

        public double GetDouble(string strName, double dDefault)
        {
            byte[] arBytes = m_Key.GetValue(strName) as byte[];
            if (arBytes != null)
            {
                return BitConverter.ToDouble(arBytes, 0);
            }
            else
                return dDefault;
        }

        public void SetDouble(string strName, double dValue)
        {
            byte[] arBytes = BitConverter.GetBytes(dValue);
            m_Key.SetValue(strName, arBytes);
        }

        public string[] GetStrings(string strName, string[] arDefault)
        {
            string[] arResult = (string[])m_Key.GetValue(strName, arDefault);
            return arResult;
        }

        public void SetStrings(string strName, string[] arValue)
        {
            m_Key.SetValue(strName, arValue, RegistryValueKind.MultiString);
        }

        public T GetEnum<T>(string strName, T eDefault) where T : struct
        {
            try
            {
                string strValue = GetString(strName, eDefault.ToString());
                T eResult = (T)Enum.Parse(typeof(T), strValue);
                return eResult;
            }
            catch (ArgumentException)
            {
                return eDefault;
            }
        }

        public void SetEnum<T>(string strName, T eValue) where T : struct
        {
            SetString(strName, eValue.ToString());
        }

        #endregion

        #region Public Methods To Get Key MetaData

        public void DeleteSubKey(string strName)
        {
            m_Key.DeleteSubKeyTree(strName);
        }

        public void DeleteValue(string strName)
        {
            m_Key.DeleteValue(strName);
        }

        public SettingsKey FindSubKey(string strName)
        {
            RegistryKey SubKey = m_Key.OpenSubKey(strName, true);
            if (SubKey != null)
            {
                return new SettingsKey(SubKey);
            }
            else
            {
                return null;
            }
        }

        public SettingsKey GetSubKey(string strName)
        {
            return new SettingsKey(m_Key.CreateSubKey(strName));
        }

        public string[] GetSubKeyNames()
        {
            return m_Key.GetSubKeyNames();
        }

        public string[] GetValueNames()
        {
            return m_Key.GetValueNames();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_Key.Close();
        }

        #endregion

        #region Private Data Members

        private RegistryKey m_Key;

        #endregion
    }
}
