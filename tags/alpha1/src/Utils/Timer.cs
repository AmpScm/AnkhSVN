// $Id$
using System;
using System.Runtime.InteropServices;

namespace Utils
{
	/// <summary>
	/// A high precision timer for performance measurement purposes.
	/// </summary>
	public class Timer
	{
        public Timer()
        {
            this.startTime = 0;
            this.endTime = 0;

            QueryPerformanceFrequency( out this.frequency );
        }
        

        public void Start()
        {
            QueryPerformanceCounter( out this.startTime );
        }

        public void End()
        {
            QueryPerformanceCounter( out this.endTime );
        }

        public double Interval
        {
            get{ return (double)(this.endTime-this.startTime)/(double)frequency; }
        }


        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long startTime;
        private long endTime;
        private long frequency;
	}
}
