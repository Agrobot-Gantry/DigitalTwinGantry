using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class RosCommandListener : MonoBehaviour
{
	private AgrobotGantry m_gantry;
	private RosListeningBehaviour m_behaviour;
	private Dictionary<string, Dictionary<string, MethodInfo>> m_translationTable; //<topic, <message, command>>

	//classes for reading the json
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
		TextAsset textAsset = Resources.Load<TextAsset>("RosTranslationTable");
		TranslationEntry[] entries = JsonUtility.FromJson<Translations>(textAsset.text).entries;

		//fill dictionary
		foreach (TranslationEntry entry in entries)
		{
			if (!m_translationTable.ContainsKey(entry.topic))
			{
				m_translationTable.Add(entry.topic, new Dictionary<string, MethodInfo>());
			}
			m_translationTable[entry.topic].Add(entry.message, m_behaviour.GetType().GetMethod(entry.command));
		}

		//TODO check every methodinfo

		//create a subscriber for each unique topic
		foreach (string topic in m_translationTable.Keys)
		{
			GantryCommandSubscriber subscriber = gameObject.AddComponent<GantryCommandSubscriber>();
			subscriber.Topic = topic;
			subscriber.Callback = OnCommandReceived;
		}
	}

	public void OnCommandReceived(string topic, string command)
	{
		if (m_gantry.CurrentBehaviour == null)
		{
			return;
		}
		if (m_gantry.CurrentBehaviour.GetType() != typeof(RosListeningBehaviour))
		{
			m_gantry.SetBehaviour(m_behaviour);
		}

		m_translationTable[topic][command].Invoke(m_behaviour, null);
	}
}