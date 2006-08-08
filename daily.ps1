# Script to create daily "dogfood" builds of AnkhSVN
# run from Windows Scheduler with the following command line:
# C:\WINDOWS\system32\cmd.exe /k "D:\Program Files\Microsoft Visual Studio .NET\Common7\Tools\vsvars32.bat" && powershell.exe D:\mydocs\vcsharp\ankh\daily.ps1

# the /k leaves the command prompt open after running the build, change to /c to have it closed

# The label for the build

Write-Progress -Activity "Building Ankh" "Initializing"
$label = "Daily_{0}" -f ([datetime]::Now.ToString("ddMMyyyy_HH.mm"))
    
# extract the build script to the tmp directory
$deploy = "${env:TEMP}\deploy.py"

Write-Debug "deploy.py to be retrieved to $deploy"

Write-Progress -Activity "Building Ankh" -Status "Retrieving deploy.py"
# need to use set-content here, using > causes the file to be UCS-2 or UTF16
svn cat http://ankhsvn.com/svn/finalproject/trunk/deploy.py | set-content $deploy
    
# get rid of any existing instances of devenv
# if any were alive, installing the MSI would cause a reboot
# Use -ErrorAction SilentlyContinue to keep this from throwing an error if there is no such process
Write-Progress -Activity "Building Ankh" -Status "Stopping existing devenv.exe processes"
$procs = get-process -name devenv -ErrorAction SilentlyContinue 

if ( $procs -ne $null )
{
    $procs | foreach{
        stop-process -InputObject $_
        $_.WaitForExit()        
    }
}
   
Write-Progress -Activity "Building Ankh" -Status "Setting registry key so Ankh won't load"

# Make sure Ankh doesn't load 
"hklm","hkcu" | foreach {
	$root = $_
	"7.0","7,1","8.0" | foreach	{
		$version = $_
		$path = "${root}:\Software\Microsoft\VisualStudio\${version}\Addins\Ankh"
		Write-Host $path
		if ( Test-Path $path )
		{		
			Set-ItemProperty -Path $path -Name LoadBehavior -Value 0
		}
	}
}

Write-Progress -Activity "Building Ankh" -Status "Removing existing directories named build"
# remove any existing directory called build, so the build result will always be in a
# predictable place
remove-item -recurse -force "build"

if ( Test-Path "build" )
{
    throw "Unable to delete build directory"
}

# Do the build
Write-Progress -Activity "Building Ankh" -Status "Running deploy.py"
python $deploy LABEL=$label TMP=D:\tmp ANKHSVN=http://ankhsvn.com/svn/finalproject/trunk/src

if ($LASTEXITCODE -ne 0)
{
    throw "Build failure"
}




Write-Progress -Activity "Waiting for devenv" -Status "Waiting for all instances of devenv to be closed"
do
{    
    [Threading.Thread]::Sleep(1000)
    $procs = (get-process devenv* -ErrorAction "SilentlyContinue")
    if ( $procs )
    {
        Write-Debug ("Count: {0}" -f $procs.Count)
        Write-Progress -Activity "Waiting for devenv" -Status ("{0} instances remaining" -f ($procs.Count))
    }
} while( $procs -ne $null)

function RunMsiExecAndWait($arguments)
{
	$proc = [System.Diagnostics.Process]::Start("msiexec.exe", $arguments)
	$proc.WaitForExit()
}


Write-Progress -Activity "Installing Ankh" -Status "Uninstalling previous versions"
Get-Wmiobject win32_product | where { $_.Name -match "Ankh" } | foreach { 
	RunMsiExecAndWait( "/qb /x {$_}.IdentifyingNumber /q /norestart" )
}


# find the generated MSI
set-location build
$msi = (get-childitem *.msi)

Write-Debug "Installing from $msi"

# and install it quietly
Write-Progress -Activity "Installing Ankh" -Status "Running MSI installer from $msi"
RunMsiExecAndWait( "/qb /i $msi /norestart ALLUSERS=1" ) 

#remove-item -recurse -force build -ErrorAction SilentlyContinue