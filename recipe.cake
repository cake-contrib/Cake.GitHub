#load nuget:?package=Cake.Recipe&version=3.1.1

//*************************************************************************************************
// Settings
//*************************************************************************************************

Environment.SetVariableNames();

BuildParameters.SetParameters(
    context: Context,
    buildSystem: BuildSystem,
    sourceDirectoryPath: "./src",
    title: "Cake.GitHub",
    repositoryOwner: "cake-contrib",
    repositoryName: "Cake.GitHub",
    appVeyorAccountName: "cakecontrib",
    shouldUseDeterministicBuilds: true,
    shouldRunCodecov: false,
    shouldGenerateDocumentation: false,
    shouldRunInspectCode: false,
    shouldRunCoveralls: false);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(
    context: Context,
    skipDuplicatePackages: true,
    testCoverageFilter: "+[*]* -[xunit.*]* -[Cake.Core]* -[Cake.Testing]* -[*.Tests]* -[Cake.GitHub.Reporting.Generic]LitJson.* -[Shouldly]* -[DiffEngine]* -[EmptyFiles]*",
    testCoverageExcludeByAttribute: "*.ExcludeFromCodeCoverage*",
    testCoverageExcludeByFile: "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs");

//*************************************************************************************************
// Execution
//*************************************************************************************************

Build.RunDotNetCore();