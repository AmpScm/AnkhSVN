using System;
using System.Collections;

using System.Text;
using SharpSvn;
using Ankh.Ids;
using System.Collections.Generic;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to lock the selected item.
    /// </summary>
    [Command(AnkhCommand.Lock)]
    public class LockCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        { 
            e.Enabled = false;
        }

        #region Implementation of ICommand


        public override void OnExecute(CommandEventArgs e)
        {
            // TODO: Use path selector based dialog to select files
            // and lock thise
        }

        #endregion

        
    }
}