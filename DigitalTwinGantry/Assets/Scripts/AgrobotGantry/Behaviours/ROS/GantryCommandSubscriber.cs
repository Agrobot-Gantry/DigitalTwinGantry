using RosSharp.RosBridgeClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GantryCommandSubscriber : UnitySubscriber<RosSharp.RosBridgeClient.MessageTypes.Std.String>
{
	public Action<string, string> Callback { get; set; }

	protected override void ReceiveMessage(RosSharp.RosBridgeClient.MessageTypes.Std.String message)
	{
		Callback.Invoke(Topic, message.data);
	}
}