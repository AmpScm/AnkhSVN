using System;
using System.Threading;
using System.Windows.Forms;
using Ankh;
using Ankh.UI;
using Ankh.VS.Dialogs;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class VSCommandRoutingMessageTests
    {
        [TestCase(0x0113)] // WM_TIMER: pointer-sized timer id
        [TestCase(0x0020)] // WM_SETCURSOR: window handle
        [TestCase(0x000F)] // WM_PAINT
        public void NonKeyboardMessagesAcceptPointerSizedWParam(int messageId)
        {
            using (var context = new AnkhServiceContainer())
            using (var form = new VSContainerForm())
            using (var routing = new VSCommandRouting(context, form))
            {
                var message = Message.Create(IntPtr.Zero, messageId,
                    new IntPtr(IntPtr.Size == 8 ? 0x123456789L : 123), IntPtr.Zero);
                Assert.That(routing.PreFilterMessage(ref message), Is.False);
            }
        }
    }
}
