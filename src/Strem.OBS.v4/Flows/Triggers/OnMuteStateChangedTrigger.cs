﻿using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Obs.v4.WebSocket;
using Obs.v4.WebSocket.Reactive;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Processors;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.OBS.v4.Variables;
using Strem.OBS.v4.Extensions;

namespace Strem.OBS.v4.Flows.Triggers;

public class OnMuteStateChangedTrigger : FlowTrigger<OnMuteStateChangedTriggerData>
{
    public override string Code => OnMuteStateChangedTriggerData.TriggerCode;
    public override string Version => OnMuteStateChangedTriggerData.TriggerVersion;

    public override string Name => "On Mute State Changed";
    public override string Category => "OBS v4";
    public override string Description => "Triggers when a mute state has changed on a specific source";

    public static VariableEntry ObsSourceVariable = new("source.name", OBSVars.OBSContext);
    public static VariableEntry ObsSourceMuteStateVariable = new("source.mute-state", OBSVars.OBSContext);
    public static VariableEntry ObsSourceIsMutedVariable = new("source.is-muted", OBSVars.OBSContext);

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ObsSourceVariable.ToDescriptor(), ObsSourceMuteStateVariable.ToDescriptor(), ObsSourceIsMutedVariable.ToDescriptor()
    };

    public IObservableOBSWebSocket ObsClient { get; }

    public OnMuteStateChangedTrigger(ILogger<FlowTrigger<OnMuteStateChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost();

    public override async Task<IObservable<IVariables>> Execute(OnMuteStateChangedTriggerData data)
    {
        if(string.IsNullOrEmpty(data.SourceName))
        { return Observable.Empty<IVariables>(); }
        
        var observableChain = ObsClient.OnSourceMuteStateChanged;
        if (data.TriggerOnStart)
        {
            var muteState = await GetInitialValue(data);
            var args = new SourceMuteStateChangedEventArgs { SourceName = data.SourceName, Muted = muteState };
            observableChain = observableChain.StartWith(args);
        }

        return observableChain
                .Where(x => MatchesEvent(data, x))
                .Select(PopulateVariables);
    }

    public async Task<bool> GetInitialValue(OnMuteStateChangedTriggerData data)
    {
        if(!ObsClient.IsConnected) { return false; }
        return await ObsClient.GetMute(data.SourceName);
    }

    public bool MatchesEvent(OnMuteStateChangedTriggerData data, SourceMuteStateChangedEventArgs args)
    {
        if(!args.SourceName.Equals(data.SourceName, StringComparison.OrdinalIgnoreCase))
        { return false; }
                
        if(data.TriggerOnMuted && args.Muted) { return true; }
        if(data.TriggerOnUnmuted && !args.Muted) { return true; }

        return false;
    }
    
    public IVariables PopulateVariables(SourceMuteStateChangedEventArgs args)
    {
        var variables = new Core.Variables.Variables();
        variables.Set(ObsSourceVariable, args.SourceName);
        variables.Set(ObsSourceIsMutedVariable, args.Muted);
        variables.Set(ObsSourceMuteStateVariable, args.Muted ? "Muted" : "Unmuted");
        return variables;
    }
}