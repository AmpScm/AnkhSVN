using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using Ankh.Commands;

namespace Ankh
{
    public class AnkhRuntime : IAnkhServiceProvider
    {
        readonly ServiceContainer _container;
        readonly CommandMapper _commandMapper;
        readonly AnkhContext _context;

        public AnkhRuntime(IServiceProvider parentProvider)
        {
            if (parentProvider == null)
                throw new ArgumentNullException("parentProvider");

            _container = new ServiceContainer(parentProvider);

            _commandMapper = ((CommandMapper)parentProvider.GetService(typeof(CommandMapper))) ?? new CommandMapper(this);
            _context = ((AnkhContext)parentProvider.GetService(typeof(AnkhContext))) ?? AnkhContext.Create(this);

            if (parentProvider.GetService(typeof(AnkhRuntime)) == null)
                _container.AddService(typeof(AnkhRuntime), this, true);
        }

        #region IAnkhServiceProvider Members

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
