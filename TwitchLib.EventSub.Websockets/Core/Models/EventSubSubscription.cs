using System;
using System.Collections.Generic;

namespace TwitchLib.EventSub.Websockets.Core.Models;

/// <summary>
/// Defines an EventSub Subscription
/// </summary>
public class EventSubSubscription
{
    /// <summary>
    /// The ID of the subscription
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// The subscription type name.
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// The subscription type version.
    /// </summary>
    public string Version { get; set; }
    
    /// <summary>
    /// The status of the subscription.
    /// </summary>
    public string Status { get; set; }
    
    /// <summary>
    /// Subscription-specific parameters. The parameters inside this object differ by subscription type and may differ by version.
    /// </summary>
    public Dictionary<string, string> Condition { get; set; }
    
    /// <summary>
    /// Transport-specific parameters.
    /// </summary>
    public EventSubTransport Transport { get; set; }
    
    /// <summary>
    /// The time the subscription was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// How much the subscription counts against your limit. 
    /// </summary>
    public int Cost { get; set; }
}