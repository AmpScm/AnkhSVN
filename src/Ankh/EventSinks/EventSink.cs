using System;
using System.Collections;
using EnvDTE;
using Microsoft.Win32;

namespace Ankh.EventSinks
{
	/// <summary>
	/// Base class for event sink classes.
	/// </summary>
	internal abstract class EventSink
	{
		protected EventSink( AnkhContext context )
		{
			this.context = context;
		}

        /// <summary>
        /// Unhooks from the events.
        /// </summary>
        public abstract void Unhook();

        public static IList CreateEventSinks( AnkhContext context)
        {
            // create event sinks only for projects actually loaded
            
            ArrayList foundKinds = new ArrayList();
            ArrayList sinks = new ArrayList();
            
            foreach( Project project in context.DTE.Solution.Projects )
            {
                // only create one set of sinks for each project type
                if ( foundKinds.Contains( project.Kind ) ) 
                    continue;

                ProjectsEventSink projectsEvents = GetProjectsEvents( project.Kind, context );
                if ( projectsEvents != null )
                    sinks.Add( projectsEvents );

                ProjectItemsEventSink projectItemsEvents = 
                    GetProjectItemsEvents( project.Kind, context );
                if ( projectItemsEvents != null )
                    sinks.Add( projectItemsEvents );
            }

            sinks.Add( new DocumentEventsSink( context ) );

            return sinks;
        }


        protected AnkhContext Context
        {
            get{ return this.context; }
        }  
      
        /// <summary>
        /// Retrieves a VSProjectsEventSink associated with the project 
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static ProjectsEventSink GetProjectsEvents( string kind, AnkhContext context )
        {
            string objectName = GetName( kind, "ProjectsEvents" );    
            if ( objectName != null )
                return new ProjectsEventSink( (ProjectsEvents)
                    context.DTE.Events.GetObject( objectName ),
                    context );
            else
                return null;
        }

        /// <summary>
        /// Retrieves a VSProjectItemsEventSink associated with the project 
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static ProjectItemsEventSink GetProjectItemsEvents( string kind, AnkhContext context )
        {
            string objectName = GetName( kind, "ProjectItemsEvents" );    
            if ( objectName != null )
                return new ProjectItemsEventSink( (ProjectItemsEvents)
                    context.DTE.Events.GetObject( objectName ),
                    context );
            else
                return null;
        }

        /// <summary>
        /// Finds a registry value with a name containing the substring.
        /// </summary>
        /// <param name="substring">The substring to search for</param>
        /// <returns></returns>
        private static string GetName( string kind, string substring )
        {
            string packageGuid = GetPackageGuid( kind );
            string keyName = PACKAGEPATH + packageGuid + "\\AutomationEvents";
            RegistryKey key = Registry.LocalMachine.OpenSubKey( keyName );
            foreach( string name in key.GetValueNames() )
            {
                if ( name.IndexOf( substring ) >= 0 )
                    return name;
            }

            return null;
        }

        /// <summary>
        /// Retrieve the package guid given a project guid
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        private static string GetPackageGuid( string kind )
        {
            string path = PROJECTPATH + kind;
            RegistryKey key = Registry.LocalMachine.OpenSubKey( path );
            return key.GetValue( "Package" ).ToString();
        }
        

        private AnkhContext context;
        private const string PROJECTPATH = @"SOFTWARE\Microsoft\VisualStudio\7.0\Projects\";
        private const string PACKAGEPATH = 
            @"SOFTWARE\Microsoft\VisualStudio\7.0\Packages\";

	}
}
