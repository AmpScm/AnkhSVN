using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Resources;
using System.ComponentModel;

namespace Ankh.DotNet2Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Dictionary<string, Assembly> handled = new Dictionary<string, Assembly>();
            VerifyCompatibility(handled, typeof(Ankh.VSPackage.AnkhSvnPackage).Assembly);
        }

        private static void VerifyCompatibility(Dictionary<string, Assembly> handled, Assembly asm)
        {
            AssemblyName name = new AssemblyName(asm.FullName);
            handled.Add(name.Name, asm);

            Console.WriteLine("Verifying assembly {0}", name.Name);

            foreach(string resourceName in asm.GetManifestResourceNames())
            {
                if (!".resources".Equals(Path.GetExtension(resourceName)))
                    continue;

                Console.WriteLine("  Loading resource {0}", Path.GetFileNameWithoutExtension(resourceName));
                using(Stream s = asm.GetManifestResourceStream(resourceName))
                using (ResourceReader rr = new ResourceReader(s))
                {
                    foreach (object o in rr)
                    {
                        GC.KeepAlive(o);
                    }
                }
            }

            Type[] assemblyTypes = null;

            try
            {
                assemblyTypes = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (Exception ee in e.LoaderExceptions)
                {
                    Console.WriteLine(ee.ToString());
                }
            }

            if (assemblyTypes != null)
            {
                foreach (Type t in assemblyTypes)
                {
                    if (!typeof(IComponent).IsAssignableFrom(t))
                        continue;
                    else if (t.ContainsGenericParameters)
                        continue;
                    else if (null == t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.ExactBinding, null, new Type[0], null))
                        continue;

                    Console.WriteLine("  Instantiating {0}", t.FullName);
                    Activator.CreateInstance(t);
                }
            }

            foreach (AssemblyName an in asm.GetReferencedAssemblies())
            {
                if (!an.Name.StartsWith("Ankh."))
                    continue;
                else if (handled.ContainsKey(an.Name))
                    continue;

                Assembly a = Assembly.Load(an);
                VerifyCompatibility(handled, a);
            }
        }
    }
}
