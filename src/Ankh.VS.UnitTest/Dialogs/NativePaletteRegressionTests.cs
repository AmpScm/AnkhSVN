using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Ankh.UI.VSSelectionControls;
using Ankh.WpfPackage.Services;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class NativePaletteRegressionTests
    {
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam);

        [Test]
        public void TreePaletteRestoresNativeColorsEvenWhenManagedColorsAreUnchanged()
        {
            using (var tree = new TreeView { BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.WhiteSmoke })
            {
                SendMessage(tree.Handle, 0x1100 + 29, IntPtr.Zero, (IntPtr)ColorTranslator.ToWin32(Color.White));
                SendMessage(tree.Handle, 0x1100 + 30, IntPtr.Zero, IntPtr.Zero);
                ThemingService.RestoreNativeTreeColors(tree);
                Assert.That(SendMessage(tree.Handle, 0x1100 + 31, IntPtr.Zero, IntPtr.Zero).ToInt32(),
                    Is.EqualTo(ColorTranslator.ToWin32(tree.BackColor)));
                Assert.That(SendMessage(tree.Handle, 0x1100 + 32, IntPtr.Zero, IntPtr.Zero).ToInt32(),
                    Is.EqualTo(ColorTranslator.ToWin32(tree.ForeColor)));
            }
        }

        [Test]
        public void ListPaletteRestoresNativeTextAndBackgroundColors()
        {
            using (var list = new ListView { BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.WhiteSmoke })
            {
                SendMessage(list.Handle, 0x1000 + 1, IntPtr.Zero, (IntPtr)ColorTranslator.ToWin32(Color.White));
                SendMessage(list.Handle, 0x1000 + 38, IntPtr.Zero, (IntPtr)ColorTranslator.ToWin32(Color.White));
                SendMessage(list.Handle, 0x1000 + 36, IntPtr.Zero, IntPtr.Zero);
                ThemingService.RestoreNativeListColors(list);
                Assert.That(SendMessage(list.Handle, 0x1000, IntPtr.Zero, IntPtr.Zero).ToInt32(),
                    Is.EqualTo(ColorTranslator.ToWin32(list.BackColor)));
                Assert.That(SendMessage(list.Handle, 0x1000 + 37, IntPtr.Zero, IntPtr.Zero).ToInt32(),
                    Is.EqualTo(ColorTranslator.ToWin32(list.BackColor)));
                Assert.That(SendMessage(list.Handle, 0x1000 + 35, IntPtr.Zero, IntPtr.Zero).ToInt32(),
                    Is.EqualTo(ColorTranslator.ToWin32(list.ForeColor)));
            }
        }

        sealed class RecreatingTree : SmartTreeView
        {
            public void Recreate() { RecreateHandle(); }
        }

        [Test]
        public void RecreatedTreeKeepsItsOwnPaletteInAMixedSurfaceTheme()
        {
            using (var parent = new Panel { BackColor = Color.White, ForeColor = Color.Black })
            using (var tree = new RecreatingTree { BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.WhiteSmoke })
            {
                parent.Controls.Add(tree);
                typeof(SmartTreeView).GetField("_useDarkNativeTheme", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(tree, true);
                var handle = tree.Handle;
                tree.Recreate();
                Assert.That(tree.BackColor, Is.EqualTo(Color.FromArgb(30, 30, 30)));
                Assert.That(tree.ForeColor, Is.EqualTo(Color.WhiteSmoke));
            }
        }
    }
}
