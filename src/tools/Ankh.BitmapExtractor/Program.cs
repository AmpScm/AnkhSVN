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
        // This code requires VSCTLibrary.dll and VSCTCompress.dll, which are available as part of the VS-SDK
        // and as part of The VSCT Powertoy
        static void Main(string[] args)
        {
            List<string> argList = new List<string>(args);
            bool _transparentHack = false;
            while (argList.Count > 0 && argList[0].StartsWith("-"))
            {
                string arg = argList[0];
                if (arg == "--")
                {
                    argList.RemoveAt(0);
                    break;
                }
                switch (arg)
                {
                    case "-th":
                        _transparentHack = true;
                        break;
                    default:
                        Console.Error.WriteLine("Unhandled argument {0}", arg);
                        Environment.Exit(1);
                        break;
                }
                argList.RemoveAt(0);
            }

            if (argList.Count < 2)
            {
                Console.Error.WriteLine("Required argument missing");
                Console.Error.WriteLine("BitmapExtractor [-th] <assembly> <dir>");
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


            try
            {
                if (asm != null
                    ? !table.Read(Assembly.LoadFile(from), new ReadErrorHandler())
                    : !table.Read(from, new ReadErrorHandler()))
                {
                    Console.Error.WriteLine("* Loading failed, exiting *");
                    Environment.Exit(1);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                Environment.Exit(1);
            }

            BitmapItemList bitmaps = table.GetBitmapList();

            foreach (BitmapItem bi in bitmaps)
            {
                uint resourceId = bi.IDResource;
                Dictionary<uint, int> map = new Dictionary<uint, int>();
                Color transparentColor = Color.FromArgb(0xFF, 0xFF, 0, 0xFF);
                bool haveColor = false;

                for (int i = 0; i < bi.UsedSlots.Length; i++)
                {
                    map[bi.UsedSlots[i]] = i;
                    if (_transparentHack && !haveColor && bi.UsedSlots[i] == 1)
                    {
                        Bitmap bm = bitmaps.GetBitmap(bi.GID, bi.UsedSlots[i]);
                        transparentColor = bm.GetPixel(0, 0);
                        Console.WriteLine("Found color: {0}", transparentColor);
                        haveColor = true;
                    }
                }

                ButtonList bl = table.GetButtonList();

                foreach (CommandButton cb in table.GetButtonList())
                {
                    if (cb.IconGID != bi.GID)
                        continue;

                    Bitmap bm = bitmaps.GetBitmap(cb.IconGID, cb.IconIndex);
                    string name = cb.CanonicalName.Trim(' ', '.', '\t', '&').Replace(" ", "").Replace("&", "");

                    if (bm == null)
                    {
                        Console.WriteLine("Couldn't get icon for {0}", name);
                        continue;
                    }

                    if (_transparentHack)
                        bm.MakeTransparent(transparentColor);

                    Console.WriteLine("Writing {0}", name);
                    bm.Save(Path.Combine(dir, name + ".png"), ImageFormat.Png);
                }
            }
            Console.WriteLine("Done");
        }
    }
}
