using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc;
using Ankh.UI;

namespace Ankh
{
    public class AnkhModule : Module
    {
        IContext _context;
        public AnkhModule(AnkhRuntime runtime)
            : base(runtime)
        {
        }

        public override void OnPreInitialize()
        {
            Runtime.CommandMapper.LoadFrom(typeof(AnkhModule).Assembly);

            // Ensure old context behaviour
            _context = GetService<IContext>();
            if (_context == null)
            {
                _context = new OldAnkhContext(GetService<IAnkhPackage>());
                Container.AddService(typeof(IContext), _context, true);
            }

            // TODO: Register services
            if(null == Container.GetService(typeof(IFileStatusCache)))
                Container.AddService(typeof(IFileStatusCache), _context.StatusCache);
            Container.AddService(typeof(IStatusImageMapper), new StatusImages.TempStatusImageMapper());            
        }

        public override void OnInitialize()
        {
            //throw new NotImplementedException();
        }
    }
}
