using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Ankh.Diff;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class NumericTextBoxBehaviorTests
    {
        [Test]
        public void DefaultsAndUnsupportedTextBoxFeaturesRemainConstrained()
        {
            using (var box = new NumericTextBox())
            {
                Assert.Multiple(() =>
                {
                    Assert.That(box.Text, Is.EqualTo("0"));
                    Assert.That(box.Value, Is.Zero);
                    Assert.That(box.MinValue, Is.Zero);
                    Assert.That(box.MaxValue, Is.Zero);
                    Assert.That(box.AllowFloat, Is.False);
                    Assert.That(box.AllowNegative, Is.False);
                    Assert.That(box.AllowAllInput, Is.False);
                    Assert.That(box.RoundIntValue, Is.True);
                    Assert.That(box.Multiline, Is.False);
                    Assert.That(box.ScrollBars, Is.EqualTo(ScrollBars.None));
                    Assert.That(box.AcceptsReturn, Is.False);
                    Assert.That(box.AcceptsTab, Is.False);
                    Assert.That(box.Lines, Is.Empty);
                    Assert.That(box.WordWrap, Is.False);
                    Assert.That(box.PasswordChar, Is.EqualTo('\0'));
                });

                Assert.Multiple(() =>
                {
                    Assert.Throws<NotSupportedException>(() => box.Multiline = true);
                    Assert.Throws<NotSupportedException>(() => box.ScrollBars = ScrollBars.Vertical);
                    Assert.Throws<NotSupportedException>(() => box.AcceptsReturn = true);
                    Assert.Throws<NotSupportedException>(() => box.AcceptsTab = true);
                    Assert.Throws<NotSupportedException>(() => box.Lines = new[] { "1" });
                    Assert.Throws<NotSupportedException>(() => box.WordWrap = true);
                    Assert.Throws<NotSupportedException>(() => box.PasswordChar = '*');
                });
            }
        }

        [Test]
        public void ValidateCoversNegativeBoundsFloatRoundingAndTruncation()
        {
            using (var box = new NumericTextBox())
            {
                double value = -4.75;
                Assert.That(box.Validate(ref value), Is.False);
                Assert.That(value, Is.EqualTo(5.0));

                box.AllowNegative = true;
                box.AllowFloat = true;
                value = -4.75;
                Assert.That(box.Validate(ref value), Is.True);
                Assert.That(value, Is.EqualTo(-4.75));

                box.MinValue = -2;
                box.MaxValue = 10;

                value = -9;
                Assert.That(box.Validate(ref value), Is.False);
                Assert.That(value, Is.EqualTo(-2));

                value = 25;
                Assert.That(box.Validate(ref value), Is.False);
                Assert.That(value, Is.EqualTo(10));

                value = 4.5;
                Assert.That(box.Validate(ref value), Is.True);

                box.AllowFloat = false;
                box.RoundIntValue = true;
                value = 4.6;
                Assert.That(box.Validate(ref value), Is.False);
                Assert.That(value, Is.EqualTo(5));

                box.RoundIntValue = false;
                value = 4.9;
                Assert.That(box.Validate(ref value), Is.False);
                Assert.That(value, Is.EqualTo(4));

                box.MinValue = 20;
                box.MaxValue = 10;
                box.AllowFloat = true;
                value = 15;
                Assert.That(box.Validate(ref value), Is.True,
                    "An invalid min/max range intentionally disables bound clamping.");
            }
        }

        [Test]
        public void PropertySettersNormalizeValueAndIntValueHonorsRoundingMode()
        {
            using (var box = new NumericTextBox())
            {
                box.AllowNegative = true;
                box.AllowFloat = true;
                box.MinValue = -10;
                box.MaxValue = 10;
                box.Value = 4.6;

                Assert.That(box.Text, Is.EqualTo(4.6.ToString("R")));

                box.RoundIntValue = true;
                Assert.That(box.IntValue, Is.EqualTo(5));

                box.RoundIntValue = false;
                Assert.That(box.IntValue, Is.EqualTo(4));

                box.Value = 20;
                Assert.That(box.Value, Is.EqualTo(10));

                box.Value = -20;
                Assert.That(box.Value, Is.EqualTo(-10));

                box.AllowNegative = false;
                Assert.That(box.Value, Is.EqualTo(10));

                box.AllowFloat = false;
                box.Value = 2.4;
                Assert.That(box.Value, Is.EqualTo(2));

                // Reassigning the current values exercises the no-op setter paths.
                box.Value = box.Value;
                box.MinValue = box.MinValue;
                box.MaxValue = box.MaxValue;
                box.AllowFloat = box.AllowFloat;
                box.AllowNegative = box.AllowNegative;
            }
        }

        [Test]
        public void ValidateTextAcceptsNumbersAndRestoresLastValueForInvalidText()
        {
            using (var box = new NumericTextBox())
            {
                box.AllowFloat = true;
                box.AllowNegative = true;
                box.Value = 3.25;

                box.Text = "-7.5";
                Assert.That(box.Validate(), Is.True);
                Assert.That(box.Value, Is.EqualTo(-7.5));

                box.Text = "not-a-number";
                Assert.That(box.Validate(), Is.False);
                Assert.That(box.Text, Is.EqualTo((-7.5).ToString("R")));

                box.Text = double.MaxValue.ToString("R", CultureInfo.InvariantCulture) + "0";
                Assert.That(box.Validate(), Is.False);
                Assert.That(box.Value, Is.EqualTo(-7.5));
            }
        }

        [Test]
        public void KeyFilteringCoversDigitsSignsDecimalDuplicatesAndFreeInput()
        {
            using (var box = new NumericTextBox())
            {
                box.Text = "12";

                Assert.That(Press(box, '3').Handled, Is.False);
                Assert.That(Press(box, 'x').Handled, Is.True);
                Assert.That(Press(box, '-').Handled, Is.True);

                box.AllowNegative = true;
                box.SelectionStart = 0;
                Assert.That(Press(box, '-').Handled, Is.False);

                box.Text = "-12";
                box.SelectionStart = 3;
                Assert.That(Press(box, '-').Handled, Is.True);

                box.SelectionStart = 0;
                box.SelectionLength = 1;
                Assert.That(Press(box, '-').Handled, Is.True,
                    "Replacing the existing sign is still treated as duplicate by the historical control.");

                char decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0];
                box.Text = "12";
                box.SelectionStart = box.Text.Length;
                Assert.That(Press(box, decimalSeparator).Handled, Is.True);

                box.AllowFloat = true;
                Assert.That(Press(box, decimalSeparator).Handled, Is.False);

                box.Text = "1" + decimalSeparator + "2";
                box.SelectionStart = box.Text.Length;
                Assert.That(Press(box, decimalSeparator).Handled, Is.True);

                Assert.That(Press(box, '\b').Handled, Is.False);

                box.AllowAllInput = true;
                Assert.That(Press(box, 'x').Handled, Is.False);
                Assert.That(box.CanPaste, Is.True);
            }
        }

        [Test]
        public void ValidatingAndLeaveRespectCancellationAndCausesValidation()
        {
            using (var box = new NumericTextBox())
            {
                box.AllowFloat = true;
                box.Value = 2.5;
                box.Text = "bad";

                var cancelled = new CancelEventArgs(true);
                InvokeProtected(box, "OnValidating", cancelled);
                Assert.That(box.Text, Is.EqualTo("bad"));

                var allowed = new CancelEventArgs(false);
                InvokeProtected(box, "OnValidating", allowed);
                Assert.That(box.Text, Is.EqualTo(2.5.ToString("R")));

                box.CausesValidation = false;
                box.Text = "bad-again";
                InvokeProtected(box, "OnLeave", EventArgs.Empty);
                Assert.That(box.Text, Is.EqualTo(2.5.ToString("R")));

                box.CausesValidation = true;
                box.Text = "left-alone";
                InvokeProtected(box, "OnLeave", EventArgs.Empty);
                Assert.That(box.Text, Is.EqualTo("left-alone"));
            }
        }

        static KeyPressEventArgs Press(NumericTextBox box, char key)
        {
            var args = new KeyPressEventArgs(key);
            InvokeProtected(box, "OnKeyPress", args);
            return args;
        }

        static void InvokeProtected(NumericTextBox box, string methodName, object argument)
        {
            MethodInfo method = typeof(NumericTextBox).GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(box, new[] { argument });
        }
    }
}
