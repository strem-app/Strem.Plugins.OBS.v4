﻿using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Obs.v4.WebSocket.Reactive;
using Strem.OBS.v4.Extensions;

namespace Strem.OBS.v4.Flows.Tasks;

public class ChangeSceneTask : FlowTask<ChangeSceneTaskData>
{
    public override string Code => ChangeSceneTaskData.TaskCode;
    public override string Version => ChangeSceneTaskData.TaskVersion;
    
    public override string Name => "Change Scene";
    public override string Category => "OBS v4";
    public override string Description => "Changes the active scene in OBS";

    public IObservableOBSWebSocket ObsClient { get; }

    public ChangeSceneTask(ILogger<FlowTask<ChangeSceneTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(ChangeSceneTaskData data, IVariables flowVars)
    {
        if(string.IsNullOrEmpty(data.NewScene))
        { return ExecutionResult.Failed("NewScene is empty and is required"); }
        
        await ObsClient.SetCurrentScene(data.NewScene);
        return ExecutionResult.Success();
    }
}
