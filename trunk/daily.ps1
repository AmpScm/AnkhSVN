# Script to create daily "dogfood" builds of AnkhSVN
# run from Windows Scheduler with the following command line:
# C:\WINDOWS\system32\cmd.exe /k "D:\Program Files\Microsoft Visual Studio .NET\Common7\Tools\vsvars32.bat" && powershell.exe D:\mydocs\vcsharp\ankh\daily.ps1

# the /k leaves the command prompt open after running the build, change to /c to have it closed

# The label for the build
$label = "daily_{0}" -f ([datetime]::Now.ToString("ddMMyyyy_HH.mm"))
    
# extract the build script to the tmp directory
$deploy = "${env:TEMP}\deploy.py"

# need to use set-content here, using > causes the file to be UCS-2 or UTF16
svn cat http://ankhsvn.com/svn/finalproject/trunk/deploy.py | set-content $deploy
    
# remove any existing directory called build, so the build result will always be in a
# predictable place
remove-item -recurse -force build

# Do the build
python $deploy LABEL=$label TMP=D:\tmp

if ($LASTEXITCODE -ne 0)
{
    throw "Build failure"
}

# get rid of any existing instances of devenv
# if any were alive, installing the MSI would cause a reboot
get-process -name devenv | stop-process

# find the generated MSI
set-location build
$msi = (get-childitem *.msi)

# and install it quietly
msiexec /q /i $msi