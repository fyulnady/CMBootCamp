# Generate build label
if($env:BUILD_NUMBER -ne $null) {
    $env:buildlabel = "$env:TEAMCITY_PROJECT_NAME $env:TEAMCITY_BUILDCONF_NAME $env:BUILD_NUMBER on $(Get-Date -Format g)"
    $env:buildconfig = "Release"
}
else {
    $env:buildlabel = "Manual Build on $(Get-Date -Format g)"
    $env:buildconfig = "Debug"
}

Framework "4.5.1"

properties {
    $projectName = "ClearMeasure.Bootcamp"
    $unitTestAssembly = "ClearMeasure.Bootcamp.UnitTests.dll"
    $integrationTestAssembly = "ClearMeasure.Bootcamp.IntegrationTests.dll"
	$projectConfig = "Release"
	$base_dir = resolve-path .\
	$source_dir = "$base_dir\src"
    $nunitPath = "$source_dir\packages\NUnit*\Tools"
	
	$build_dir = "$base_dir\build"
	$test_dir = "$build_dir\test"
	$testCopyIgnorePath = "_ReSharper"
	$package_dir = "$build_dir\package"	
	$package_file = "$build_dir\latestVersion\" + $projectName +"_Package.zip"

    $databaseName = $projectName
    $databaseServer = if([Environment]::GetEnvironmentVariable("dbServer","User") -eq $null) { "localhost\SQLEXPRESS2014" } else { [Environment]::GetEnvironmentVariable("dbServer","User")}
    $databaseScripts = "$source_dir\Database\scripts"
    $hibernateConfig = "$source_dir\hibernate.cfg.xml"
    $schemaDatabaseName = $databaseName + "_schema"
    $integratedSecurity = "Integrated Security=true"

	$databaseUsername = "bootcamp"
	$databasePassword = "9Db12345678"
    
    $connection_string = "server=$databaseserver;database=$databasename;$databaseUser;"
    $AliaSql = "$source_dir\Database\scripts\AliaSql.exe"
    $webapp_dir = "$source_dir\UI"
}

task default -depends Init, Compile, RebuildDatabase, Test, LoadData
task ci -depends Init, CommonAssemblyInfo, ConnectionString, Compile, RebuildDatabase, Test, Package

task Init {
    delete_file $package_file
    delete_directory $build_dir
    create_directory $test_dir
    create_directory $build_dir
}

task ConnectionString {
    $connection_string = "server=$databaseserver;database=$databasename;$integratedSecurity;"
    write-host "Using connection string: $connection_string"
    if ( Test-Path "$hibernateConfig" ) {
        poke-xml $hibernateConfig "//e:property[@name = 'connection.connection_string']" $connection_string @{"e" = "urn:nhibernate-configuration-2.2"}
    }
}

