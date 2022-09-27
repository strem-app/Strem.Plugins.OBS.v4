﻿using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;

namespace Strem.OBS.v4.Flows.Tasks;

public class SetRecordingStateTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-obs-v4-recording-state";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public bool StartRecording { get; set; }
}