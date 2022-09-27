using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Frosting;

namespace Strem.Build.Tasks;

[IsDependentOn(typeof(CleanDirectoriesTask))]
public class PackagePluginTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var pluginName = $"Strem.OBS.v4";
        var pluginTempOutput = $"{Directories.Dist}/plugin-temp";
        var pluginContainerFolder = $"{Directories.Dist}/plugin/";
        var pluginFinalOutput = $"{pluginContainerFolder}/{pluginName}";
        if(!Directory.Exists(pluginTempOutput)) { context.CreateDirectory(pluginTempOutput); }
        if(!Directory.Exists(pluginFinalOutput)) { context.CreateDirectory(pluginFinalOutput); }

        var pluginProject = $"{Directories.Src}/Strem.OBS.v4/Strem.OBS.v4.csproj";
        var publishSettings = new DotNetPublishSettings
        {
            Configuration = "Release",
            OutputDirectory = pluginTempOutput,
            MSBuildSettings = new DotNetMSBuildSettings
            {
                Version = context.Version
            }
        };
        context.DotNetPublish(pluginProject, publishSettings);

        context.MoveFile($"{pluginTempOutput}/Obs.v4.WebSocket.dll", $"{pluginFinalOutput}/Obs.v4.WebSocket.dll");
        context.MoveFile($"{pluginTempOutput}/Obs.v4.WebSocket.Reactive.dll", $"{pluginFinalOutput}/Obs.v4.WebSocket.Reactive.dll");
        context.MoveFile($"{pluginTempOutput}/WebSocket4Net.dll", $"{pluginFinalOutput}/WebSocket4Net.dll");
        context.MoveFile($"{pluginTempOutput}/SuperSocket.ClientEngine.dll", $"{pluginFinalOutput}/SuperSocket.ClientEngine.dll");
        context.MoveFile($"{pluginTempOutput}/Strem.OBS.v4.dll", $"{pluginFinalOutput}/Strem.OBS.v4.dll");
        context.Zip(pluginContainerFolder, $"{Directories.Dist}/{pluginName}.zip");
    }
}