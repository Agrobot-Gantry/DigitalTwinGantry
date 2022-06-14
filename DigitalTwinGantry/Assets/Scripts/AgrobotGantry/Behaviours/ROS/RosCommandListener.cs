using RosSharp.RosBridgeClient;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Listens for ROS messages as specified in the ROS translation JSON file.
/// When a message is received this will change the gantry behaviour to RosListeningBehaviour if it is not already.
/// Received messages are used to call methods on the RosListeningBehaviour.
/// The ROS app to connect with should be running before the application starts.
/// </summary>
/// <remarks>
/// Be careful when expanding this functionality! The ROS connection on the other side could potentially be abused.
/// Make sure that any functionality triggered by the ROS connection cannot be used to affect the device running
/// this application. Things like reading/writing files can be dangerous even when the ROS application isn't 
/// intentionally malicious.
/// </remarks>
class RosCommandListener : MonoBehaviour
{
	private static readonly string TRANSLATION_FILE_NAME = "RosTranslationTable";

	[SerializeField] private bool m_logRecievedMessages = false;
	private AgrobotGantry m_gantry;
	private RosListeningBehaviour m_behaviour;
	private Dictionary<string, Dictionary<string, MethodInfo>> m_translationTable; //<topic, <message, command>>
	private MainThreadActionQueuer m_actionQueuer;

	//classes for reading the ROS translation JSON
	[System.Serializable]
	public class Translations
	{
		public TranslationEntry[] entries;
	}

	[System.Serializable]
	public class TranslationEntry
	{
		public string topic;
		public string message;
		public string command;
	}

	void Start()
	{
		m_gantry = GetComponent<AgrobotGantry>();
		m_behaviour = new RosListeningBehaviour();
		m_translationTable = new Dictionary<string, Dictionary<string, MethodInfo>>();
		m_actionQueuer = GetComponent<MainThreadActionQueuer>();

		GetComponent<RosConnector>().RosSocket.protocol.OnClosed += OnRosClosed;

		//load json
		TextAsset textAsset = Resources.Load<TextAsset>(TRANSLATION_FILE_NAME);
		if (textAsset == null)
		{
			Debug.LogError("Could not find ROS translation file located at Assets/Resources/" + TRANSLATION_FILE_NAME
				+ "\nROS listening will be disabled");
			return;
		}
		TranslationEntry[] entries = JsonUtility.FromJson<Translations>(textAsset.text).entries;

		//fill dictionary
		foreach (TranslationEntry entry in entries)
		{
			if (!m_translationTable.ContainsKey(entry.topic))
			{
				m_translationTable.Add(entry.topic, new Dictionary<string, MethodInfo>());
			}
			m_translationTable[entry.topic].Add(entry.message, m_behaviour.GetType().GetMethod(entry.command));
			if (m_translationTable[entry.topic][entry.message] == null)
			{
				Debug.LogError("Could not find " + m_behaviour.GetType().Name + " method that matches command named " + entry.command
					+ "\nROS listening will be disabled"
					+ "\nPlease check Assets/Resources/" + TRANSLATION_FILE_NAME);
				return;
			}
		}

		//create a subscriber for each unique topic
		foreach (string topic in m_translationTable.Keys)
		{
			GantryCommandSubscriber subscriber = gameObject.AddComponent<GantryCommandSubscriber>();
			subscriber.Topic = topic;
			subscriber.Callback = OnMessageReceived;
		}
	}

	/// <summary>
	/// Gets called by the GantryCommandSubscribers when they receive a message.
	/// These calls are not on the main thread so an action is queued to handle the message on the main thread.
	/// </summary>
	public void OnMessageReceived(string topic, string message)
	{
		if (m_logRecievedMessages)
		{
			Debug.Log("RosCommandListener received: " + topic + " " + message, this);
		}
		m_actionQueuer.QueueAction(() => HandleMessage(topic, message));
	}

	/// <summary>
	/// Messages are ignored unless the current gantry behaviour is RosListeningBehaviour.
	/// </summary>
	private void HandleMessage(string topic, string message)
	{
		if (m_gantry.CurrentBehaviour == null)
		{
			return;
		}
		if (m_gantry.CurrentBehaviour.GetType() != typeof(RosListeningBehaviour))
		{
			StartListening();//
							 //return;
		}
		if (m_translationTable.ContainsKey(topic))
		{
			if (m_translationTable[topic].ContainsKey(message))
			{
				m_translationTable[topic][message].Invoke(m_behaviour, null);
			}
		}
		if (message == "stop")//TODO remove after button is added
		{
			StopListening();
		}
	}

	public void ToggleListening()
	{
		if (m_gantry.CurrentBehaviour.GetType() != typeof(RosListeningBehaviour))
		{
			StartListening();
		}
		else
		{
			StopListening();
		}
	}

	private void StartListening()
	{
		Debug.Log("started listening");//
		m_gantry.SetBehaviour(m_behaviour);
	}

	private void StopListening()
	{
		Debug.Log("stopped listening");//
		if (m_gantry.CurrentBehaviour.GetType() == typeof(RosListeningBehaviour))
		{
			m_gantry.SetBehaviour(new LaneFarmingBehaviour());
		}
	}

	private void OnRosClosed(object sender, EventArgs e)
	{
		Debug.LogWarning("ROS connection closed, listening will be disabled");
		m_actionQueuer.QueueAction(() => StopListening());
	}
}