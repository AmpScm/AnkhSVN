Xtreme Simplicity VS.NET toolwindow shim.
-----------------------------------------------------

To run this sample add-in:

1. Compile the solution.
2. Run the "ReCreateCommands.reg" file to register the addin.
3. Run the ToolWindowTest project. You might need to change the debug .exe path to point to your devenv.exe.

Release Notes:
------------------

To use this shim in your own addin, there are several points you need to be aware of:

1. We've changed the api - you should take a close look at the CreateToolWindow method in the 
	Connect.cs file to see how to call it.

2. There is some code that needs to be added to your user control to fully support tabbing 
	correctly. The code is included in each of our sample conrols (inheritance being problematic 
	for design-time controls)... Here it is for reference:

		protected override void WndProc(ref System.Windows.Forms.Message m) 
		{
			const int WM_SETFOCUS = 7;
			if (m.Msg == WM_SETFOCUS)
			{
				if(ActiveControl == null)
					SelectNextControl(this, true, true, true, false);
				else
					ActiveControl.Focus();
				return;
			}
			base.WndProc(ref m);
		}

		protected override bool ProcessTabKey(bool forward)
		{
			return SelectNextControl(ActiveControl, forward, true, true, true);
		}

		Without this code you will notice two problems - firstly, when the toolwindow is activated it 
		won't set the focus to the active control. Secondly, tabbing will work, but won't wrap.
		
3. There are still known issues with the control - we've developed it this far and no further, as it suits our 
	needs (and probably many of yours). The known limitations are:
	
	a) Using controls with accept tabs turned on won't work.
	b) Hosting a browser window (IE) doesn't work fully - there may be similar issues with other COM controls.
	Having said that, we use a componentone grid, as well as listview and treeview controls with no problems.
	
Final note: 
	This software is released as a service to the add-in developing community to fill the void left by the 
	withdrawal of Microsoft's official (but non-working) shim. There is *no* warranty of any kind.
	
Thanks - if anyone has any questions (we don't "support" the code, but will happily answer any sensible 
questions, feel free to email us at shim@xtreme-simplicity.net. If anyone finds and/or fixes any bugs, please 
let us know and we'll consider integrating and re-releasing it.

-----------------------------------------------
Jeremy Kothe, CTO Xtreme Simplicity