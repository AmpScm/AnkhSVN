using System;
using EnvDTE;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Event sink for command events
    /// </summary>
    public class CommandsEventSink : EventSink
    {
        public CommandsEventSink( IContext context ) : base( context )
        {
            
            this.RegisterCommandEvents( );
        }

        public override void Unhook()
        {
            _DTE dte = this.Context.DTE;

            if ( this.projectShowAll != null )
            {
                this.projectShowAll.AfterExecute -= new _dispCommandEvents_AfterExecuteEventHandler(
                    this.AfterProjectShowAll );
            }
        }

        private void RegisterCommandEvents( )
        {
            _DTE dte = this.Context.DTE;

            // Project.ShowAll
            try
            {
                this.projectShowAll = dte.Events.get_CommandEvents( PROJECTSHOWALLGUID, 600 );
                this.projectShowAll.AfterExecute += new _dispCommandEvents_AfterExecuteEventHandler(
                    this.AfterProjectShowAll );
            }
            catch( NullReferenceException )
            {
                // happens if Visual Assist X is installed
                this.Context.OutputPane.WriteLine( "Unable to attach to the Project.ShowAll event." + 
                    Environment.NewLine + "Possible conflict with other installed plugin." );
            }
            
        }


        private void AfterProjectShowAll( string guid,
            int id,
            object customIn,
            object customOut )
        {
            try
            {
                // if the "Show all" button is pressed, we need to refresh the whole project
                foreach( Project proj in (Array)this.Context.DTE.ActiveSolutionProjects )
                    this.Context.SolutionExplorer.Refresh( proj );
            }
            catch( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }
        }



        private CommandEvents projectShowAll;
        private const string PROJECTSHOWALLGUID = "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}";

    }
}
