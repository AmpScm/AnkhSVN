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
