// $Id$
//
// Copyright 2009 The AnkhSVN Project
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

using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.VSSelectionControls;
using SharpSvn;
using System.Globalization;

namespace Ankh.UI.PropertyEditors
{
    public class PropertyEditItem : SmartListViewItem
    {
        readonly string _name;

        public PropertyEditItem(SmartListView listView, string name)
            : base(listView)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Text = _name = name;
        }

        public string PropertyName
        {
            get { return _name; }
        }

        public void Refresh()
        {
            SetValues(
                PropertyName,
                UpdateDescription,
                CreateValue(BaseValue),
                CreateValue(Value));
        }

        SvnPropertyValue _baseValue, _value, _originalValue;
        /// <summary>
        /// Gets or sets the base value.
        /// </summary>
        /// <value>The base value.</value>
        public SvnPropertyValue BaseValue
        {
            get { return _baseValue; }
            set { _baseValue = value; }
        }

        /// <summary>
        /// Gets the update description.
        /// </summary>
        /// <value>The update description.</value>
        public string UpdateDescription
        {
            get
            {
                string newDescription;
                if (Value == null)
                {
                    newDescription = BaseValue == null ?
                        PropertyEditStrings.StateUnmodified // invalid state, item should be deleted
                        : PropertyEditStrings.StateDeleted;
                }
                else
                {
                    newDescription = BaseValue == null ?
                        PropertyEditStrings.StateNew
                        : Value.ValueEquals(BaseValue) ?
                            PropertyEditStrings.StateUnmodified
                            : PropertyEditStrings.StateModified;
                }
                return newDescription;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public SvnPropertyValue Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public SvnPropertyValue OriginalValue
        {
            get { return _originalValue; }
            set { _originalValue = value; }
        }

        public bool IsDeleted
        {
            get { return Value == null && BaseValue != null; }
        }

        public bool IsModified
        {
            get { return Value != null && (BaseValue == null || !ValuesEqual()); }
        }

        public bool ShouldPersist
        {
            get { return (OriginalValue != null) != (Value != null) || Value == null || !Value.ValueEquals(OriginalValue); }
        }

        public static string CreateValue(SvnPropertyValue value)
        {
            if (value == null)
                return "";
            else if (value.StringValue != null)
                return value.StringValue.Replace("\r", "").Replace("\n", "\x23CE");
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("0x");
                int n = 0;
                foreach (byte b in value.RawValue)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0:x2}", b);
                    if (n++ > 128)
                    {
                        sb.Append("...");
                        break;
                    }
                }

                return sb.ToString();
            }
        }

        public bool ValuesEqual()
        {
            if (Value == null || BaseValue == null)
                return Value == BaseValue;

            if (Value.StringValue != null || BaseValue.StringValue != null)
                return BaseValue.StringValue == Value.StringValue;

            if (Value.RawValue.Count != BaseValue.RawValue.Count)
                return false;

            IEnumerator<byte> ve = Value.RawValue.GetEnumerator();
            IEnumerator<byte> bve = BaseValue.RawValue.GetEnumerator();

            while (ve.MoveNext() && bve.MoveNext())
            {
                if (ve.Current != bve.Current)
                    return false;
            }
            return true; // Lengths are equal -> OK
        }


    }
}
