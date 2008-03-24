using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Ankh.UI.Presenters;
using Rhino.Mocks;
using Ankh.UI;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks.Interfaces;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.Tests.Presenters
{
	[TestFixture]
	public class SvnLogPresenterTest
	{
		class FakedEnumerable<T> : IEnumerable<T>
		{
			public delegate T Create();
			Create creator;
			int count;
			public FakedEnumerable(int count, Create creator)
			{
				this.count = count;
				this.creator = creator;
			}
			#region IEnumerable<T> Members

			public IEnumerator<T> GetEnumerator()
			{
				for (int i = 0; i < count; i++)
					yield return creator();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion
		}
		[Test]
		public void PresenterTest()
		{
			MockRepository mocks = new MockRepository();
			mocks.Record();

			ISvnLogView logView = mocks.DynamicMock<ISvnLogView>();
			Expect.Call(logView.StrictNodeHistory).Return(true).Repeat.AtLeastOnce();
			Expect.Call(logView.IncludeMergedRevisions).Return(true).Repeat.AtLeastOnce();
			
			logView.StrictNodeHistoryChanged += null;
			IEventRaiser strictHistoryRaiser = LastCall.IgnoreArguments().GetEventRaiser();

			logView.IncludeMergedRevisionsChanged += null;
			IEventRaiser includeMergedRaiser = LastCall.IgnoreArguments().GetEventRaiser();

			logView.ScrollPositionChanged += null;
			IEventRaiser itemsRequestedRaiser = LastCall.IgnoreArguments().GetEventRaiser();

			ISvnLogService logService = mocks.DynamicMock<ISvnLogService>();
			logService.IncludeMergedRevisions = false;
			LastCall.IgnoreArguments().PropertyBehavior();

			logService.StrictNodeHistory = false;
			LastCall.IgnoreArguments().PropertyBehavior();

			logService.LogItemReceived += null;		
			IEventRaiser itemReceivedRaiser = LastCall.IgnoreArguments().GetEventRaiser();

			IEnumerable<SvnLogEventArgs> emptyEnum = new FakedEnumerable<SvnLogEventArgs>(0, delegate
				{
					return null;
				});
			Expect.Call(logService.RetrieveAndFlushLogItems()).Return(emptyEnum).Repeat.AtLeastOnce();
			

			mocks.ReplayAll();

			SvnLogPresenter pres = new SvnLogPresenter(logView, logService);

			includeMergedRaiser.Raise(this, EventArgs.Empty);
			Assert.That(logService.IncludeMergedRevisions, Is.True);

			strictHistoryRaiser.Raise(this, EventArgs.Empty);
			Assert.That(logService.StrictNodeHistory, Is.True);

			itemsRequestedRaiser.Raise(this, new ScrollEventArgs(ScrollEventType.SmallIncrement, 0, 5, ScrollOrientation.VerticalScroll));

			itemReceivedRaiser.Raise(this, EventArgs.Empty);

			mocks.VerifyAll();
		}

		[Test, Ignore]
		public void TestStart()
		{
			MockRepository mocks = new MockRepository();
			
			ISvnLogView logView = mocks.DynamicMock<ISvnLogView>();
			ISvnLogService logService = mocks.DynamicMock<ISvnLogService>();

			logView.ScrollPositionChanged += null;
			IEventRaiser itemsRequestedRaiser = LastCall.IgnoreArguments().GetEventRaiser();

			logService.Completed += null;
			IEventRaiser completedRaiser = LastCall.IgnoreArguments().GetEventRaiser();

			logService.Started += null;
			IEventRaiser startedRaiser = LastCall.IgnoreArguments().GetEventRaiser();

			logService.Start();
			LastCall.Repeat.Once();
			
			mocks.ReplayAll();

			SvnLogPresenter pres = new SvnLogPresenter(logView, logService);
			itemsRequestedRaiser.Raise(this, EventArgs.Empty);

			mocks.VerifyAll();
			mocks.BackToRecordAll();

			logService.Start();
			LastCall.Repeat.Never();

			mocks.ReplayAll();

			startedRaiser.Raise(this, EventArgs.Empty);
			itemsRequestedRaiser.Raise(this, EventArgs.Empty); 

			mocks.VerifyAll();
			mocks.BackToRecordAll();

			logService.Start();
			LastCall.Repeat.Once();

			mocks.ReplayAll();

			completedRaiser.Raise(this, EventArgs.Empty);
			itemsRequestedRaiser.Raise(this, EventArgs.Empty); 
			mocks.VerifyAll();

		}

	}
}
