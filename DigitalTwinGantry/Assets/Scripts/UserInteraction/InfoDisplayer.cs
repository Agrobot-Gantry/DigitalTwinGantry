using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will write info about the application on selected text fields
/// </summary>
public class InfoDisplayer : MonoBehaviour
{
    [SerializeField] private CropField m_cropField;
    [SerializeField] private AgrobotGantry m_gantry;
    [SerializeField] private Text m_monthText;
    [SerializeField] private Text m_actionText;

    private void Start() 
    {
        StartCoroutine(OnUpdate());
    }

    private IEnumerator OnUpdate() 
    {
        while (true) 
        {
            m_monthText.text = "Month: " + (m_cropField.CurrentMonth + 1);

            string actionText = "Actions: ";
            if (m_gantry.CurrentBehaviour != null)
            {
                foreach (AgrobotAction action in m_gantry.CurrentBehaviour.OnGoingActions)
                {
                    actionText += action.Flags.ToString() + ", ";
                }

                m_actionText.text = actionText;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
