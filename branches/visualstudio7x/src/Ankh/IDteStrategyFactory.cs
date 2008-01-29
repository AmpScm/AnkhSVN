using System;

using System.Text;

namespace Ankh
{
    /// <summary>
    /// A factory for creating DTE-specific strategy objects.
    /// </summary>
    public interface IDteStrategyFactory
    {
        IToolWindowStrategy GetToolWindowStrategy();
    }
}
