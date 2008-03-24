using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using AnkhSvn.Ids;

namespace Ankh.Scc
{
    /// <summary>
    /// Handles the finishtasks special command; this command is posted to the back of the command queueue
    /// if the SCC implementation needs to perform some post processing of VSs scc actions
    /// </summary>
    [Command(AnkhCommand.SccFinishTasks)]
    class TaskFinisher : ICommandHandler
    {
        ProjectTracker _tracker;
        AnkhSccProvider _scc;

        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (_tracker == null)
                _tracker = (ProjectTracker)e.Context.GetService<IAnkhProjectDocumentTracker>();
            if(_scc == null)
                _scc = (AnkhSccProvider)e.Context.GetService<IAnkhSccService>();

            if(_tracker != null)
                _tracker.OnSccCleanup(e);

            if (_scc != null)
                _scc.OnSccCleanup(e);            
        }

        #endregion
    }
}
