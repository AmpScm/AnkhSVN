using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Ankh.VS.UnitTest
{
    [TestFixture]
    public class ComponentSmokeTests
    {
        private static readonly string[] ProductionAssemblies =
        {
            "Ankh",
            "Ankh.Copilot",
            "Ankh.Diff",
            "Ankh.ExtensionPoints",
            "Ankh.Ids",
            "Ankh.ImageCatalog",
            "Ankh.Package",
            "Ankh.Scc",
            "Ankh.Services",
            "Ankh.UI",
            "Ankh.VS",
            "Ankh.VS.Interop"
        };

        [TestCaseSource(nameof(ProductionAssemblies))]
        public void ProductionAssembly_CanLoadAndEnumerateMetadata(string assemblyName)
        {
            string assemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, assemblyName + ".dll");

            Assert.That(File.Exists(assemblyPath), Is.True,
                assemblyName + " must be present in the test output so the shipped component is actually exercised by CI.");

            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            Assert.That(assembly.GetName().Name, Is.EqualTo(assemblyName));

            try
            {
                // Enumerating the type table is deliberately part of this smoke test. It catches
                // missing runtime dependencies and type-load failures that a successful compile alone
                // does not expose.
                _ = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                string details = string.Join(Environment.NewLine,
                    ex.LoaderExceptions
                        .Where(e => e != null)
                        .Select(e => e.GetType().Name + ": " + e.Message));

                Assert.Fail(assemblyName + " contains types that cannot be loaded:" + Environment.NewLine + details);
            }
        }

        [Test]
        public void ProductionAssemblies_HaveUniqueAssemblyIdentities()
        {
            var identities = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (string assemblyName in ProductionAssemblies)
            {
                string assemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, assemblyName + ".dll");
                Assert.That(File.Exists(assemblyPath), Is.True, assemblyName + " is missing from the test output.");

                AssemblyName identity = AssemblyName.GetAssemblyName(assemblyPath);
                string fullName = identity.FullName;

                Assert.That(identities.ContainsKey(fullName), Is.False,
                    assemblyName + " duplicates the assembly identity of " +
                    (identities.TryGetValue(fullName, out string existing) ? existing : "another component") + ".");

                identities.Add(fullName, assemblyName);
            }
        }
    }
}
