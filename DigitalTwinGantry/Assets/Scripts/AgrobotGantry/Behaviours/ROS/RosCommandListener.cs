using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Listens for ROS messages as specified in the ROS translation JSON file.
/// When a message is received this will change the gantry behaviour to RosListeningBehaviour if it is not already.
/// Received messages are used to call methods on the RosListeningBehaviour.
/// </summary>
class RosCommandListener : MonoBehaviour
{
	private static readonly string TRANSLATION_FILE_NAME = "RosTranslationTable";
	//TODO reset gantry on disconnect/timeout

	private AgrobotGantry m_gantry;
	private RosListeningBehaviour m_behaviour;
	private Dictionary<string, Dictionary<string, MethodInfo>> m_translationTable; //<topic, <message, command>>

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

	public void OnMessageReceived(string topic, string message)
	{
		if (m_gantry.CurrentBehaviour == null)
		{
			return;
		}
		if (m_gantry.CurrentBehaviour.GetType() != typeof(RosListeningBehaviour))
		{
			m_gantry.SetBehaviour(m_behaviour);
		}

		m_translationTable[topic][message].Invoke(m_behaviour, null);
	}
}