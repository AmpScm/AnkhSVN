using System;
using System.Collections.Generic;
using System.Text;

/* 
 * IWizardNode.cs
 * 
 * Copyright (c) 2008 CollabNet, Inc. ("CollabNet"), http://www.collab.net,
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, 
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 * See the License for the specific language governing permissions and 
 * limitations under the License.
 * 
 **/
namespace WizardFramework
{
    /// <summary>
    /// A wizard node is a placeholder for a wizard for use in a
    /// wizard selection page.
    /// </summary>
    /// <para><code>WizardSelectionPage</code> is an implementation
    /// of this interface.  Feel free to implement this interface if
    /// <code>WizardSelectionPage</code> does not suit your needs.</para>
    public interface IWizardNode
    {
        /// <summary>
        /// Disposes the wizard represented by this node.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Returns the wizard this placeholder represents.
        /// </summary>
        IWizard Wizard { get; }
    }
}
