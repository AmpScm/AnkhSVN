// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Drawing;
using System.Threading.Tasks;

using Ankh.Scc;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Scc
{
    [TestFixture]
    public class SccProviderThunkTaskTests
    {
        [Test]
        public void RunTaskOnMainThread_ReturnsCompletedTask()
        {
            var thunk = new TestSccProviderThunk();
            int invocationCount = 0;

            Task task = thunk.RunTaskAsync(
                delegate { invocationCount++; });

            Assert.That(invocationCount, Is.EqualTo(1));
            Assert.That(task, Is.Not.Null);
            Assert.That(task.IsCompleted, Is.True);
            Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }

        [Test]
        public void RunTaskOnMainThread_RejectsNullAction()
        {
            var thunk = new TestSccProviderThunk();

            Assert.ThrowsAsync<ArgumentNullException>(
                async delegate { await thunk.RunTaskAsync(null); });
        }

        private sealed class TestSccProviderThunk : SccProviderThunk
        {
            public Task RunTaskAsync(SccAction action)
            {
                return (Task)RunTaskOnMainThread(action);
            }

            protected override void OnBranchUIClicked(Point clickedElement)
            {
            }

            protected override void OnPendingChangesClicked(Point clickedElement)
            {
            }

            protected override void OnRepositoryUIClicked(Point clickedElement)
            {
            }

            protected override void OnUnpublishedCommitsUIClicked(Point clickedElement)
            {
            }

            protected override object GetService(Type serviceType)
            {
                return null;
            }
        }
    }
}
