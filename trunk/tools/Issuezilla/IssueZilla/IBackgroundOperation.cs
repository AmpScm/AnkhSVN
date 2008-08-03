using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace IssueZilla
{
    public interface IBackgroundOperation
    {
        void Work();
        void WorkCompleted( RunWorkerCompletedEventArgs e );             
    }
}
