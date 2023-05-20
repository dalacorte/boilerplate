## User Input
$ProjectName = Read-Host 'Placeholder';

## Variables
$BaseName = 'Placeholder';
$Exclude = @("*.ps1", "*.md")


Write-Host "- Renaming project from ($BaseName) to ($ProjectName)";

$count = 1;
Write-Host "1 - Renaming files...";
Get-ChildItem -Include "*$BaseName*" -Exclude $Exclude -File -Recurse | ForEach `
{
    $NomeReplace = $_.Name -replace ($BaseName,$ProjectName); `
    Write-Host " 1.$count - File: ($_) => ($NomeReplace) "; `
    Rename-Item -Path $_ -NewName ($_.Name.Replace($BaseName,$ProjectName));
    $count++; `
};

$count = 1;
Write-Host "2 - Renaming files content...";
Get-ChildItem -Exclude $Exclude -File -Recurse | Where-Object { Select-String -Pattern $BaseName $_ -Quiet } | ForEach `
{
    Write-Host " 2.$count - File: ($_)"; `
    $conteudoReplace = (Get-Content -Path $_ -Raw) -replace ($BaseName,$ProjectName); `
    Set-Content -Path $_.FullName -Value $conteudoReplace; `
    $count++;
};

$count = 1;
Write-Host "3 - Renaming folers..."
Get-ChildItem -Path "*" -Filter "*$BaseName*" -Directory -Recurse | ForEach-Object `
{ 
    $FullFolderName = $_.FullName; `
    $NewFolderName = $_.Name -replace ($BaseName,$ProjectName); `
    Write-Host " 3.$count - Folder: ($FullFolderName) => $NewFolderName"; `
    Rename-Item -Path $_.FullName -NewName $NewFolderName; `
    $count++;
};

Read-Host “Press Enter...”