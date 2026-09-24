// Copyright 2008-2009 The AnkhSVN Project
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

using System.Linq;
using Ankh;
using Ankh.Commands;
using NUnit.Framework;

namespace UnitTestProject.MenuItemTests
{
    [TestFixture]
    public class MenuItemTest
    {
        /// <summary>
        /// Verifies that Refresh remains discoverable by the attribute-driven
        /// command mapper used to populate Visual Studio commands.
        /// </summary>
        [Test]
        public void InitializeMenuCommand()
        {
            CommandAttribute[] mappings = typeof(AnkhModule).Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => typeof(ICommandHandler).IsAssignableFrom(type))
                .SelectMany(type => type
                    .GetCustomAttributes(typeof(CommandAttribute), false)
                    .Cast<CommandAttribute>())
                .ToArray();

            bool refreshMapped = mappings.Any(mapping =>
                mapping.Command == AnkhCommand.Refresh ||
                (mapping.LastCommand != AnkhCommand.None &&
                 (int)AnkhCommand.Refresh >= (int)mapping.Command &&
                 (int)AnkhCommand.Refresh <= (int)mapping.LastCommand));

            Assert.That(refreshMapped, Is.True,
                "The Refresh command must remain registered with an ICommandHandler.");
        }
    }
}
