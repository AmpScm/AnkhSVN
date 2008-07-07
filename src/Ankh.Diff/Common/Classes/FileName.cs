#region Copyright And Revision History

/*---------------------------------------------------------------------------

	FileName.cs
	Copyright © 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	5.22.2002	Created.

	BMenees	3.2.2003	Overloaded ToString().

	BMenees	5.28.2003	Added GetShortVersion() and GetLongVersion() methods.

	BMenees 3.12.2006	Added GetRelativePath.

-----------------------------------------------------------------------------*/

#endregion

#region Using Statements

using System;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;

#endregion

namespace Ankh.Diff
{
	/// <summary>
	/// Exposes various file name parts (e.g. extension, path, name) as properties
	/// and provides useful helper methods for working with file names.
	/// </summary>
	public sealed class FileName
	{
		#region Constructors

		/// <summary>
		/// Creates a new empty filename.
		/// </summary>
		public FileName()
		{
			m_strFileName = "";
		}

		/// <summary>
		/// Creates a new instance for the given filename.
		/// </summary>
		/// <param name="strFileName"></param>
		public FileName(string strFileName)
		{
			m_strFileName = strFileName;
		}

		#endregion

		#region Parts

		/// <summary>
		/// Returns the extension with a leading '.' (e.g. .txt for Test.txt).  
		/// If there's no extension, then an empty string is returned.
		/// </summary>
		public string Extension
		{
			get
			{
				char[] arChars = { '.', '\\', ':' };
				int i = m_strFileName.LastIndexOfAny(arChars);
				if ((i == c_iNPos) || (m_strFileName[i] != '.'))
					return ("");
				else
					return m_strFileName.Substring(i);
			}
			set
			{
				char[] arChars = { '.', '\\', ':' };
				int i = m_strFileName.LastIndexOfAny(arChars);
				if ((i == c_iNPos) || (m_strFileName[i] != '.'))
					m_strFileName += value;
				else
					m_strFileName = m_strFileName.Substring(0, i) + value;
			}
		}

		/// <summary>
		/// Returns the name of the file with the path removed.
		/// </summary>
		public string Name
		{
			get
			{
				char[] arChars = { '\\', ':' };
				int i = m_strFileName.LastIndexOfAny(arChars);
				if (i >= m_strFileName.Length)
					return "";
				else
					return m_strFileName.Substring(i+1);
			}
			set
			{
				m_strFileName = this.Path + value;
			}
		}

		/// <summary>
		/// Returns the name of the file with the path and extension removed.
		/// </summary>
		public string NameNoExt
		{
			get
			{
				string strName = this.Name;
				int i = strName.LastIndexOf('.');
				if (i == c_iNPos)
					return strName;
				else
					return strName.Substring(0, i);
			}
		}

		/// <summary>
		/// The path (e.g. C:\Test\ for C:\Test\File.txt).
		/// </summary>
		public string Path
		{
			get
			{
				char[] arChars = { '\\', ':' };
				int i = m_strFileName.LastIndexOfAny(arChars);
				if (i == c_iNPos)
					return "";
				else if (i >= m_strFileName.Length)
					return m_strFileName;
				else
					return m_strFileName.Substring(0, i+1);
			}
			set
			{
				string strPath = value;
				if (strPath.Length > 0 && !strPath.EndsWith("\\"))
					strPath += "\\";

				m_strFileName = strPath + this.Name;
			}
		}

		/// <summary>
		/// Drive letter (e.g. C:) or UNC server and share (e.g. \\TestServer\Share).
		/// </summary>
		public string Drive
		{
			get
			{
				int iLen = m_strFileName.Length;
				if ((iLen >= 2) && (m_strFileName[1] == ':'))
					return m_strFileName.Substring(0, 2);
				else if (iLen > 2 && m_strFileName.StartsWith(@"\\"))
				{
					int j = 0;
					int i = 2;
					while ((i < (iLen-1)) && (j < 2))
					{
						if (m_strFileName[i] == '\\') j++;
						if (j < 2) i++;
					}
					if (m_strFileName[i] == '\\') i--;

					return m_strFileName.Substring(0, i+1);
				}
				else 
					return "";
			}
		}

		/// <summary>
		/// Returns the full file name.
		/// </summary>
		public string FullName
		{
			get
			{
				return m_strFileName;
			}
			set
			{
				m_strFileName = value;
			}
		}

		/// <summary>
		/// Returns the full file name as a string.
		/// </summary>
		/// <returns>The full file name as a string.</returns>
		public override string ToString()
		{
			return m_strFileName;
		}

		/// <summary>
		/// Fully expanded name and path
		/// </summary>
		public string ExpandedName
		{
			get
			{
				return Windows.GetFullPathName(m_strFileName);
			}
		}

		#endregion

		#region Operators

		/// <summary>
		/// Allows a FileName instance to be used as a string.
		/// </summary>
		/// <param name="FN">A FileName instance</param>
		/// <returns>The string representation of the current filename.</returns>
		public static implicit operator string(FileName FN)
		{
			return FN.m_strFileName;
		}

		#endregion

		#region Other file functions

		/// <summary>
		/// Gets a FileInfo instance for the current file name.
		/// </summary>
		public FileInfo FileInfo
		{
			get 
			{ 
				return new FileInfo(m_strFileName); 
			}
		}

		/// <summary>
		/// Returns true if the file exists on disk.
		/// </summary>
		public bool Exists
		{
			get
			{
				return File.Exists(m_strFileName);
			}
		}

