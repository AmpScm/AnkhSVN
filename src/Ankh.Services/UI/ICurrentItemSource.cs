using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.Collections.ObjectModel;

namespace Ankh.UI
{
    /// <summary>
    /// To be implemented by the provider of focus/selection
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface ICurrentItemSource<TItem>
    {
        /// <summary>
        /// Occurs when the selection has been changed
        /// </summary>
        event SelectionChangedEventHandler<TItem> SelectionChanged;

        /// <summary>
        /// Occurs when the focus has been changed
        /// </summary>
        event FocusChangedEventHandler<TItem> FocusChanged;

        /// <summary>
        /// Gets the focused item
        /// </summary>
        TItem FocusedItem { get; }

        /// <summary>
        /// Gets the selected items
        /// </summary>
        IList<TItem> SelectedItems { get; }
    }

    /// <summary>
    /// To be implemented by the consumer of focus/selection
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface ICurrentItemDestination<TItem>
    {
        /// <summary>
        /// Gets or sets the <see cref="ICurrentItemSource"/>
        /// </summary>
        ICurrentItemSource<TItem> ItemSource { get;set;}
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SelectionChangedEventHandler<TItem>(object sender, IList<TItem> e);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FocusChangedEventHandler<TItem>(object sender, TItem e);
}
