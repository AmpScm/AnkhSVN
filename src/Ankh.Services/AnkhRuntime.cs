using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using Ankh.Commands;
using System.Diagnostics;

namespace Ankh
{
    public class AnkhRuntime : IAnkhServiceProvider
    {
        readonly IServiceContainer _container;
        readonly CommandMapper _commandMapper;
        readonly AnkhContext _context;
        bool _ensureServices;

        public AnkhRuntime(IServiceContainer parentContainer)
        {
            if (parentContainer == null)
                throw new ArgumentNullException("parentContainer");

            _container = parentContainer;

            _commandMapper = (CommandMapper)_container.GetService(typeof(CommandMapper));
            if (_commandMapper == null)
                _container.AddService(typeof(CommandMapper), _commandMapper = new CommandMapper(this));

            _context = ((AnkhContext)_container.GetService(typeof(AnkhContext));
            if(_context == null)
                _container.AddService(typeof(AnkhContext), _context = AnkhContext.Create(this));

            InitializeServices();
        }

        public AnkhRuntime(IServiceProvider parentProvider)
        {
            if (parentProvider == null)
                throw new ArgumentNullException("parentProvider");

            _container = new AnkhServiceContainer(parentProvider);

            _commandMapper = (CommandMapper)_container.GetService(typeof(CommandMapper));
            if (_commandMapper == null)
                _container.AddService(typeof(CommandMapper), _commandMapper = new CommandMapper(this));

            _context = ((AnkhContext)_container.GetService(typeof(AnkhContext));
            if(_context == null)
                _container.AddService(typeof(AnkhContext), _context = AnkhContext.Create(this));

            InitializeServices();
        }

        void InitializeServices()
        {
            if (_container.GetService(typeof(AnkhRuntime)) == null)
            {
                _container.AddService(typeof(AnkhRuntime), this, true);

                if (_container.GetService(typeof(IAnkhServiceProvider)) == null)
                    _container.AddService(typeof(IAnkhServiceProvider), this);
            }

#if DEBUG
            PreloadServicesViaEnsure = true;
#endif
        }

        /// <summary>
        /// Gets or sets a value indicating whether all modules must preload their services
        /// </summary>
        /// <value><c>true</c> if all services should preload their required services, otherwise <c>false</c>.</value>
        public bool PreloadServicesViaEnsure
        {
            get { return _ensureServices; }
            set { _ensureServices = value; }
        }
        
        #region IAnkhServiceProvider Members

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        #endregion

        #region IServiceProvider Members

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        [DebuggerStepThrough]
        public object GetService(Type serviceType)
        {
            return _container.GetService(serviceType);
        }

        #endregion

        /// <summary>
        /// Gets the command mapper.
        /// </summary>
        /// <value>The command mapper.</value>
        public CommandMapper CommandMapper
        {
            get { return _commandMapper; }
        }

        /// <summary>
        /// Gets the single context instance
        /// </summary>
        /// <value>The context.</value>
        public AnkhContext Context
        {
            get { return _context; }
        }

        readonly List<Module> _modules = new List<Module>();

        public void AddModule(Module module)
        {
            _modules.Add(module);

            module.OnPreInitialize();
        }

        public void Start()
        {
            foreach (Module module in _modules)
            {
                module.OnInitialize();
            }
        }

        public static AnkhRuntime Get(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            return (AnkhRuntime)serviceProvider.GetService(typeof(AnkhRuntime));
        }
    }
}
