using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Migrate
{
    public static class Cleanup
    {
        [CLSCompliant(false)]
        public static void RemoveOldUI(IServiceProvider sp, bool fullSearch)
        {
            _DTE dte = (_DTE)sp.GetService(typeof(SDTE));


			foreach (Command c in dte.Commands)
			{

                try
                {
                    if (c.Guid == "{1E58696E-C90F-11D2-AAB2-00C04F688DDE}" && c.Name.StartsWith("Ankh"))
                    {

                        c.Delete();
                    }
                }
                catch
                {
                    // For some weird reason c.Delete() can throw:
                    //   System.ArgumentException: 
                    //   Value does not fall within the expected range. 
                }
			}

			foreach (CommandBar bar in ((CommandBars)dte.CommandBars))
			{
                string name = null;
                try
                {
                    name = bar.Name;
                }
                catch
                {
                    // Swallow
                }
				if (name == "Context Menus")
				{
					foreach (CommandBarControl control in bar.Controls)
					{
						if (control.Type == MsoControlType.msoControlPopup)
						{
                            string caption = null;
                            try
                            {
                                caption = control.Caption;
                            }
                            catch
                            {
                                // Swallow
                            }

                            CommandBarPopup popup = control as CommandBarPopup;
							if (caption == "Project and Solution Context Menus" || caption == "Other Context Menus")
								RecurseCommands(popup.Controls);
							else if (fullSearch)
								RecurseCommands(popup.Controls);
						}
					}
				}
				else if (name == "ReposExplorer")
					bar.Delete();
				else if (name == "WorkingCopyExplorer")
					bar.Delete();
				else if (name == "MenuBar")
					RecurseCommands(bar.Controls);
				else if (fullSearch)
					RecurseCommands(bar.Controls);
			}
        }

		static void RecurseCommands(CommandBarControls controls)
		{
			foreach (CommandBarControl control in controls)
			{
                try
                {
                    if (control.accChildCount > 0 && control.Type == MsoControlType.msoControlPopup)
                    {
                        CommandBarPopup popup = control as CommandBarPopup;
                        if (popup != null)
                            RecurseCommands(popup.Controls);
                    }
                }
                catch { }
                
                
                string caption = null;
                try
                {
                    caption = control.Caption;
                }
                catch
                {
                    // Swallow
                }

                // Ankh and AnkhSVN (without accelerators) have been installed by all versions prior to 0.6.0 snapshot 33
                // Later versions didn't replace this with an item with accelerator, so we still have to check for them when migrating
				if (caption == "An&kh" || caption == "Ankh" || caption == "An&khSVN" || caption == "AnkhSVN" || caption == "WorkingCopyExplorer" || caption == "ReposExplorer")
				{
					control.Delete(false);
					//Debug.WriteLine(GetPath(control));
				}
			}
		}

		/*static string GetPath(object control)
		{
			CommandBarControl barCtrl = control as CommandBarControl;
			if (barCtrl != null)
				return GetPath(barCtrl.Parent) + "->" + barCtrl.Caption;

			CommandBar bar = control as CommandBar;
			if (bar != null)
				return GetPath(bar.Parent) + "->" + bar.Name;

			return "DTE";
		}*/
    }
}
