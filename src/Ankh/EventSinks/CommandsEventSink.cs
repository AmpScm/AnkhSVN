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
            if ( this.projectShowAll != null )
            {
                this.projectShowAll.AfterExecute -= new _dispCommandEvents_AfterExecuteEventHandler(
                    this.AfterProjectShowAll );
                this.refresh.AfterExecute -= new _dispCommandEvents_AfterExecuteEventHandler(
                    this.AfterProjectShowAll );
            }
        }

        private void RegisterCommandEvents( )
        {
            _DTE dte = this.Context.DTE;

            // Project.ShowAll
            try
            {
                this.projectShowAll = dte.Events.get_CommandEvents( PROJECTSHOWALLGUID, PROJECTSHOWALLID );
                this.projectShowAll.AfterExecute += new _dispCommandEvents_AfterExecuteEventHandler(
                    this.AfterProjectShowAll );
            }
            catch ( NullReferenceException )
            {
                // happens if Visual Assist X is installed
                this.Context.OutputPane.WriteLine( "Unable to attach to the Project.ShowAll event." +
                    Environment.NewLine + "Possible conflict with other installed plugin." );
            }
            try
            {

                //this.allEvents = (EnvDTE.CommandEvents)dte.Events.get_CommandEvents("{00000000-0000-0000-0000-000000000000}", 0);
                //this.allEvents.AfterExecute += new _dispCommandEvents_AfterExecuteEventHandler( allEvents_AfterExecute );

                this.refresh = dte.Events.get_CommandEvents( REFRESHGUID, REFRESHID );
                this.refresh.AfterExecute += new _dispCommandEvents_AfterExecuteEventHandler(
                    this.AfterProjectShowAll );
            }
            catch ( NullReferenceException )
            {
                // happens if Visual Assist X is installed
                this.Context.OutputPane.WriteLine( "Unable to attach to the Project.ShowAll event." +
                    Environment.NewLine + "Possible conflict with other installed plugin." );

            }
           
            
        }

        //void allEvents_AfterExecute( string Guid, int ID, object CustomIn, object CustomOut )
        //{
        //    string name = "";
        //    try
        //    {
        //        name = this.Context.DTE.Commands.Item( Guid, ID ).Name;
        //    }
        //    catch ( Exception )
        //    {
        //    }
        //    this.Context.OutputPane.WriteLine( "Command {0} invoked - Guid: {1}, ID: {2}", name, Guid, ID );
        //}


        private void AfterProjectShowAll( string guid,
            int id,
            object customIn,
            object customOut )
        {
            try
            {
                // if the "Show all" or "Refresh" buttons are pressed, we need to refresh the whole project
                foreach( Project proj in (Array)this.Context.DTE.ActiveSolutionProjects )
                    this.Context.SolutionExplorer.Refresh( proj );
            }
            catch( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }
        }



        private CommandEvents projectShowAll;
        //private CommandEvents allEvents;
        private CommandEvents refresh;
        private const string PROJECTSHOWALLGUID = "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}";
        private const string REFRESHGUID = "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}";
        private const int REFRESHID = 222;
        private const int PROJECTSHOWALLID = 600;

    }
}
