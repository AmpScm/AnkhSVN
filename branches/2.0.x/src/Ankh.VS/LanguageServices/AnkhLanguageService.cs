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
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using System.ComponentModel.Design;
using Ankh.Selection;
using Ankh.UI;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

namespace Ankh.VS.LanguageServices
{
    [CLSCompliant(false)]
    public abstract partial class AnkhLanguageService : LanguageService, IAnkhServiceImplementation, IAnkhServiceProvider, IObjectWithAutomation, IAnkhIdleProcessor
    {
        readonly IAnkhServiceProvider _context;

        protected AnkhLanguageService(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            _context = context;
        }

        public virtual void OnPreInitialize()
        {
            // Initialize the language service api
            SetSite(GetService<IServiceContainer>());
        }

        public virtual void OnInitialize()
        {
            GetService<IAnkhPackage>().RegisterIdleProcessor(this);
        }

        void IAnkhIdleProcessor.OnIdle(AnkhIdleArgs e)
        {
            try
            {
                OnIdle(e.Periodic);
            }
            catch (COMException)
            {
                OnActiveViewChanged(null);
            }
            catch 
            { }
        }

        protected internal IAnkhServiceProvider Context
        {
            get { return _context; }
        }

        #region IAnkhServiceProvider Members

        public T GetService<T>() where T : class
        {
            return Context.GetService<T>();
        }

        public T GetService<T>(Type serviceType) where T : class
        {
            return Context.GetService<T>(serviceType);
        }

        #endregion

        #region IObjectWithAutomation Members

        public object AutomationObject
        {
            get { return Preferences; }
        }

        #endregion
    }

    abstract partial class AnkhViewFilter : ViewFilter
    {
        protected AnkhViewFilter(CodeWindowManager mgr, IVsTextView view)
            : base(mgr, view)
        {
        }
        public virtual void PrepareContextMenu(ref int menuId, ref Guid groupGuid, ref Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget target)
        {
            
        }

        public override void Dispose()
        {
            Type vf = typeof(ViewFilter);
            const BindingFlags privateField = BindingFlags.NonPublic | BindingFlags.Instance;

            if (TextView != null && !Marshal.IsComObject(TextView))
            {
                // Avoid exception on dispose
                FieldInfo textView = vf.GetField("textView", privateField);

                if (textView != null)
                    textView.SetValue(this, null);
            }

            FieldInfo nextTarget = vf.GetField("nextTarget", privateField);

            if (nextTarget != null)
            {
                object nt = nextTarget.GetValue(this);

                if (nt != null && !Marshal.IsComObject(nt))
                {
                    nextTarget.SetValue(this, null);
                }
            }

            base.Dispose();
        }
    }

    class AnkhSource : Source
    {
        public AnkhSource(AnkhLanguageService service, IVsTextLines textLines, Colorizer colorizer)
            : base(service, textLines, colorizer)
        {
        }

        public override void Dispose()
        {
            Type sc = typeof(Source);
            const BindingFlags privateField = BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo field;
            
            field = sc.GetField("colorState", privateField);

            if (field != null)
            {
                object v = field.GetValue(this);

                if (!Marshal.IsComObject(v))
                    field.SetValue(this, null);
            }

            field = sc.GetField("textLines", privateField);

            if (field != null)
            {
                object v = field.GetValue(this);

                if (!Marshal.IsComObject(v))
                    field.SetValue(this, null);
            }

            base.Dispose();
        }
    }
}
