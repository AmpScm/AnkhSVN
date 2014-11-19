// $Id$
//
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

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Ankh;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: CLSCompliant(true)]
[assembly: AssemblyTitle("AnkhSvn Package")]
[assembly: AssemblyDescription("AnkhSVN - Subversion Support for Visual Studio")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(AnkhId.AssemblyCompany)]
[assembly: AssemblyProduct(AnkhId.AssemblyProduct)]
[assembly: AssemblyCopyright(AnkhId.AssemblyCopyright)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: NeutralResourcesLanguage("en-US")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("2.6.*")]

// TODO: These should not be set in release code
#if !FOR_MSI
[assembly: InternalsVisibleTo("AnkhSvn.IntegrationTest, PublicKey=00240000048000009400000006020000002400005253413100040000010001000bcfcfdecfef04d5fd7772440779a083c6c3b83a8afd1c770f94b88f7f4fdc3153717ddd2f299c898835bd8e79d82eb0a3cd557d5782b8cf784d36bb2d59a4d65e96612585de3a10b41ad3101072e2c0f04b89ff9a6218b22275bef93f41fa98942adcebd4bb6fcf8404f56d25183b496dbfcedb70952abbcceecf478e1b72f3")]
[assembly: InternalsVisibleTo("AnkhSvn.UnitTest, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b1926fb2e39111a4c15d986cb4b17ca59e7000ee33ded2863b7e25d0c8be9418f80f0999c49ab2f556afba4996199e2803bbf2d1f0e35dedf3b3e0a4c8cfcec3974490d8a7149d235a7ba6384012d4021de4823a2739f1d3a15941cfafe562bb8523982fc0e98489aff60ed8a4a4d18ff6ace0fabfe535741695e91ae2b35eaf")]
#endif

// /TODO
