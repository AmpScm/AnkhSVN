using System;
using System.Collections.Generic;
using System.Text;
using Ankh;
using Ankh.VS;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell;
using Ankh.Commands;
using Ankh.UI;

namespace Ankh.Services
{
	[GlobalService(typeof(IAnkhMigrationService))]
	class MigrationService : AnkhService, IAnkhMigrationService
	{
		public MigrationService(IAnkhServiceProvider context)
			: base(context)
		{
		}

		const string MigrateId = "MigrateId";
		public void MaybeMigrate()
		{
			IAnkhPackage pkg = GetService<IAnkhPackage>();
			IAnkhCommandService cs = GetService<IAnkhCommandService>();

			if (pkg == null || cs == null)
				return;

			using (RegistryKey rkRoot = pkg.UserRegistryRoot)
			using (RegistryKey ankhMigration = rkRoot.CreateSubKey("AnkhSVN-Trigger"))
			{
				int migrateFrom = 0;
				object value = ankhMigration.GetValue(MigrateId, migrateFrom);

				if (value is int)
					migrateFrom = (int)value;
				else
					ankhMigration.DeleteValue(MigrateId, false);

				if (migrateFrom < 0)
					migrateFrom = 0;

				if (migrateFrom >= AnkhId.MigrateVersion)
					return; // Nothing to do

				try
				{
					if (cs.DirectlyExecCommand(AnkhCommand.MigrateSettings).Success)
					{
						ankhMigration.SetValue(MigrateId, AnkhId.MigrateVersion);
					}
				}
				catch
				{ /* NOP: Don't fail here... ever! */}
			}
		}
	}
}
