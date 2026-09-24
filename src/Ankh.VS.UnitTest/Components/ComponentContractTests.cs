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
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using Ankh;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.VSPackage;
using Ankh.VSPackage.Attributes;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Components
{
    [TestFixture]
    public class ProductionAssemblySmokeTests
    {
        [TestCase("Ankh")]
        [TestCase("Ankh.Copilot")]
        [TestCase("Ankh.Diff")]
        [TestCase("Ankh.ExtensionPoints")]
        [TestCase("Ankh.Ids")]
        [TestCase("Ankh.ImageCatalog")]
        [TestCase("Ankh.Package")]
        [TestCase("Ankh.Scc")]
        [TestCase("Ankh.Services")]
        [TestCase("Ankh.UI")]
        [TestCase("Ankh.VS")]
        [TestCase("Ankh.VS.Interop")]
        public void ProductionAssemblyLoads(string assemblyName)
        {
            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));

            Assert.That(assembly, Is.Not.Null);
            Assert.That(assembly.GetName().Name, Is.EqualTo(assemblyName));
        }

        [Test]
        public void ImageCatalogContainsRepositoryExplorerImage()
        {
            Assembly assembly = Assembly.Load(new AssemblyName("Ankh.ImageCatalog"));

            CollectionAssert.Contains(
                assembly.GetManifestResourceNames(),
                "Ankh.ImageCatalog.Imgs.RepositoryExplorer.png");
        }
    }

    [TestFixture]
    public class IdContractTests
    {
        [Test]
        public void CoreGuidConstantsMatchTheirRuntimeGuids()
        {
            Assert.Multiple(() =>
            {
                Assert.That(AnkhId.PackageGuid, Is.EqualTo(Guid.Parse(AnkhId.PackageId)));
                Assert.That(AnkhId.CommandSetGuid, Is.EqualTo(Guid.Parse(AnkhId.CommandSet)));
                Assert.That(AnkhId.BmpGuid, Is.EqualTo(Guid.Parse(AnkhId.BmpId)));
                Assert.That(AnkhId.SccProviderGuid, Is.EqualTo(Guid.Parse(AnkhId.SccProviderId)));
                Assert.That(AnkhId.SccServiceGuid, Is.EqualTo(Guid.Parse(AnkhId.SccServiceId)));
                Assert.That(AnkhId.PendingChangeContextGuid, Is.EqualTo(Guid.Parse(AnkhId.PendingChangeViewContext)));
                Assert.That(AnkhId.DiffMergeContextGuid, Is.EqualTo(Guid.Parse(AnkhId.DiffMergeViewContext)));
                Assert.That(AnkhId.SccExplorerContextGuid, Is.EqualTo(Guid.Parse(AnkhId.SccExplorerViewContext)));
                Assert.That(AnkhId.LogContextGuid, Is.EqualTo(Guid.Parse(AnkhId.LogViewContext)));
                Assert.That(AnkhId.AnnotateContextGuid, Is.EqualTo(Guid.Parse(AnkhId.AnnotateContext)));
            });
        }

        [Test]
        public void CommandEnumRetainsRegisteredCommandSetGuid()
        {
            GuidAttribute attribute = typeof(AnkhCommand)
                .GetCustomAttributes(typeof(GuidAttribute), false)
                .Cast<GuidAttribute>()
                .Single();

            Assert.That(new Guid(attribute.Value), Is.EqualTo(AnkhId.CommandSetGuid));
        }

        [Test]
        public void DynamicCommandRangesRemainNonOverlapping()
        {
            Assert.Multiple(() =>
            {
                Assert.That((int)AnkhCommand.ListViewSortMax - (int)AnkhCommand.ListViewSort0, Is.EqualTo(64));
                Assert.That((int)AnkhCommand.ListViewGroup0, Is.GreaterThan((int)AnkhCommand.ListViewSortMax));
                Assert.That((int)AnkhCommand.ListViewGroupMax - (int)AnkhCommand.ListViewGroup0, Is.EqualTo(64));
                Assert.That((int)AnkhCommand.ListViewShow0, Is.GreaterThan((int)AnkhCommand.ListViewGroupMax));
                Assert.That((int)AnkhCommand.ListViewShowMax - (int)AnkhCommand.ListViewShow0, Is.EqualTo(64));
                Assert.That((int)AnkhCommand.MoveToExistingChangeListMax - (int)AnkhCommand.MoveToExistingChangeList0, Is.EqualTo(20));
            });
        }
    }

    [TestFixture]
    public class ExtensionPointContractTests
    {
        [Test]
        public void PreCommitArgsPreserveCommitStateAndCustomProperties()
        {
            var paths = new List<Uri>
            {
                new Uri("https://example.test/svn/project/trunk/file.cs")
            };
            var args = new PreCommitArgs(paths, 42, "Initial message");

            args.Cancel = true;
            args.CommitMessage = "Updated message";
            args.IssueText = "ANKH-42";
            args.SkipIssueVerify = true;
            args.CustomProperties["reviewed-by"] = "tester";

            Assert.Multiple(() =>
            {
                Assert.That(args.Paths, Is.SameAs(paths));
                Assert.That(args.Revision, Is.EqualTo(42));
                Assert.That(args.Cancel, Is.True);
                Assert.That(args.CommitMessage, Is.EqualTo("Updated message"));
                Assert.That(args.IssueText, Is.EqualTo("ANKH-42"));
                Assert.That(args.SkipIssueVerify, Is.True);
                Assert.That(args.CustomProperties["reviewed-by"], Is.EqualTo("tester"));
                Assert.That(args.CustomProperties, Is.SameAs(args.CustomProperties));
            });
        }

        [Test]
        public void PostCommitArgsExposeCommittedValues()
        {
            var paths = new List<Uri>
            {
                new Uri("https://example.test/svn/project/trunk/file.cs")
            };
            var args = new PostCommitArgs(paths, 73, "Committed message");

            Assert.Multiple(() =>
            {
                Assert.That(args.Paths, Is.SameAs(paths));
                Assert.That(args.Revision, Is.EqualTo(73));
                Assert.That(args.CommitMessage, Is.EqualTo("Committed message"));
            });
        }

        [Test]
        public void RepositorySettingsSeparateIdentityFromCustomValueEquality()
        {
            var leftProperties = new Dictionary<string, object>
            {
                { "project", "AnkhSVN" },
                { "enabled", true }
            };
            var rightProperties = new Dictionary<string, object>
            {
                { "project", "AnkhSVN" },
                { "enabled", false }
            };

            var left = new TestIssueRepositorySettings(
                "tracker",
                new Uri("https://issues.example.test/"),
                "repo-1",
                leftProperties);
            var right = new TestIssueRepositorySettings(
                "tracker",
                new Uri("https://issues.example.test/"),
                "repo-1",
                rightProperties);

            Assert.Multiple(() =>
            {
                Assert.That(left.Equals(right), Is.True);
                Assert.That(left.GetHashCode(), Is.EqualTo(right.GetHashCode()));
                Assert.That(left.ValueEquals(right), Is.False);
            });

            rightProperties["enabled"] = true;
            Assert.That(left.ValueEquals(right), Is.True);
        }

        sealed class TestIssueRepositorySettings : IssueRepositorySettings
        {
            readonly Uri _repositoryUri;
            readonly string _repositoryId;
            readonly IDictionary<string, object> _customProperties;

            public TestIssueRepositorySettings(
                string connectorName,
                Uri repositoryUri,
                string repositoryId,
                IDictionary<string, object> customProperties)
                : base(connectorName)
            {
                _repositoryUri = repositoryUri;
                _repositoryId = repositoryId;
                _customProperties = customProperties;
            }

            public override Uri RepositoryUri
            {
                get { return _repositoryUri; }
            }

            public override string RepositoryId
            {
                get { return _repositoryId; }
            }

            public override IDictionary<string, object> CustomProperties
            {
                get { return _customProperties; }
            }
        }
    }

    [TestFixture]
    public class VisualStudioInteropContractTests
    {
        [Test]
        public void CommandGroupsRetainVisualStudioGuidsAndContextIds()
        {
            Assert.Multiple(() =>
            {
                Assert.That(typeof(CMDSETID_HtmEdGrp).GUID,
                    Is.EqualTo(new Guid("D7E8C5E1-BDB8-11d0-9C88-0000F8040A53")));
                Assert.That(typeof(CMDSETID_XamlUI).GUID,
                    Is.EqualTo(new Guid("4c87b692-1202-46aa-b64c-ef01faec53da")));
                Assert.That((int)CMDSETID_HtmEdGrp.IDMX_HTM_SOURCE_ASPX, Is.EqualTo(0x35));
                Assert.That((int)CMDSETID_WinFormsDesigner.IDM_IF_CTXT_SELECTION, Is.EqualTo(0x500));
                Assert.That((int)CMDSETID_WinFormsDesigner.IDM_IF_CTXT_DOCUMENT_OUTLINE, Is.EqualTo(0x504));
                Assert.That((int)CMDSETID_XamlUI.IDM_XAML_EDITOR, Is.EqualTo(0x103));
            });
        }
    }

    [TestFixture]
    public class PackageRegistrationContractTests
    {
        static Assembly PackageAssembly
        {
            get { return typeof(AnkhSvnPackage).Assembly; }
        }

        static Type AttributeType(string name)
        {
            Type type = PackageAssembly.GetType(
                "Ankh.VSPackage.Attributes." + name,
                true,
                false);
            Assert.That(type, Is.Not.Null);
            return type;
        }

        static object GetProperty(object instance, string name)
        {
            PropertyInfo property = instance.GetType().GetProperty(name);
            Assert.That(property, Is.Not.Null);
            return property.GetValue(instance, null);
        }

        [Test]
        public void ThemeRegistrationNormalizesThemeGuidAndRegistryPath()
        {
            const string theme = "de3dbbcd-f642-433c-8353-8f1df4370aba";
            object attribute = Activator.CreateInstance(
                AttributeType("ProvideThemeInformationAttribute"),
                new object[] { theme, true });

            Assert.Multiple(() =>
            {
                Assert.That(GetProperty(attribute, "RegistryPath"),
                    Is.EqualTo(@"Extensions\AnkhSVN\Themes"));
                Assert.That(GetProperty(attribute, "Theme"),
                    Is.EqualTo(new Guid(theme).ToString("B")));
            });
        }

        [Test]
        public void OutputWindowRegistrationRoundTripsOptions()
        {
            object attribute = Activator.CreateInstance(
                AttributeType("ProvideOutputWindowAttribute"),
                new object[] { AnkhId.AnkhOutputPaneId, "#100" });

            Type type = attribute.GetType();
            type.GetProperty("Name").SetValue(attribute, "AnkhSVN", null);
            type.GetProperty("InitiallyInvisible").SetValue(attribute, true, null);
            type.GetProperty("ClearWithSolution").SetValue(attribute, true, null);

            Assert.Multiple(() =>
            {
                Assert.That(GetProperty(attribute, "OutputWindowId"),
                    Is.EqualTo(new Guid(AnkhId.AnkhOutputPaneId)));
                Assert.That(GetProperty(attribute, "ResourceId"), Is.EqualTo("#100"));
                Assert.That(GetProperty(attribute, "Name"), Is.EqualTo("AnkhSVN"));
                Assert.That(GetProperty(attribute, "InitiallyInvisible"), Is.True);
                Assert.That(GetProperty(attribute, "ClearWithSolution"), Is.True);
            });
        }

        [Test]
        public void SourceControlCommandRegistrationUsesEnumCommandIdentity()
        {
            object attribute = Activator.CreateInstance(
                AttributeType("ProvideSourceControlCommandAttribute"),
                new object[]
                {
                    AnkhId.SccProviderId,
                    SccProviderCommand.Open,
                    AnkhCommand.FileSccOpenFromSubversion
                });

            Assert.Multiple(() =>
            {
                Assert.That(GetProperty(attribute, "RegGuid"), Is.EqualTo(AnkhId.SccProviderGuid));
                Assert.That(GetProperty(attribute, "UINamePkg"), Is.EqualTo(AnkhId.PackageGuid));
            });
        }
    }
}
