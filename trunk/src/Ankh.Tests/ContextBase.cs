// $Id$
//
// Copyright 2004-2008 The AnkhSVN Project
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
using Ankh.Configuration;
using EnvDTE;
using NSvn.Core;
using System.Collections;
using System.Windows.Forms;
using System.IO;

using IServiceProvider = System.IServiceProvider;
using SharpSvn;
using Ankh.UI.Services;
using Ankh.UI;
using System.Diagnostics;

namespace Ankh.Tests
{


    public class ErrorHandlerImpl : IAnkhErrorHandler
    {
        public Exception Exception;

        #region IErrorHandler Members

        public virtual void OnError(Exception ex)
        {
            this.Exception = ex;
        }

        public virtual void SendReport()
        {
            // empty
        }

        public void Write(string message, Exception ex, TextWriter writer)
        {
            // empty
        }

        #endregion

        #region IErrorHandler Members

        public void LogException(Exception exception, string message, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    /// <summary>
    /// An ISynchronizeInvoke for which InvokeRequired will always return false.
    /// </summary>
    public class NoSynch : System.ComponentModel.ISynchronizeInvoke
    {
        #region ISynchronizeInvoke Members

        public object EndInvoke(IAsyncResult result)
        {
            // TODO:  Add NoSynch.EndInvoke implementation
            return null;
        }

        public object Invoke(Delegate method, object[] args)
        {
            // TODO:  Add NoSynch.Invoke implementation
            return null;
        }

        public bool InvokeRequired
        {
            get
            {
                return false;
            }
        }

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            // TODO:  Add NoSynch.BeginInvoke implementation
            return null;
        }

        #endregion

    }

    public class UIShellImpl : IUIShell
    {        
        PathSelectorResult IUIShell.ShowPathSelector(PathSelectorInfo info)
        {
            throw new NotImplementedException();
        }
    }

}
