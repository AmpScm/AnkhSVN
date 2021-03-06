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

using System;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace IntegrationTests
{
    [TestClass]
    public class ReloadProjectTests
    {
        #region fields
        private delegate void ThreadInvoker();
        private TestContext _testContext;
        #endregion

        #region properties
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return _testContext; }
            set { _testContext = value; }
        }
        #endregion

        #region ctors
        public ReloadProjectTests()
        {
        }
        #endregion

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestInitialize]
        public void Initialize()
        {
            UIThreadInvoker.Initialize();
        }

        internal void ReloadProject(int n)
        {
            DTE dte = (DTE)VsIdeTestHostContext.ServiceProvider.GetService(typeof(DTE));

            Project proj = dte.Solution.Projects.Item(1);

            string uniqueName = proj.UniqueName;
            string name = proj.Name;

            IVsSolution solutionService = (IVsSolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(IVsSolution));

            IVsHierarchy hier;

            Marshal.ThrowExceptionForHR(solutionService.GetProjectOfUniqueName(uniqueName, out hier));

            solutionService.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, hier, 0);

            Window solutionExplorer = dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer) as Window;

            solutionExplorer.Activate();
            UIHierarchy uiHier = solutionExplorer.Object as UIHierarchy;

            UIHierarchyItem item = uiHier.GetItem(Path.GetFileNameWithoutExtension(dte.Solution.FileName) + "\\" + name);
            item.Select(vsUISelectionType.vsUISelectionTypeSelect);

            dte.ExecuteCommand("Project.ReloadProject", "");
        }

        [TestMethod]
        [HostType("VS IDE")]
        public void VBWinformsApplicationReload()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                //Solution and project creation parameters
                string solutionName = "VBWinApp";
                string projectName = "VBWinApp";

                //Template parameters
                string language = "VisualBasic";
                string projectTemplateName = "WindowsApplication.Zip";
                string itemTemplateName = "CodeFile.zip";
                string newFileName = "Test.vb";

                DTE dte = (DTE)VsIdeTestHostContext.ServiceProvider.GetService(typeof(DTE));

                TestUtils testUtils = new TestUtils();

                string testDir = char.ToLower(TestContext.TestDir[0]) + TestContext.TestDir.Substring(1);

                testUtils.CreateEmptySolution(testDir, solutionName);
                Assert.AreEqual<int>(0, testUtils.ProjectCount());

                //Add new  Windows application project to existing solution
                testUtils.CreateProjectFromTemplate(projectName, projectTemplateName, language, false);

                //Verify that the new project has been added to the solution
                Assert.AreEqual<int>(1, testUtils.ProjectCount());

                //Get the project
                Project project = dte.Solution.Item(1);
                Assert.IsNotNull(project);
                Assert.IsTrue(string.Compare(project.Name, projectName, StringComparison.InvariantCultureIgnoreCase) == 0);

                //Verify Adding new code file to project
                ProjectItem newCodeFileItem = testUtils.AddNewItemFromVsTemplate(project.ProjectItems, itemTemplateName, language, newFileName);
                Assert.IsNotNull(newCodeFileItem, "Could not create new project item");

                ReloadProject(0);
            });
        }
    }
}
