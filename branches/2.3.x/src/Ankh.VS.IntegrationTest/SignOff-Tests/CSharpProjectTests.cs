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

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace IntegrationTests
{
	[TestClass]
	public class CSharpProjectTests
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
		public CSharpProjectTests()
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
        [TestInitialize()]
        public void Initialize()
        {
            UIThreadInvoker.Initialize();
        }
		#endregion

		[TestMethod]
		[HostType("VS IDE")]
		public void WinformsApplication()
		{
			UIThreadInvoker.Invoke((ThreadInvoker)delegate()
			{
				TestUtils testUtils = new TestUtils();

				testUtils.CreateEmptySolution(TestContext.TestDir, "CreateWinformsApplication");
				Assert.AreEqual<int>(0, testUtils.ProjectCount());

				//Create Winforms application project
				//TestUtils.CreateProjectFromTemplate("MyWindowsApp", "Windows Application", "CSharp", false);
				//Assert.AreEqual<int>(1, TestUtils.ProjectCount());

				//TODO Verify that we can debug launch the application

				//TODO Set Break point and verify that will hit

				//TODO Verify Adding new project item to project

			});
		}

	}
}
