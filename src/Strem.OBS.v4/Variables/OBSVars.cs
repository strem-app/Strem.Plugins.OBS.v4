﻿using Strem.Core.Variables;

namespace Strem.OBS.v4.Variables;

public class OBSVars
{
    public static readonly string OBSContext = "OBS-v4";

    public static readonly VariableEntry Host = new("host", OBSContext);
    public static readonly VariableEntry Port = new("port", OBSContext);
    public static readonly VariableEntry Password = new("password", OBSContext);
    
    public static readonly VariableEntry CurrentScene = new("current-scene", OBSContext);
    public static readonly VariableEntry SourceItems = new("source-items", OBSContext);
}