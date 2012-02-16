using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.CommandTable;

namespace Ankh.BitmapExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> argList = new List<string>(args);
            while(argList.Count > 0 && argList[0].StartsWith("-"))
            {
                string arg = argList[0];
                if (arg == "--")
                {
                    argList.RemoveAt(0);
                    break;
                }

                Console.Error.WriteLine("Unhandled argument {0}", arg);
                Environment.Exit(1);
            }

            if (argList.Count < 2)
            {
                Console.Error.WriteLine("Required argument missing");
                Environment.Exit(1);
            }
            string from = argList[0];
            string dir = argList[1];

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            CommandTable table = new CommandTable();

            Console.WriteLine("Dumping bitmaps in {0} to {1}:", from, dir);

            Assembly asm = null;
            try
            {
                asm = Assembly.LoadFile(from);
            }
            catch { }


            if (asm != null
                ? !table.Read(Assembly.LoadFile(from), new ReadErrorHandler())
                : !table.Read(from, new ReadErrorHandler()))
            {
                Console.Error.WriteLine("* Loading failed, exiting *");
                Environment.Exit(1);
            }

            BitmapItemList bitmaps = table.GetBitmapList();

            foreach (BitmapItem bi in bitmaps)
            {
                uint resourceId = bi.IDResource;
                Dictionary<uint, int> map = new Dictionary<uint, int>();

                for (int i = 0; i < bi.UsedSlots.Length; i++)
                    map[bi.UsedSlots[i]] = i;

                ButtonList bl = table.GetButtonList();
                
                foreach(CommandButton cb in table.GetButtonList())
                {
                    if (map.Count == 0)
                        break;

                    if (cb.IconGID != bi.GID)
                        continue;

                    if (map.ContainsKey(cb.IconIndex))
                    {
                        using (Bitmap bm = bitmaps.GetBitmap(bi.GID, cb.IconIndex))
                        {
                            if (bm.PixelFormat == PixelFormat.Undefined)
                            {
                                Console.WriteLine("Couldn't get icon for {0}", cb.CanonicalName);
                                continue;
                            }

                            string name = cb.CanonicalName.Trim(' ', '.', '\t', '&').Replace(" ", "").Replace("&","");

                            Console.WriteLine("Writing {0}", name);
                            bm.Save(Path.Combine(dir, name + ".png"), ImageFormat.Png);
                        }
                        map.Remove(cb.IconIndex);
                    }
                }
            }
            Console.WriteLine("Done");
        }
    }
}
