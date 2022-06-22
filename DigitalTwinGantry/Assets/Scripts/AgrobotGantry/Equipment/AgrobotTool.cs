using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The reach of the tool is defined by the collider of the ToolReach object.
/// The tool itself (visuals and animation) is the tool parameter in the editor.
/// The flag parameter defines which type of interactable this tool can interact with.
/// Each tool keeps a list of all interactables (with the right flag) that it can reach.
/// Actions make use of tools for animation.
/// </summary>
public class AgrobotTool : MonoBehaviour
{
    [SerializeField]
    private ToolReach m_reach;
    [SerializeField]
    private GameObject m_tool;
    [SerializeField]
    private InteractableFlag m_flag;
    [SerializeField]
    private GameObject m_toolEffect;

    public ToolReach Reach { set => m_reach = value; }

    /// <summary>
    /// A list of all interactables that are in reach of this tool and have an appropriate flag.
    /// </summary>
    public List<AgrobotInteractable> Reachables => m_reachables;

    private List<AgrobotInteractable> m_reachables;

    public bool GoingTooFast { get { return m_goingTooFast; } }
    private bool m_goingTooFast;
    public bool Busy = false;

    private AgrobotArm m_arm;
    private AgrobotToolEffect m_effect;

    private void Start()
    {
        m_reachables = new List<AgrobotInteractable>();
        m_reach.ConnectToTool(this);

        //check if the tool has exactly one flag set
        Assert.IsTrue(AgrobotInteractable.FlagCount(m_flag) == 1);

        m_arm = GetComponentInChildren<AgrobotArm>();

        if (m_toolEffect != null)
        {
            AgrobotArmSegment segment = m_arm.LastSegment;
            GameObject toolEffect = Instantiate(m_toolEffect, segment.transform);
            toolEffect.transform.position = segment.EndPos;
            m_effect = toolEffect.GetComponentInChildren<AgrobotToolEffect>();
        }
    }

    /// <summary>
    /// Does the appropriate animation to interact with an interactable
    /// </summary>
    /// <param name="interactable">The interactable that needs to be interacted with</param>
    /// <param name="action">The action that needs to be performed on the interactable</param>
    /// <param name="speed">The speed at which the animation should play (delta time is already accounted for)</param>
    /// <returns>nothing, this method should be used inside a coroutine</returns>
    public IEnumerator Interact(AgrobotInteractable interactable, InteractableFlag action, float speed)
    {
        GameObject cropMesh = null;
        if (action == InteractableFlag.SOW)
        {
            cropMesh = Instantiate(interactable.InteractableObject.GetComponent<Crop>().PostSowingModel,
                m_arm.LastSegment.transform);
            cropMesh.transform.position = m_arm.LastSegment.EndPos;
            cropMesh.SetActive(true);

            // Temporary timed destroy, this has to be replaced by the commented code underneath
            Destroy(cropMesh, (speed / 2) * (1 / TimeChanger.TimeScale));
        }

        yield return m_arm.ReachForPointSmooth(interactable.transform, 0.1f, speed);

        if (m_effect != null)
        {
            m_effect.EffectStart();
        }

        yield return new WaitForSeconds(0.2f);
		m_arm.NeutralPosition(speed);

        // Correct way to destroy the cropMesh. This has been disabled because this Coroutine
        // sometimes gets stopped so the cropMesh never gets destroyed. Fix: This coroutine should not be stopped.
        // if (cropMesh != null)
        // {
        //     Destroy(cropMesh);
        // }

        if (action == InteractableFlag.UPROOT || action == InteractableFlag.HARVEST)
        {
            Quaternion rotation = interactable.InteractableObject.GetComponent<Crop>().PostSowingModel.transform.rotation;
            cropMesh = Instantiate(interactable.InteractableObject.GetComponent<Crop>().CurrentModel,
                m_arm.LastSegment.transform);
            cropMesh.transform.position = m_arm.LastSegment.EndPos;
            cropMesh.transform.rotation = rotation;
            cropMesh.SetActive(true);

            Destroy(cropMesh, 0.6f * (1 / TimeChanger.TimeScale));
        }
    }

    public void ActionCancelled()
    {
        busy = false;
        m_arm.Busy = false;
    }

    /// <summary>
    /// Call this function to reset the tool.
    /// It is recommended to use this method when regenerating a new cropfield.
    /// </summary>
    public void Reset()
    {
        if (m_reachables != null)
        {
            m_reachables.Clear();
        }
        this.Busy = false;

        if (m_arm != null)
        {
            m_arm.Busy = false;
        }
        
    }

    private void Update()
    {
        if(m_reachables.Count == 0)
        {
            m_goingTooFast = false;
        }
    }

    public void OnReachEnter(AgrobotInteractable interactable)
    {
        if (interactable.HasFlag(m_flag))
        {
            m_reachables.Add(interactable);
        }
    }

    public void OnReachExit(AgrobotInteractable interactable)
    {
        if (m_reachables.Contains(interactable))
        {
            m_goingTooFast = true;
        }
        
       // m_reachables.Remove(interactable);
    }

    /// <summary>
    /// Removes an interactable from the list of reachables if it does not have the appropriate flag.
    /// This is needed because interactable flags will be removed while the interactable is still in reach,
    /// and the tool otherwise only checks for flags when the interactable enters/exits its reach.
    /// </summary>
    /// <param name="interactable">the interactable that was modified</param>
    public void InteractableModified(AgrobotInteractable interactable)
    {
        if (interactable == null)
        {
            m_reachables.Remove(interactable);
        }
        else if (!interactable.HasFlag(m_flag))
        {
            m_reachables.Remove(interactable);
        }
        for (int i = 0; i < m_reachables.Count; i++)
        {
            if(m_reachables[i] == null)
            {
                m_reachables.RemoveAt(i);
                i--;
            }
        }
    }

    /// <returns>the GameObject that represents the visuals and animation of the tool</returns>
    public GameObject GetToolObject()
    {
        return m_tool;
    }

    public InteractableFlag GetFlag()
    {
        return m_flag;
    }

    private void OnValidate()
    {
        if (m_toolEffect != null)
        {
            if (m_toolEffect.GetComponentInChildren<AgrobotToolEffect>() == null)
            {
                Debug.LogError("Tool effect gameobject should contain a AgrobotToolEffect component");
                m_toolEffect = null;
            }
        }
    }
}
