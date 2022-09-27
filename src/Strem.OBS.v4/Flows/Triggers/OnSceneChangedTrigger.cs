﻿using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Obs.v4.WebSocket;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Processors;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Obs.v4.WebSocket.Reactive;
using Strem.OBS.v4.Variables;
using Strem.OBS.v4.Extensions;

namespace Strem.OBS.v4.Flows.Triggers;

public class OnSceneChangedTrigger : FlowTrigger<OnSceneChangedTriggerData>
{
    public override string Code => OnSceneChangedTriggerData.TriggerCode;
    public override string Version => OnSceneChangedTriggerData.TriggerVersion;

    public override string Name => "On Scene Changed";
    public override string Category => "OBS v4";
    public override string Description => "Triggers when a scene has been changed, with optional name checking";

    public static VariableEntry ObsSourceVariable = new("scene.name", OBSVars.OBSContext);

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ObsSourceVariable.ToDescriptor()
    };

    public IObservableOBSWebSocket ObsClient { get; }

    public OnSceneChangedTrigger(ILogger<FlowTrigger<OnSceneChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost();

    public override async Task<IObservable<IVariables>> Execute(OnSceneChangedTriggerData data)
    {
        bool NameMatchesIfRequired(SceneChangeEventArgs x) => string.IsNullOrEmpty(data.MatchingSceneName) || 
                                                              x.NewSceneName.Equals(data.MatchingSceneName, StringComparison.OrdinalIgnoreCase);
        
        return ObsClient.OnSceneChanged
            .Where(NameMatchesIfRequired )
            .Select(PopulateVariables);
    }
    
    public IVariables PopulateVariables(SceneChangeEventArgs args)
    {
        var variables = new Core.Variables.Variables();
        variables.Set(ObsSourceVariable, args.NewSceneName);
        return variables;
    }
}