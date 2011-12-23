using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

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
                    if (t.IsAbstract || !t.IsClass)
                        continue;

                    if (!typeof(IComponent).IsAssignableFrom(t))
                        continue;
                    else if (t.ContainsGenericParameters)
                        continue;
                    else if (null == t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.ExactBinding, null, new Type[0], null))
                        continue;

                    Console.WriteLine("  Instantiating {0}", t.FullName);
                    Activator.CreateInstance(t, true);
                }

                foreach (Type t in assemblyTypes)
                {
                    if (t.IsAbstract || !t.IsClass)
                        continue;

                    ComVisibleAttribute[] cvAttrs = (ComVisibleAttribute[])t.GetCustomAttributes(typeof(ComVisibleAttribute), false);

                    if (cvAttrs == null || cvAttrs.Length == 0 || cvAttrs[0].Value == false)
                        continue;

                    Type ancestor = t;
                    while (ancestor != null && ancestor.BaseType != null && ancestor.BaseType != typeof(Object))
                    {
                        ancestor = ancestor.BaseType;
                    }

                    // The ancestor must either be ComVisible

                    cvAttrs = (ComVisibleAttribute[])ancestor.GetCustomAttributes(typeof(ComVisibleAttribute), false);

                    if (cvAttrs != null && cvAttrs.Length > 0 && cvAttrs[0].Value)
                        continue; // Ancestor is ComVisible

                    // Or an explicit interface must be set
                    ComDefaultInterfaceAttribute[] cdiAttrs = (ComDefaultInterfaceAttribute[])t.GetCustomAttributes(typeof(ComDefaultInterfaceAttribute), true);

                    if (cdiAttrs != null && cdiAttrs.Length > 0)
                    {
                        Trace.Assert(cdiAttrs[0].Value.IsAssignableFrom(t));
                        continue; // A default interface is available
                    }

                    // Or there is no IDispatch implemented
                    ClassInterfaceAttribute[] ciAttrs = (ClassInterfaceAttribute[])t.GetCustomAttributes(typeof(ClassInterfaceAttribute), true);

                    if (ciAttrs != null && ciAttrs.Length > 0 && ciAttrs[0].Value == ClassInterfaceType.None)
                        continue;

                    throw new InvalidOperationException(string.Format("{0} is ComVisible but not IDispatch compatible", t.FullName));
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
