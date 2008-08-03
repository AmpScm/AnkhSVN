using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ErrorReport.GUI
{
    class MenuManager
    {
        public MenuManager()
        {
            this.contextMenus = new Dictionary<string, SubMenuItem>();
        }

        public void AddCommand( ICommand command, string pathString )
        {
            string[] path = pathString.Split( '.' );
            if ( path.Length == 0 )
            {
                throw new ArgumentException( "Invalid path string", "pathString" );
            }
            string firstName = path[ 0 ];
            string[] subPath = StripPathComponent( path );
            if ( firstName.Equals( MainMenuName, StringComparison.InvariantCultureIgnoreCase ) )
            {
                this.mainMenu.AddSubItem( subPath, command );
            }
        }

        public MenuStrip MainMenu
        {
            get 
            {
                return this.mainMenu.Strip;
            }
        }

        

        class MenuItem
        {
            public readonly string Name;
            private Dictionary<string, SubMenuItem> subItems = new Dictionary<string, SubMenuItem>();

            public MenuItem()
            {
                // nothing
            }

            public MenuItem( string name )
            {
                this.Name = name;
            }

            

            public void AddSubItem( string[] path, ICommand command )
            {
                Debug.Assert( path.Length > 0 );

                string firstName = path[0];

                if ( path.Length == 1 )
                {
                    this.subItems[ firstName ] = new SubMenuItem( firstName, command );
                    this.AddActualMenuItem( this.subItems[ firstName ].ToolStripMenuItem );
                }
                else
                {
                    string[] subPath = StripPathComponent( path );

                    SubMenuItem item;
                    if ( !this.subItems.TryGetValue( firstName, out item ) )
                    {
                        item = new SubMenuItem( firstName );
                        this.subItems[ firstName ] = item;
                        this.AddActualMenuItem(item.ToolStripMenuItem);
                    }
                    item.AddSubItem( subPath, command );
                }
            }

            protected virtual void AddActualMenuItem(ToolStripMenuItem item)
            {
 	            
            }
        }

        class MainMenuItem : MenuItem
        {
            public readonly MenuStrip Strip = new MenuStrip();

            public MainMenuItem()
            {

            }

            protected override void AddActualMenuItem( ToolStripMenuItem item )
            {
                this.Strip.Items.Add( item );
            }
        }

        class SubMenuItem : MenuItem
        {
            public ToolStripMenuItem ToolStripMenuItem; 

            public SubMenuItem( string name ) : base(name)
            {
                this.ToolStripMenuItem = new ToolStripMenuItem( this.Name );
            }

            public SubMenuItem( string name, ICommand command ) : this(name)
            {
                command.EnabledChanged += delegate
                {
                    this.ToolStripMenuItem.Enabled = command.Enabled;
                };
                this.ToolStripMenuItem.Enabled = command.Enabled;

                this.ToolStripMenuItem.Click += delegate
                {
                    if ( command.Enabled )
                    {
                        command.Execute();
                    }
                };
            }

            protected override void AddActualMenuItem( ToolStripMenuItem item )
            {
                ToolStripMenuItem.DropDownItems.Add( item );
            }
        }

        private static string[] StripPathComponent( string[] path )
        {
            string[] subPath = new string[ path.Length - 1 ];
            Array.Copy( path, 1, subPath, 0, subPath.Length );
            return subPath;
        }

        private Dictionary<string, SubMenuItem> contextMenus;
        private MainMenuItem mainMenu = new MainMenuItem();
        private const string MainMenuName = "MainMenu";
    }
}
