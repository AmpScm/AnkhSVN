using System;
using Ankh.ExtensionPoints.RepositoryProvider;

namespace Ankh.Services.RepositoryProvider
{
    /// <summary>
    /// Acts as a proxy to the the actual SCM repository provider.
    /// </summary>
    /// <remarks>
    /// This proxy serves "descriptive" properties w/o initializing the actual provider.
    /// The actual provider package initialization is delayed until a non-descriptive property is needed.
    /// Provider id, name, and SCM type are the descriptive properties.
    /// </remarks>
    class ScmRepositoryProviderProxy : ScmRepositoryProvider
    {
        private ScmRepositoryProvider _delegate;
        private readonly IAnkhServiceProvider _context;
        private readonly string _name;
        private readonly string _delegateId;

        public ScmRepositoryProviderProxy(IAnkhServiceProvider context, 
            string delegateServiceId, 
            string name,
            string scmType)
        {
            _context = context;
            _name = name;
            _delegateId = delegateServiceId;
            ScmType = scmType;
        }

        #region ScmRepositoryProvider members

        public override string Id
        {
            get { return _delegateId; }
        }

        public override string Name
        {
            get { return _name; }
        }

        public override ScmRepositorySelectionControl RepositorySelectionControl
        {
            get
            {
                ScmRepositoryProvider dlg = Delegate;
                if (dlg != null)
                {
                    return dlg.RepositorySelectionControl;
                }
                return null;
            }
        }

        #endregion

        private ScmRepositoryProvider Delegate
        {
            get
            {
                if (_delegate == null && !string.IsNullOrEmpty(_delegateId))
                {
                    _delegate = _context.GetService<IAnkhQueryService>().QueryService<ScmRepositoryProvider>(new Guid(_delegateId));
                }
                return _delegate;
            }
        }
    }
}
