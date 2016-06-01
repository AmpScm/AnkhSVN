using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Ankh.Scc
{
    public delegate void SccAction();
    public delegate void SccAction<T>(T arg);

    public abstract partial class SccProviderThunk : IDisposable, INotifyPropertyChanged, IServiceProvider
    {
        protected SccProviderThunk()
        {

        }

        protected virtual void OnInitialize()
        {

        }

        protected virtual void OnPreInitialize()
        {

        }

        protected virtual void Dispose(bool disposing)
        {

        }


        protected virtual void OnPublishWorkflow()
        {

        }

        protected abstract void OnBranchUIClicked(Point clickedElement);
        protected abstract void OnPendingChangesClicked(Point clickedElement);
        protected abstract void OnRepositoryUIClicked(Point clickedElement);

        protected abstract void OnUnpublishedCommitsUIClickedAsync(Point wr);

        partial void CreateDummyTask(ref object task);

        protected virtual object RunTaskOnMainThread(SccAction action)
        {
            object task = null;

            action();
            CreateDummyTask(ref task);

            return task;
        }

        public virtual string BranchName
        {
            get { return "^/trunk"; }
        }

        PropertyChangedEventHandler _propertyChangedHandler;
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                _propertyChangedHandler += value;
            }

            remove
            {
                _propertyChangedHandler -= value;
            }
        }

        public event EventHandler AdvertisePublish;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (_propertyChangedHandler != null)
                _propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return GetService(serviceType);
        }

        protected abstract object GetService(Type serviceType);

        #endregion
    }
}
