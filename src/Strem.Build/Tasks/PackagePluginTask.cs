using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Frosting;

namespace Strem.Build.Tasks;

[IsDependentOn(typeof(CleanDirectoriesTask))]
public class PackagePluginTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var outputDirectory = $"{Directories.Dist}/Strem.OBS.v4-{context.Version}";
        if(!Directory.Exists(outputDirectory))
        { context.CreateDirectory(outputDirectory); }

        var pluginProject = $"{Directories.Src}/Strem.OBS.v4/Strem.OBS.v4.csproj";
        var publishSettings = new DotNetBuildSettings
        {
            Configuration = "Release",
            OutputDirectory = outputDirectory,
            MSBuildSettings = new DotNetMSBuildSettings
            {
                Version = context.Version
           }
        };
        context.DotNetBuild(pluginProject, publishSettings);
        context.Zip(outputDirectory, $"{outputDirectory}.zip");
    }
}