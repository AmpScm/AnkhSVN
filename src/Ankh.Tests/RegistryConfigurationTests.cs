using System;
using Ankh.Configuration;
using Microsoft.Win32;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class RegistryConfigurationTests
    {
        [Test]
        public void RegistryUtilsParsesNumericValuesAndRejectsInvalidStrings()
        {
            string subKey = @"Software\AnkhSVN\Tests\RegistryUtils-" + Guid.NewGuid().ToString("N");

            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(subKey))
                {
                    key.SetValue("dword", 7, RegistryValueKind.DWord);
                    key.SetValue("numericString", "42", RegistryValueKind.String);
                    key.SetValue("invalidString", "not-a-number", RegistryValueKind.String);

                    int dword;
                    int numericString;
                    int invalidString;

                    bool dwordOk = RegistryUtils.TryGetIntValue(key, "dword", out dword);
                    bool numericStringOk = RegistryUtils.TryGetIntValue(key, "numericString", out numericString);
                    bool invalidStringOk = RegistryUtils.TryGetIntValue(key, "invalidString", out invalidString);

                    Assert.Multiple(() =>
                    {
                        Assert.That(dwordOk, Is.True);
                        Assert.That(dword, Is.EqualTo(7));

                        Assert.That(numericStringOk, Is.True);
                        Assert.That(numericString, Is.EqualTo(42));

                        Assert.That(invalidStringOk, Is.False);
                    });
                }
            }
            finally
            {
                Registry.CurrentUser.DeleteSubKeyTree(subKey, false);
            }
        }
    }
}
