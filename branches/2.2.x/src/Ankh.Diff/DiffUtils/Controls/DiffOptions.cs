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

	DiffOptions.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Drawing;
using Ankh.Diff.DiffUtils;
using System.Diagnostics;

namespace Ankh.Diff.DiffUtils.Controls
{
    /// <summary>
    /// Summary description for DiffOptions.
    /// </summary>
    public class DiffOptions
    {
        #region Public Members

        public static Color InsertedColor
        {
            get
            {
                return s_InsertedColor;
            }
            set
            {
                if (s_InsertedColor != value)
                {
                    BeginUpdate();
                    s_InsertedColor = value;
                    s_bChanged = true;
                    EndUpdate();
                }
            }
        }

        public static Color DeletedColor
        {
            get
            {
                return s_DeletedColor;
            }
            set
            {
                if (s_DeletedColor != value)
                {
                    BeginUpdate();
                    s_DeletedColor = value;
                    s_bChanged = true;
                    EndUpdate();
                }
            }
        }

        public static Color ChangedColor
        {
            get
            {
                return s_ChangedColor;
            }
            set
            {
                if (s_ChangedColor != value)
                {
                    BeginUpdate();
                    s_ChangedColor = value;
                    s_bChanged = true;
                    EndUpdate();
                }
            }
        }

        public static Color GetColorForEditType(EditType ET)
        {
            switch (ET)
            {
                case EditType.Change:
                    return ChangedColor;
                case EditType.Insert:
                    return InsertedColor;
                case EditType.Delete:
                    return DeletedColor;
            }

            Debug.Assert(false);
            return Color.Transparent;
        }

        public static int SpacesPerTab
        {
            get
            {
                return s_iSpacesPerTab;
            }
            set
            {
                if (s_iSpacesPerTab != value)
                {
                    BeginUpdate();
                    s_iSpacesPerTab = value;
                    s_bChanged = true;
                    EndUpdate();
                }
            }
        }

        public static void BeginUpdate()
        {
            s_iUpdateLevel++;
        }

        public static void EndUpdate()
        {
            s_iUpdateLevel--;

            if (s_iUpdateLevel == 0 && s_bChanged)
            {
                s_bChanged = false;

                if (OptionsChanged != null)
                {
                    OptionsChanged(null, EventArgs.Empty);
                }
            }
        }

        public static Color DefaultInsertedColor
        {
            get
            {
                return s_DefaultInsertedColor;
            }
        }

        public static Color DefaultDeletedColor
        {
            get
            {
                return s_DefaultDeletedColor;
            }
        }

        public static Color DefaultChangedColor
        {
            get
            {
                return s_DefaultChangedColor;
            }
        }

        public static void Load(SettingsKey Key)
        {
            BeginUpdate();
            try
            {
                InsertedColor = Color.FromArgb(Key.GetInt("InsertedColor", s_DefaultInsertedColor.ToArgb()));
                DeletedColor = Color.FromArgb(Key.GetInt("DeletedColor", s_DefaultDeletedColor.ToArgb()));
                ChangedColor = Color.FromArgb(Key.GetInt("ChangedColor", s_DefaultChangedColor.ToArgb()));
                SpacesPerTab = Key.GetInt("SpacesPerTab", s_iDefaultSpacesPerTab);
            }
            finally
            {
                EndUpdate();
            }
        }

        public static void Save(SettingsKey Key)
        {
            Key.SetInt("InsertedColor", InsertedColor.ToArgb());
            Key.SetInt("DeletedColor", DeletedColor.ToArgb());
            Key.SetInt("ChangedColor", ChangedColor.ToArgb());
            Key.SetInt("SpacesPerTab", SpacesPerTab);
        }

        public static event EventHandler OptionsChanged;

        #endregion

        #region Private Methods

        private DiffOptions()
        {
            //So no one can create an instance.
        }

        #endregion

        #region Private Data Members

        private static readonly Color s_DefaultInsertedColor = Color.PaleTurquoise;
        private static readonly Color s_DefaultDeletedColor = Color.Pink;
        private static readonly Color s_DefaultChangedColor = Color.PaleGreen;
        private static readonly int s_iDefaultSpacesPerTab = 4;

        private static Color s_InsertedColor = s_DefaultInsertedColor;
        private static Color s_DeletedColor = s_DefaultDeletedColor;
        private static Color s_ChangedColor = s_DefaultChangedColor;
        private static int s_iSpacesPerTab = s_iDefaultSpacesPerTab;

        private static int s_iUpdateLevel = 0;
        private static bool s_bChanged = false;

        #endregion
    }
}