		/// <summary>
		/// Gets the short 8.3 version of the full FileName.
		/// </summary>
		public string GetShortVersion()
		{
			return Windows.GetShortPathName(m_strFileName);
		}

		/// <summary>
		/// Gets the long (non-8.3) version of the full FileName.
		/// </summary>
		public string GetLongVersion()
		{
			return Windows.GetLongPathName(m_strFileName);
		}

		/// <summary>
		/// Loads the text from a file.
		/// </summary>
		public string LoadFromFile()
		{
			using (StreamReader SR = new StreamReader(m_strFileName, Encoding.Default, true))
			{
				return SR.ReadToEnd();
			}
		}

		/// <summary>
		/// Loads the lines of text from a file.
		/// </summary>
		public string[] LoadLinesFromFile()
		{
			using (StreamReader SR = new StreamReader(m_strFileName, Encoding.Default, true))
			{
				StringCollection Lines = new StringCollection();

				while (SR.Peek() > -1) 
				{
					Lines.Add(SR.ReadLine());
				}

				string[] arLines = new string[Lines.Count];
				Lines.CopyTo(arLines, 0);
				return arLines;
			}
		}

		/// <summary>
		/// Saves the specified text to a file.
		/// </summary>
		/// <param name="strText">The text to save.</param>
		public void SaveToFile(string strText)
		{
			SaveToFile(strText, false);
		}

		/// <summary>
		/// Saves the specified text to a file.
		/// </summary>
		/// <param name="strText">The text to save.</param>
		/// <param name="bAppend">Whether the text is appended to an existing file.</param>
		public void SaveToFile(string strText, bool bAppend)
		{
			using (StreamWriter SW = new StreamWriter(m_strFileName, bAppend))
			{
				SW.Write(strText);
			}
		}

		/// <summary>
		/// Gets the current file name relative to the specified base path.
		/// It strips the common path directories and adds '..\' for each level
		/// up from the base path.
		/// </summary>
		/// <param name="strBasePath">The path to make the name relative to.</param>
		/// <returns>The relative path.</returns>
		public string GetRelativePath(string strBasePath)
		{
			//This needs to end with '\' so we can call fnBasePath.PathNoDrive
			//below and get all of the directory names.  If it doesn't end with
			//'\' then the last name will be left out when we split it.
			if (!strBasePath.EndsWith("\\"))
			{
				strBasePath = strBasePath + "\\";
			}

			//First make sure the files are on the same drive or UNC base.
			FileName fnBasePath = new FileName(strBasePath);
			string strBaseDrive = fnBasePath.Drive;
			if (strBaseDrive == Drive)
			{
				//Split the paths into directory names
				string strBasePathNoDrive = fnBasePath.PathNoDrive;
				string[] arBaseDirs = SplitDirectories(strBasePathNoDrive);
				int iBaseCount = arBaseDirs.Length;

				string strDestPathNoDrive = PathNoDrive;
				string[] arDestDirs = SplitDirectories(strDestPathNoDrive);
				int iDestCount = arDestDirs.Length;

				//Determine how many directory names they have in common.
				int i = 0;
				while (i < iBaseCount && i < iDestCount)
				{
					string strBaseDir = arBaseDirs[i];
					string strDestDir = arDestDirs[i];
					if (string.Compare(strBaseDir, strDestDir, true) == 0)
					{
						i++;
					}
					else
					{
						break;
					}
				}

				//Add relative directories for the remaining base directory names.
				StringBuilder SB = new StringBuilder();
				for (int j = i; j < iBaseCount; j++)
				{
					SB.Append("..\\");
				}

				//Add the remaining destination directory names.
				for (int j = i; j < iDestCount; j++)
				{
					SB.Append(arDestDirs[j]);
					SB.Append('\\');
				}

				//Add on the file name.
				SB.Append(Name);

				string strResult = SB.ToString();
				return strResult;
			}
			else
			{
				return m_strFileName;
			}
		}

		#endregion 

		#region Static Methods

		/// <summary>
		/// Returns the file path with a trailing '\'.
		/// </summary>
		/// <param name="strFullName">The full name of a file.</param>
		public static string GetPath(string strFullName)
		{
			FileName FN = new FileName(strFullName);
			return FN.Path;
		}

		/// <summary>
		/// Gets the extension for a file.  The return
		/// value is always lowercase with a leading '.'.
		/// </summary>
		/// <param name="strFullName">The full name of a file.</param>
		public static string GetExt(string strFullName)
		{
			FileName FN = new FileName(strFullName);
			return FN.Extension.ToLower();
		}

		/// <summary>
		/// Returns the name of the file with the path removed
		/// and optionally the extension removed.
		/// </summary>
		/// <param name="strFullName">The full name of a file.</param>
		/// <param name="bRemoveExt">Whether the extension should be removed.</param>
		public static string GetFileName(string strFullName, bool bRemoveExt)
		{
			FileName FN = new FileName(strFullName);
			if (bRemoveExt)
				return FN.NameNoExt;
			else
				return FN.Name;
		}

		#endregion

		#region Private Properties and Methods

		private string PathNoDrive
		{
			get
			{
				string strPath = Path;
				FileName fnPath = new FileName(strPath);
				string strDrive = fnPath.Drive;
				return strPath.Substring(strDrive.Length);
			}
		}

		private string[] SplitDirectories(string strPath)
		{
			List<string> lstDirs = new List<string>();
			string[] arDirs = strPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string strDir in arDirs)
			{
				lstDirs.Add(strDir);
			}

			return lstDirs.ToArray();
		}

		#endregion

		#region Private Data Members

		private string m_strFileName;
		private const int c_iNPos = -1;

		#endregion
	}
}
