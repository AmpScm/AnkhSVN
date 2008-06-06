using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    public abstract class AnkhWizard : Wizard
    {
        readonly IAnkhServiceProvider _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhWizard"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected AnkhWizard(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
        }
    }
}
