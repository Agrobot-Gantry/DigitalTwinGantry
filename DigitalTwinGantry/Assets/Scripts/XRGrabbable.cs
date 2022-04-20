using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabbable : XRGrabInteractable
{
    [Header("Custom Settings")]
    [SerializeField] private bool m_snapToCenterWhenGrabbed = false;

    private Vector3 m_interactorPosition = Vector3.zero;
    private Quaternion m_interactorRotation = Quaternion.identity;
    
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        
        if (m_snapToCenterWhenGrabbed)
        {
            return;
        }

        m_interactorPosition = args.interactor.attachTransform.localPosition;
        m_interactorRotation = args.interactor.attachTransform.localRotation;

        bool hasAttach = attachTransform != null;
        args.interactor.attachTransform.position = hasAttach ? attachTransform.position : transform.position;
        args.interactor.attachTransform.rotation = hasAttach ? attachTransform.rotation : transform.rotation;
    }
    
    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        
        if (m_snapToCenterWhenGrabbed)
        {
            return;
        }

        m_interactorPosition = Vector3.zero;
        m_interactorRotation = Quaternion.identity;
    }
}