task Compile -depends Init {
    exec {
        & msbuild /t:Clean`;Rebuild /v:q /nologo /p:Configuration=$projectConfig $source_dir\$projectName.sln
    }
}

task Test {
    copy_all_assemblies_for_test $test_dir
    exec {
        & $nunitPath\nunit-console.exe $test_dir\$unitTestAssembly $test_dir\$integrationTestAssembly /nologo /xml=$build_dir\TestResult.xml
    }
}

task RebuildDatabase -depends ConnectionString {
    exec {
        & $AliaSql Rebuild $databaseServer $databaseName $databaseScripts
    }
}

task RebuildRemoteDatabase {
    write-host "Using database username: $databaseUsername"
    exec {
        & $AliaSql Rebuild $databaseServer $databaseName $databaseScripts $databaseUsername $databasePassword
    }
}

task LoadData -depends ConnectionString, Compile, RebuildDatabase {
    exec { 
		& $nunitPath\nunit-console.exe $test_dir\$integrationTestAssembly /include=DataLoader /nologo /nodots /xml=$build_dir\DataLoadResult.xml
    } "Build failed - data load failure"  
}

task CreateCompareSchema -depends SchemaConnectionString {
    exec {
        & $base_dir\aliasql\aliasql.exe Rebuild $databaseServer $schemaDatabaseName $databaseScripts
    }
}

task SchemaConnectionString {
    $connection_string = "server=$databaseserver;database=$schemaDatabaseName;@integratedSecurity;"
    write-host "Using connection string: $connection_string"
    #if ( Test-Path "$hibernateConfig" ) {
    #    poke-xml $hibernateConfig "//e:property[@name = 'connection.connection_string']" $connection_string @{"e" = "urn:nhibernate-configuration-2.2"}
    #}
}

task CommonAssemblyInfo {
    $version = "1.1.0.0"   
    create-commonAssemblyInfo "$version" $projectName "$source_dir\CommonAssemblyInfo.cs"
}

task Package {
    delete_directory $package_dir
	#web app
    copy_website_files "$webapp_dir" "$package_dir\web" 
    copy_files "$databaseScripts" "$package_dir\database"
	
	zip_directory $package_dir $package_file 
}
 
























function global:zip_directory($directory,$file) {
    write-host "Zipping folder: " $test_assembly
    delete_file $file
    cd $directory
    & "$base_dir\tools\7zip\7z.exe" a -mx=9 -r $file
    cd $base_dir
}
function global:copy_website_files($source,$destination){
    $exclude = @('*.user','*.dtd','*.tt','*.cs','*.csproj','*.orig', '*.log') 
    copy_files $source $destination $exclude
	delete_directory "$destination\obj"
}

function global:copy_files($source,$destination,$exclude=@()){    
    create_directory $destination
    Get-ChildItem $source -Recurse -Exclude $exclude | Copy-Item -Destination {Join-Path $destination $_.FullName.Substring($source.length)} 
}

function global:Copy_and_flatten ($source,$filter,$dest) {
  ls $source -filter $filter  -r | Where-Object{!$_.FullName.Contains("$testCopyIgnorePath") -and !$_.FullName.Contains("packages") }| cp -dest $dest -force
}

function global:copy_all_assemblies_for_test($destination){
  create_directory $destination
  Copy_and_flatten $source_dir *.exe $destination
  Copy_and_flatten $source_dir *.dll $destination
  Copy_and_flatten $source_dir *.config $destination
  Copy_and_flatten $source_dir *.xml $destination
  Copy_and_flatten $source_dir *.pdb $destination
  Copy_and_flatten $source_dir *.sql $destination
  Copy_and_flatten $source_dir *.xlsx $destination
}

function global:delete_file($file) {
    if($file) { remove-item $file -force -ErrorAction SilentlyContinue | out-null } 
}

function global:delete_directory($directory_name)
{
  rd $directory_name -recurse -force  -ErrorAction SilentlyContinue | out-null
}

function global:delete_files_in_dir($dir)
{
	get-childitem $dir -recurse | foreach ($_) {remove-item $_.fullname}
}

function global:create_directory($directory_name)
{
  mkdir $directory_name  -ErrorAction SilentlyContinue  | out-null
}

function global:create-commonAssemblyInfo($version,$applicationName,$filename)
{
"using System;
using System.Reflection;
using System.Runtime.InteropServices;

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: ComVisibleAttribute(false)]
[assembly: AssemblyVersionAttribute(""$version"")]
[assembly: AssemblyFileVersionAttribute(""$version"")]
[assembly: AssemblyCopyrightAttribute(""Copyright 2015"")]
[assembly: AssemblyProductAttribute(""$applicationName"")]
[assembly: AssemblyCompanyAttribute(""Clear Measure, Inc."")]
[assembly: AssemblyConfigurationAttribute(""release"")]
[assembly: AssemblyInformationalVersionAttribute(""$version"")]"  | out-file $filename -encoding "ASCII"    
}

function script:poke-xml($filePath, $xpath, $value, $namespaces = @{}) {
    [xml] $fileXml = Get-Content $filePath
    
    if($namespaces -ne $null -and $namespaces.Count -gt 0) {
        $ns = New-Object Xml.XmlNamespaceManager $fileXml.NameTable
        $namespaces.GetEnumerator() | %{ $ns.AddNamespace($_.Key,$_.Value) }
        $node = $fileXml.SelectSingleNode($xpath,$ns)
    } else {
        $node = $fileXml.SelectSingleNode($xpath)
    }
    
    Assert ($node -ne $null) "could not find node @ $xpath"
        
    if($node.NodeType -eq "Element") {
        $node.InnerText = $value
    } else {
        $node.Value = $value
    }

    $fileXml.Save($filePath) 
} 
