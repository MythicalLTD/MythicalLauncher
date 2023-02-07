param($installPath, $toolsPath, $package, $project)

$file = Join-Path $toolsPath '..\content\chromedriver.exe' | Get-ChildItem

$project.ProjectItems.Item($file.Name).Delete()

$project.ProjectItems.AddFromFile($file.FullName);
$pi = $project.ProjectItems.Item($file.Name);
$pi.Properties.Item("BuildAction").Value = [int]2;
$pi.Properties.Item("CopyToOutputDirectory").Value = [int]2;