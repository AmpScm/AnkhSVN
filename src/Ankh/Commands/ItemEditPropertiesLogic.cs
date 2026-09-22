// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;

using SharpSvn;

namespace Ankh.Commands
{
    internal enum PropertyPersistenceAction
    {
        None,
        Delete,
        SetString,
        SetRaw
    }

    internal static class ItemEditPropertiesLogic
    {
        public static bool HasPersistableChanges(
            IEnumerable<bool> shouldPersist)
        {
            if (shouldPersist == null)
                throw new ArgumentNullException("shouldPersist");

            foreach (bool value in shouldPersist)
            {
                if (value)
                    return true;
            }

            return false;
        }

        public static PropertyPersistenceAction GetPersistenceAction(
            bool shouldPersist,
            SvnPropertyValue originalValue,
            SvnPropertyValue value)
        {
            if (!shouldPersist)
                return PropertyPersistenceAction.None;

            if (value == null)
            {
                return originalValue != null
                    ? PropertyPersistenceAction.Delete
                    : PropertyPersistenceAction.None;
            }

            if (value.ValueEquals(originalValue))
                return PropertyPersistenceAction.None;

            return value.StringValue != null
                ? PropertyPersistenceAction.SetString
                : PropertyPersistenceAction.SetRaw;
        }
    }
}
