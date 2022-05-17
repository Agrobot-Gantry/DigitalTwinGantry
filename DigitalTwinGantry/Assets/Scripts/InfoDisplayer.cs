using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplayer : MonoBehaviour
{
    [SerializeField] private CropField m_cropField;
    [SerializeField] private AgrobotGantry m_gantry;
    [SerializeField] private Text m_monthText;
    [SerializeField] private Text m_actionText;

    private void Update() {
        m_monthText.text = "Month: " + m_cropField.CurrentMonth;

        string actionText = "Actions: ";
        foreach (AgrobotAction action in m_gantry.CurrentBehaviour.OnGoingActions)
        {   
            actionText += action.GetType().Name + ", ";
        }

        m_actionText.text = actionText;
    }
}
