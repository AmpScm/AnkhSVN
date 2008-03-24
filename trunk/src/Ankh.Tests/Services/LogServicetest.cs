using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Ankh.UI;
using Ankh.UI.Services;
using SharpSvn;
using System.Threading;
using Rhino.Mocks;
using System.ComponentModel;
using System.Windows.Forms;
using NUnit.Framework.SyntaxHelpers;

namespace Ankh.Tests.Services
{
	[TestFixture]
	public class LogServicetest
	{
		[Test]
		public void TestSimpleLog()
		{
			TestLog();

			new System.Windows.Forms.Button().CreateControl();
			TestLog(); // Test again with WindowsSynchronizationContext
		}
		void TestLog()
		{
			bool started = false;
			bool completed = false;
			int counter = 0;

            AnkhServiceContainer container = new AnkhServiceContainer();
            container.AddService(typeof(ISvnClientPool), new AnkhSvnClientPool(container));

            // Todo: Provide an ServiceContainer
			ISvnLogService logSvc = new SvnLogService(null);//syncContext);
			logSvc.RemoteTarget = new Uri("http://ankhsvn.open.collab.net/svn/ankhsvn/");
			logSvc.RequiredItemCount = 10;
			logSvc.LogItemReceived += delegate(object sender, EventArgs e)
			{
				counter++;
			};
			logSvc.Started += delegate { started = true; };
			logSvc.Completed += delegate
			{
				completed = true;
			};
			logSvc.Start();

			Assert.IsTrue(started);
			Assert.IsFalse(completed);

			while (!completed)
			{
				Thread.Sleep(10);
				Application.DoEvents();
			}
			Assert.IsTrue(counter == 10);

			counter = 0;
			foreach (object o in logSvc.RetrieveAndFlushLogItems())
				counter++;
			Assert.IsTrue(counter == 10);

			counter = 0;
			foreach (object o in logSvc.RetrieveAndFlushLogItems())
				counter++;

			Assert.IsTrue(counter == 0);
		}

		[Test]
		public void TestCancel()
		{
			bool completed = false;
			int counter = 0;
			ISvnLogService logSvc = new SvnLogService(null);
			logSvc.RemoteTarget = new Uri("http://ankhsvn.open.collab.net/svn/ankhsvn/");
			logSvc.RequiredItemCount = 10;
			logSvc.LogItemReceived += delegate(object sender, EventArgs e)
			{
				logSvc.Cancel();
				counter++;
			};
			logSvc.Completed += delegate
			{
				completed = true;
			};
			logSvc.Start();

			while (!completed)
			{
				Thread.Sleep(10);
				Application.DoEvents();
			}

			Assert.That(counter, Is.LessThan(10));
			Assert.That(completed);
		}
	}
}
