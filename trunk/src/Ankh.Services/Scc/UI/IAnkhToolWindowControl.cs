using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc.UI
{
    public interface IAnkhToolWindowControl 
    {
        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <value>The context.</value>
        IAnkhServiceProvider Context { set; }

        /// <summary>
        /// Called when the frame is created
        /// </summary>
        /// <param name="e"></param>
        void OnFrameCreated(EventArgs e);

        /// <summary>
        /// Called when the frame is closed
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnFrameClose(EventArgs e);

        /// <summary>
        /// Called when the dockstate is changing
        /// </summary>
        /// <param name="e"></param>
        void OnFrameDockableChanged(FrameEventArgs e);
        /// <summary>
        /// Called when the frame is moved
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Scc.UI.FrameEventArgs"/> instance containing the event data.</param>
        void OnFrameMove(FrameEventArgs e);

        /// <summary>
        /// Called when the frame is shown or hidden
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Scc.UI.FrameEventArgs"/> instance containing the event data.</param>
        void OnFrameShow(FrameEventArgs e);

        /// <summary>
        /// Called when the frame size has changed
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Scc.UI.FrameEventArgs"/> instance containing the event data.</param>
        void OnFrameSize(FrameEventArgs e);
    }

    /// <summary>
    /// 
    /// </summary>
    public class FrameEventArgs : EventArgs
    {
        readonly Rectangle _location;
        readonly bool _docked;
        readonly __FRAMESHOW _show;

        /// <summary>
        /// Initializes a new instance of the <see cref="DockableEventArgs"/> class.
        /// </summary>
        /// <param name="docked">if set to <c>true</c> [docked].</param>
        /// <param name="location">The location.</param>
        [CLSCompliant(false)]
        public FrameEventArgs(bool docked, Rectangle location, __FRAMESHOW show)
        {
            _location = location;
            _docked = docked;
            _show = show;
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        /// <remarks>Set on the OnFrameMove and OnFrameDockableChanged</remarks>
        public Rectangle Location
        {
            get { return _location; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DockableEventArgs"/> is docked.
        /// </summary>
        /// <value><c>true</c> if docked; otherwise, <c>false</c>.</value>
        /// <remarks>Set on OnFrameDockableChanged event</remarks>
        public bool Docked
        {
            get { return _docked; }
        }

        /// <summary>
        /// Gets the show.
        /// </summary>
        /// <value>The show.</value>
        /// <remarks>Set on OnFrameShow</remarks>
        [CLSCompliant(false)]
        public __FRAMESHOW Show
        {
            get { return _show; }
        }
    }
}
