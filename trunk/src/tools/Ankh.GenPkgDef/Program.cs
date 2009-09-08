using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.Shell;

namespace Ankh.GenPkgDef
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: Ankh.GenPkgDef <output> <assemblies....>");
                Environment.ExitCode = 1;
                return;
            }

            using (StreamWriter pkgdef = File.CreateText(args[0]))
            {
                for (int i = 1; i < args.Length; i++)
                {
                    Assembly asm = Assembly.LoadFrom(args[i]);

                    foreach (Type type in asm.GetTypes())
                    {
                        if (type.IsAbstract)
                            continue;

                        using(PkgDefContext ctx = new PkgDefContext(pkgdef, type))
                        foreach (RegistrationAttribute ra in type.GetCustomAttributes(typeof(RegistrationAttribute), true))
                        {
                            ra.Register(ctx);
                        }
                    }
                }
            }
        }
    }
}
