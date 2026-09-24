using System;
using Ankh.Configuration;
using Microsoft.Win32;
using Moq;
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
        [Test]
        public void RegistryLifoListKeepsNewestItemsAtAndBeyondCapacity()
        {
            string subKey = @"Software\AnkhSVN\Tests\RegistryLifo-" + Guid.NewGuid().ToString("N");

            try
            {
                var configuration = new Mock<IAnkhConfigurationService>();
                configuration
                    .Setup(x => x.OpenUserInstanceKey(It.IsAny<string>()))
                    .Returns(() => Registry.CurrentUser.CreateSubKey(subKey));

                using (var services = new AnkhServiceContainer())
                {
                    services.AddService(typeof(IAnkhConfigurationService), configuration.Object);

                    var list = new RegistryLifoList(services, "Recent", 3);
                    list.Add("one");
                    list.Add("two");
                    list.Add("three");

                    CollectionAssert.AreEqual(
                        new[] { "three", "two", "one" },
                        list,
                        "Filling the ring to capacity must retain the newest item.");

                    list.Add("four");

                    CollectionAssert.AreEqual(
                        new[] { "four", "three", "two" },
                        list,
                        "Overflow must evict only the oldest item.");
                }
            }
            finally
            {
                Registry.CurrentUser.DeleteSubKeyTree(subKey, false);
            }
        }

    }
}
