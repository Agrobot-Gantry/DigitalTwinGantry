using RosSharp.RosBridgeClient;
using System;

/// <summary>
/// Listens for strings on the set topic. Received messages will be received in the callback(topic, message).
/// </summary>
class GantryCommandSubscriber : UnitySubscriber<RosSharp.RosBridgeClient.MessageTypes.Std.String>
{
	public Action<string, string> Callback { get; set; }

	protected override void ReceiveMessage(RosSharp.RosBridgeClient.MessageTypes.Std.String message)
	{
		Callback.Invoke(Topic, message.data);
	}
}