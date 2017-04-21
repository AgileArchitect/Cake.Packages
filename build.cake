
#addin "Cake.Git"


var target = Argument("target", "Default");
var buildNumber = AppVeyor.Environment.Build.Number;

var local = BuildSystem.IsLocalBuild;
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;

var version = "0.1.2";
var semVersion = local ? version : (version + string.Concat("+", buildNumber));

var artifacts = "./artifacts";

var branchName = isRunningOnAppVeyor ? EnvironmentVariable("APPVEYOR_REPO_BRANCH") : GitBranchCurrent(DirectoryPath.FromString(".")).FriendlyName;
var isMasterBranch = System.String.Equals("master", branchName, System.StringComparison.OrdinalIgnoreCase);
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;

var solution = "Cake.Packages.sln";

Task("Clean")
    .Does(() => {
        CreateDirectory(artifacts);
        CleanDirectory(artifacts);
    });

Task("Restore-Nuget-Packages")
	.IsDependentOn("Clean")
	.Does(() => {
		NuGetRestore(solution);
	});

Task("Build")
    .IsDependentOn("Restore-Nuget-Packages")
    .Does(() => {
        DotNetBuild(solution, s => s.SetConfiguration("Release").SetVerbosity(Verbosity.Quiet));
    });

Task("Package")
    .IsDependentOn("Build")
    .Does(() => {
        



        NuGetPack(new NuGetPackSettings {
                                Id                      = "Cake.Packages", 
                                Title                   = "Cake.Packages", 
                                Version                 = version, 
                                Description             = "Generated Dashboard Controllers", 
                                Authors                 = new[] {"Tom Staijen"}, 
                                Files                   = new [] 
                                { 
                                                        new NuSpecContent {Source = "Cake.Packages.dll", Target = "/lib/net45"}, 
                                                        new NuSpecContent {Source = "Cake.Packages.pdb", Target = "/lib/net45"}, 
                                }, 
                                BasePath                = "./Cake.Packages/bin/Release", 
                                OutputDirectory         = artifacts 
        });
    });

Task("Publish")
	.IsDependentOn("Package")
    .WithCriteria(() => isRunningOnAppVeyor)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isMasterBranch)
	.Does(() => {
		
	    var apiKey = EnvironmentVariable("NUGET_API_KEY");

    	if(string.IsNullOrEmpty(apiKey))    
        	throw new InvalidOperationException("Could not resolve Nuget API key.");
		
		var package = artifacts + "/Cake.Packages." + version + ".nupkg";
            
		// Push the package.
		NuGetPush(package, new NuGetPushSettings {
    		Source = "https://www.nuget.org/api/v2/package",
    		ApiKey = apiKey
		});
	});

Task("Update-AppVeyor-Build-Number")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(semVersion);
});

Task("AppVeyor")
	.IsDependentOn("Update-AppVeyor-Build-Number")
	.IsDependentOn("Publish");
	

Task("Default")
    .IsDependentOn("Package");


RunTarget(target);