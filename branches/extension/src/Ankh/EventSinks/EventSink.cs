// $Id$
using System;
using System.Collections;
using EnvDTE;
using Microsoft.Win32;
using Microsoft;
using Interop.esproj;
using System.Threading;
//using Microsoft.VisualStudio.VCProjectEngine;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Base class for event sink classes.
    /// </summary>
    public abstract class EventSink
    {
        protected EventSink( IContext context )
        {
            this.context = context;
        }

        /// <summary>
        /// Unhooks from the events.
        /// </summary>
        public abstract void Unhook();

        public static IList CreateEventSinks( IContext context)
        {
            // create event sinks only for projects actually loaded
            
            ArrayList foundKinds = new ArrayList();
            ArrayList sinks = new ArrayList();

            Projects projects = context.DTE.Solution.Projects;
            
            for( int i = 1; i <= projects.Count; i++ )
            {
                try
                {
                    Project project = projects.Item(i);
                    // only create one set of sinks for each project type
                    if ( foundKinds.Contains( project.Kind ) ) 
                        continue;
                    foundKinds.Add( project.Kind );

                    // VC++ projects are a special case
                    if ( project.Kind == VCPROJECTGUID )
                    {
                        object events = 
                            context.DTE.Events.GetObject( VCPROJECT );
                        sinks.Add( new VCProjectEventSink( events, context ) );

                        continue;
                    }
                    
                    // all other projects should follow the normal model
                    ProjectsEventSink projectsEvents = GetProjectsEvents( project.Kind, context );
                    if ( projectsEvents != null )
                        sinks.Add( projectsEvents );

                    ProjectItemsEventSink projectItemsEvents = 
                        GetProjectItemsEvents( project.Kind, context );
                    if ( projectItemsEvents != null )
                        sinks.Add( projectItemsEvents );
                }
                catch( Exception ex )
                {
                    context.ErrorHandler.Handle( ex );
                    context.OutputPane.WriteLine( "Error when retrieving events sink for project." + 
                        "Manual refresh of this project may be necessary." );
                }
            }

            sinks.Add( new DocumentEventsSink( context ) );
            sinks.Add( new SolutionEventsSink( context ) );

            sinks.Add( new CommandsEventSink( context ) );

            sinks.Add( new ProjectFilesEventSink( context ) );

            return sinks;
        }


        protected IContext Context
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.context; }
        }  

        /// <summary>
        /// Whether a VC++ project is currently being added. This property is
        /// used by the VCProjectEventSink to keep track of when a VC++ project is being 
        /// added and suppress file added events during that time.
        /// </summary>
        protected static bool AddingProject
        {
            get{ return addingProject; }
            set{ addingProject = value; }
        }        
      
        /// <summary>
        /// Retrieves a VSProjectsEventSink associated with the project 
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static ProjectsEventSink GetProjectsEvents( string kind, IContext context )
        {
            string objectName = GetName( kind, "ProjectsEvents", context.DTE );
            if (objectName == null)
            {
                objectName = GetName(kind, "WebSiteEvents", context.DTE);
            }
            if ( objectName != null )
            {
                object projectsEvents;
                try
                {
                    projectsEvents = context.DTE.Events.GetObject( objectName );
                }
                catch( System.Runtime.InteropServices.COMException )
                {
                    // all eVB projects just throw "Catastrophic failure" when trying 
                    // to retrieve the event objects
                    context.OutputPane.WriteLine( "Unable to retrieve project events object for " + 
                        "project of type {0}", objectName );
                    return null;
                }

                if ( projectsEvents is ProjectsEvents )
                {
                    return new ProjectsEventSink( (ProjectsEvents)
                        projectsEvents,
                        context );
                }
                else if ( projectsEvents is ICSharpEventsRoot )
                {
                    return new ProjectsEventSink( (ProjectsEvents)
                        ((ICSharpEventsRoot)projectsEvents).CSharpProjectsEvents, 
                        context );
                }
                else
                {
                    throw new ApplicationException( String.Format(
                        @"Could not retrieve ProjectsEvents.
kind: {0} 
objectName: {1}
type: {2}
ToString(): {3}
Please report this error.", kind, objectName, projectsEvents.GetType(), 
                        projectsEvents.ToString() ) );
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Retrieves a VSProjectItemsEventSink associated with the project 
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static ProjectItemsEventSink GetProjectItemsEvents( string kind, IContext context )
        {
            string objectName = GetName( kind, "ProjectItemsEvents", context.DTE );
            if (objectName == null)
            {
                objectName = GetName(kind, "WebsiteItemsEvents", context.DTE);
            }
            if ( objectName != null )
            {
                object events;
                try
                {
                    events = context.DTE.Events.GetObject( objectName );
                }
                catch( System.Runtime.InteropServices.COMException )
                {
                    // all eVB projects just throw "Catastrophic failure" when trying 
                    // to retrieve the event objects
                    context.OutputPane.WriteLine( "Unable to retrieve project item events object for " + 
                        "project of type {0}", objectName );
                    return null;
                }

                if ( events is ProjectItemsEvents )
                {
                    return new ProjectItemsEventSink( (ProjectItemsEvents)
                        events,
                        context );
                }
                else if ( events is ICSharpEventsRoot )
                {
                    return new ProjectItemsEventSink( (ProjectItemsEvents)
                        ((ICSharpEventsRoot)events).get_CSharpProjectItemsEvents( null ),
                        context );
                }
                else
                {
                    throw new ApplicationException( String.Format(
                        @"Could not retrieve ProjectItemsEvents.
kind: {0} 
objectName: {1}
type: {2}
ToString(): {3}
Please report this error.", kind, objectName, events.GetType(), 
                        events.ToString() ) );
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Finds a registry value with a name containing the substring.
        /// </summary>
        /// <param name="substring">The substring to search for</param>
        /// <returns></returns>
        private static string GetName( string kind, string substring, _DTE dte )
        {
            string packageGuid = GetPackageGuid( kind, dte );
            if ( packageGuid == null )
                return null;

            string keyName = dte.RegistryRoot + PACKAGEPATH + packageGuid + "\\AutomationEvents";
            RegistryKey key = Registry.LocalMachine.OpenSubKey( keyName );
            if ( key == null )
                return null;
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
        private static string GetPackageGuid( string kind, _DTE dte )
        {
            string path = dte.RegistryRoot + PROJECTPATH + kind;
            RegistryKey key = Registry.LocalMachine.OpenSubKey( path );
            if ( key == null ) 
                return null;
            else
                return key.GetValue( "Package" ).ToString();
        }
        
        protected const int REFRESHDELAY = 200;
        private static bool addingProject = false;
        private IContext context;
        private const string PROJECTPATH = @"\Projects\";
        private const string PACKAGEPATH = 
            @"\Packages\";
        private const string VCPROJECTGUID = 
            @"{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
        private const string VCPROJECT = "VCProjectEngineEventsObject";

    }
}
