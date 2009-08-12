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
using System.ComponentModel;
using Ankh.UI;

namespace Ankh.Scc
{
    partial class SvnItemData
    {
        [TypeConverter(typeof(ChangeListTypeConverter)), ImmutableObject(true)]
        public sealed class SvnChangeList
        {
            readonly string _list;

            public SvnChangeList(string list)
            {
                if (string.IsNullOrEmpty(list))
                    throw new ArgumentNullException("list");

                _list = list;
            }

            public string List
            {
                get { return _list; }
            }

            public override string ToString()
            {
                return _list;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SvnChangeList);
            }

            public bool Equals(SvnChangeList obj)
            {
                if (obj == null)
                    return false;

                return List == obj.List;
            }

            public override int GetHashCode()
            {
                return StringComparer.Ordinal.GetHashCode(List);
            }

            public static implicit operator string(SvnChangeList list)
            {
                if (list == null)
                    return null;
                return list.List;
            }

            public static implicit operator SvnChangeList(string list)
            {
                if (string.IsNullOrEmpty(list))
                    return null;
                return new SvnChangeList(list);
            }
        }

        sealed class ChangeListTypeConverter : StringConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (typeof(string).IsAssignableFrom(sourceType) || sourceType == typeof(SvnChangeList))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                SvnChangeList chl = value as SvnChangeList;
                if (chl != null)
                    return chl;

                string cl = value as string;

                if (cl != null)
                    return string.IsNullOrEmpty(cl) ? null : new SvnChangeList(cl);

                return base.ConvertFrom(context, culture, value);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType.IsAssignableFrom(typeof(string)) || destinationType == typeof(SvnChangeList))
                    return true;

                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                SvnChangeList chl = value as SvnChangeList;
                string listName = chl != null ? chl.List : null;

                if (destinationType.IsAssignableFrom(typeof(string)))
                    return listName;
                else if (destinationType == typeof(SvnChangeList))
                    return chl;

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                List<SvnChangeList> names = new List<SvnChangeList>();

                IAnkhPackage package = context.GetService(typeof(IAnkhPackage)) as IAnkhPackage;

                if (package != null)
                {
                    IPendingChangesManager pcm = package.GetService<IPendingChangesManager>();

                    foreach (string cl in pcm.GetSuggestedChangeLists())
                    {
                        names.Add(cl);
                    }
                }

                names.Add("ignore-on-commit");

                StandardValuesCollection svc = new StandardValuesCollection(names);
                return svc;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return false;
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
        }
    }
}
