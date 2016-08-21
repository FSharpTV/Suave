tools\paket.bootstrapper.exe
tools\paket.exe install
path=%path%;%windir%\Microsoft.net\Framework\v4.0.30319
msbuild src\Web.sln
