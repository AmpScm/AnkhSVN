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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Ankh.Scc
{
    public sealed class EnlistmentState
    {
        readonly bool _needLocation;
        readonly bool _needDebugLocation;
        string _location;
        string _localLocation;
        string _debugLocation;
        string _localDebugLocation;

        public class EnlistmentEventArgs : CancelEventArgs
        {
            readonly EnlistmentState _state;
            public EnlistmentEventArgs(EnlistmentState state)
            {
                if (state == null)
                    throw new ArgumentNullException("state");

                _state = state;
            }

            public EnlistmentState State
            {
                get { return _state; }
            }
        }

        public EnlistmentState(bool needLocation, bool needDebugLocation)
        {
            _needLocation = needLocation;
            _needDebugLocation = needDebugLocation;
        }

        public bool NeedLocation
        {
            get { return _needLocation; }
        }

        public bool NeedDebugLocation
        {
            get { return _needDebugLocation; }
        }

        public event EventHandler<EnlistmentEventArgs> BrowseLocation;
        public event EventHandler<EnlistmentEventArgs> VerifyLocation;
        public event EventHandler<EnlistmentEventArgs> VerifyDebugLocation;
        public event EventHandler<EnlistmentEventArgs> BrowseDebugLocation;

        public string Location
        {
            get { return _location; }
            set 
            { 
                string origLocation = _location;
                _location = value;

                if(VerifyLocation != null)
                {
                    EnlistmentEventArgs e = new EnlistmentEventArgs(this);

                    VerifyLocation(this, e);

                    if(e.Cancel)
                        _location = origLocation;
                }
            }
        }

        public string LocalLocation
        {
            get { return _localLocation; }
            set { _localLocation = value; }
        }

        public string DebugLocation
        {
            get { return _debugLocation; }
            set
            {
                string origLocation = _debugLocation;
                _debugLocation = value;

                if (VerifyDebugLocation != null)
                {
                    EnlistmentEventArgs e = new EnlistmentEventArgs(this);

                    VerifyDebugLocation(this, e);

                    if (e.Cancel)
                        _location = origLocation;
                }
            }
        }

        public string LocalDebugLocation
        {
            get { return _localDebugLocation; }
            set { _localDebugLocation = value; }
        }

        public bool CanBrowseForLocation
        {
            get { return BrowseLocation != null; }
        }

        public bool CanBrowseForDebugLocation
        {
            get { return BrowseDebugLocation != null; }
        }

        bool _allowEditLocation;
        public bool AllowEditLocation
        {
            get { return _allowEditLocation; }
            set { _allowEditLocation = value; }
        }

        bool _allowEditDebugLocation;
        public bool AllowEditDebugLocation
        {
            get { return _allowEditDebugLocation; }
            set { _allowEditDebugLocation = value; }
        }

        public void InvokeBrowseLocation()
        {
            EnlistmentEventArgs e = new EnlistmentEventArgs(this);

            if (CanBrowseForLocation)
                BrowseLocation(this, e);
        }

        public void InvokeBrowseDebugLocation()
        {
            EnlistmentEventArgs e = new EnlistmentEventArgs(this);

            if (CanBrowseForDebugLocation)
                BrowseDebugLocation(this, e);
        }
    }
}
