using System.IO;
using System.Threading;

namespace Utils
{

    public class ProcessReader
    {
        public ProcessReader( StreamReader reader )
        {
            this.reader = reader;
        }
            
        public void Start()
        {
            this.thread = new Thread( new ThreadStart( this.Read ) );
            this.thread.Start();
        }

        public void Wait()
        {
            this.thread.Join();
        }

        public string Output
        {
            get{ return this.output; }
        }

        private void Read()
        {
            this.output = reader.ReadToEnd();
        }

            

        private StreamReader reader;
        private Thread thread;
        private string output;
    }
}