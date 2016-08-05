set SEVENZIP=

if not defined SEVENZIP (
    if exist "C:\Program Files\7-Zip\7z.exe" (
        set SEVENZIP="C:\Program Files\7-Zip\7z.exe"
    )
)

if not defined SEVENZIP (
    echo This script requires that 7-Zip ^(7z.exe^) be in the PATH or that
    echo it be installed to the default location, C:\Program Files\7-Zip.
    echo It is used for creating and restoring backups of custom\install.
    echo Web site: http://www.7-zip.org/
    goto exitfailure
)

%SEVENZIP% a -tzip -mx9 "voltdb-client-csharp-win-latest.zip " ReleaseBuild VoltDB.Examples LICENSE VoltDB.Examples.*.sln -xr!.svn -xr!bin -xr!obj