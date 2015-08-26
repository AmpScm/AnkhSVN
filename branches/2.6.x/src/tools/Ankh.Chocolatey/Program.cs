using System;
using System.IO;
using System.Xml;

namespace Ankh.Chocolatey
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Usage: Ankh.Chocolatey <msi> <outputdir> <url>");
                return 1;
            }

            string msiFile = args[0];
            string dir = args[1];
            string url = args[2];
            string toolsDir = Path.Combine(dir, "tools");
            

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!Directory.Exists(toolsDir))
                Directory.CreateDirectory(toolsDir);

            NativeMethods.MsiSetInternalUI(2, IntPtr.Zero); // Hide all UI. Without this you get a MSI dialog

            MsiHandle msi;
            uint err;
            if (0 != (err = NativeMethods.MsiOpenPackageW(args[0], out msi)))
            {
                Console.Error.WriteLine("Can't open MSI, error {0}", err);
                return 1;
            }

            // Strings available in all MSIs
            string productCode;
            string productName;
            string productVersion;
            string manufacturer;
            // Strings available in recent AnkhSVN MSIs
            string shortName;
            string pkgLicenseUrl;
            string pkgProjectUrl;
            string pkgIconUrl;
            string pkgDescription;
            string pkgCopyright;
            string pkgTags;

            using (msi)
            {

                // Obtain several MSI properties
                if (0 != NativeMethods.MsiGetProductProperty(msi, "ProductCode", out productCode))
                    throw new InvalidOperationException("Can't obtain product code");

                if (0 != NativeMethods.MsiGetProductProperty(msi, "ProductName", out productName))
                    throw new InvalidOperationException("Can't obtain product code");

                if (0 != NativeMethods.MsiGetProductProperty(msi, "ProductVersion", out productVersion))
                    throw new InvalidOperationException("Can't obtain product version");

                if (0 != NativeMethods.MsiGetProductProperty(msi, "Manufacturer", out manufacturer))
                    throw new InvalidOperationException("Can't obtain manufacturer");

                if (0 != NativeMethods.MsiGetProductProperty(msi, "ShortProductName", out shortName))
                {
                    shortName = productName.Replace(productVersion, "").Trim();
                }

                NativeMethods.MsiGetProductProperty(msi, "Pkg_LicenseUrl", out pkgLicenseUrl);
                NativeMethods.MsiGetProductProperty(msi, "Pkg_ProjectUrl", out pkgProjectUrl);
                NativeMethods.MsiGetProductProperty(msi, "Pkg_IconUrl", out pkgIconUrl);
                NativeMethods.MsiGetProductProperty(msi, "Pkg_Description", out pkgDescription);
                NativeMethods.MsiGetProductProperty(msi, "Pkg_Copyright", out pkgCopyright);
                NativeMethods.MsiGetProductProperty(msi, "Pkg_Tags", out pkgTags);
            }
            string spec = Path.Combine(dir, shortName + ".spec");

            Console.WriteLine("Generating {0}", spec);

            string ns = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            using (XmlWriter xw = XmlWriter.Create(spec, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("package", ns);
                xw.WriteComment(string.Format("Extracted from {0}: {1}, productcode={2}", Path.GetFileName(args[0]), productName, productCode));
                xw.WriteStartElement("metadata", ns);

                xw.WriteElementString("id", ns, shortName);
                xw.WriteElementString("version", ns, productVersion);

                xw.WriteElementString("authors", ns, manufacturer);

                xw.WriteElementString("owners", ns, "AnkhSVN");
                xw.WriteElementString("licenseUrl", ns, pkgLicenseUrl ?? "");
                xw.WriteElementString("projectUrl", ns, pkgProjectUrl ?? "");
                xw.WriteElementString("iconUrl", ns, pkgIconUrl ?? "");
                xw.WriteElementString("requireLicenseAcceptance", ns, "true");
                xw.WriteElementString("description", ns, pkgDescription ?? "");
                xw.WriteElementString("copyright", ns, pkgCopyright ?? "");
                xw.WriteElementString("tags", ns, pkgTags ?? "");

                xw.WriteEndElement(); // /metadata

                xw.WriteStartElement("files", ns);

                xw.WriteStartElement("file", ns);
                xw.WriteAttributeString("target", "tools");
                xw.WriteAttributeString("src", "tools/ChocolateyInstall.ps1");
                xw.WriteEndElement();

                xw.WriteStartElement("file", ns);
                xw.WriteAttributeString("target", "tools");
                xw.WriteAttributeString("src", "tools/ChocolateyUninstall.ps1");
                xw.WriteEndElement();

                xw.WriteEndElement(); // /files

                xw.WriteEndElement(); // /package
            }

            using(TextWriter tw = File.CreateText(Path.Combine(toolsDir, "ChocolateyInstall.ps1")))
            {
                tw.WriteLine("# Extracted from {0}: {1}, productcode={2}", Path.GetFileName(args[0]), productName, productCode);
                tw.WriteLine("Install-ChocolateyPackage `");
                tw.WriteLine("        '{0}' 'msi' '/passive' `", shortName);
                tw.WriteLine("        '{0}'", url);
                tw.WriteLine();
            }

            using (TextWriter tw = File.CreateText(Path.Combine(toolsDir, "ChocolateyUninstall.ps1")))
            {
                tw.WriteLine("# Extracted from {0}: {1}, productcode={2}", Path.GetFileName(args[0]), productName, productCode);
                tw.WriteLine("$package = 'AnkhSvn'");
                tw.WriteLine("");
                tw.WriteLine("");
                tw.WriteLine("try {");
                tw.WriteLine("  $msiid = '{0}'", productCode);
                tw.WriteLine("  Uninstall-ChocolateyPackage $package 'MSI' -SilentArgs \"$msIid /qb\" -validExitCodes @(0)");
                tw.WriteLine("  ");
                tw.WriteLine("  # the following is all part of error handling");
                tw.WriteLine("  Write-ChocolateySuccess $package");
                tw.WriteLine("} catch {");
                tw.WriteLine("  Write-ChocolateyFailure $package \"$($_.Exception.Message)\"");
                tw.WriteLine("}");
            }



            return 0;
        }
    }
}
