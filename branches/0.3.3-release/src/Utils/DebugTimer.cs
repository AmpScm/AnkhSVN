using System;
using System.Diagnostics;

namespace Utils
{
    /// <summary>
    /// Used to time and write debug messages for profiling purposes.
    /// </summary>
    public class DebugTimer
    {
        private DebugTimer()
        {
            this.timer = new Timer();
            this.timer.Start();
        }

        public static DebugTimer Start()
        {
            return new DebugTimer();
        }

        public static DebugTimer Start( string message, string cathegory )
        {
            Debug.WriteLine( message, cathegory );
            return new DebugTimer();
        }

        public void End()
        {
            this.timer.End();
            Debug.WriteLine( this.timer.Interval );
        }

        public void End( string message, string cathegory )
        {
            this.timer.End();
            Debug.WriteLine( String.Format("{0}: {1:G3}", message, this.timer.Interval), cathegory );
        }

        private Timer timer;
    }
}
