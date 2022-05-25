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
	private Dictionary<string, Dictionary<string, MethodInfo>> m_translationTable; //<topic, <message, action>>

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
		m_behaviour = new RosListeningBehaviour();

		m_translationTable = new Dictionary<string, Dictionary<string, MethodInfo>>();
		TextAsset textAsset = Resources.Load<TextAsset>("RosTranslationTable");
		TranslationEntry[] entries = JsonUtility.FromJson<Translations>(textAsset.text).entries;
		foreach (TranslationEntry entry in entries)
		{
			if (!m_translationTable.ContainsKey(entry.topic))
			{
				m_translationTable.Add(entry.topic, new Dictionary<string, MethodInfo>());
			}
			m_translationTable[entry.topic].Add(entry.message, m_behaviour.GetType().GetMethod(entry.command));
		}

		//TODO create subscriber for each unique dictionary topic
		OnCommandReceived("/scrimblo", "w");
	}

	public void OnCommandReceived(string topic, string command)
	{
		//if (m_gantry.CurrentBehaviour.GetType() != typeof(RosListeningBehaviour))
		{
			//m_gantry.SetBehaviour(m_behaviour);
		}

		m_translationTable[topic][command].Invoke(m_behaviour, null);
	}
}