using System;

using System.Text;

namespace Ankh
{
    class DteStrategyFactory 
    {
        /// <summary>
        /// Creates the DTE strategy factory.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static IDteStrategyFactory CreateDteStrategyFactory( IContext context )
        {
            if ( context.DTE.Version[ 0 ] == '7' )
            {
                return new Dte7StrategyFactory( context );
            }
            else
            {
                return new Dte8StrategyFactory( context );
            }
        }
        private class Dte7StrategyFactory : IDteStrategyFactory
        {
            public Dte7StrategyFactory( IContext context )
            {
                this.strategy = new AnkhUserControlHostToolWindowStrategy( context );
            }
            #region IDteStrategyFactory Members

            public IToolWindowStrategy GetToolWindowStrategy()
            {
                return this.strategy;
            }

            #endregion

            private AnkhUserControlHostToolWindowStrategy strategy; 
        }

        private class Dte8StrategyFactory : IDteStrategyFactory
        {
            public Dte8StrategyFactory( IContext context )
            {
                this.strategy = new CreateToolWindow2ToolWindowStrategy( context );
            }
            #region IDteStrategyFactory Members

            public IToolWindowStrategy GetToolWindowStrategy()
            {
                return this.strategy;
            }

            #endregion

            private CreateToolWindow2ToolWindowStrategy strategy; 
        }
}
}
