﻿using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Triggers;

namespace Strem.OBS.v4.Flows.Triggers;

public class OnStreamingStateChangedTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-obs-v4-stream-state-changed";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;

    [Required]
    public string SourceName { get; set; }
    public bool TriggerOnStarted { get; set; }
    public bool TriggerOnStopped { get; set; }
    public bool TriggerOnStart { get; set; }
}