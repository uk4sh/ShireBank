Param(
    [string] $OutputDir
)

if(!$OutputDir) {$OutputDir = "./ShireBankServer/"}

dotnet publish ..\ShireBank.Server\ -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true --output $OutputDir