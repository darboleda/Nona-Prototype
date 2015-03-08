using UnityEngine;
using System.Collections;

public class AnchorTarget : MonoBehaviour {
    public Anchor anchor;
    public Transform root;

    private Transform cachedTransform;

    public void OnEnable()
    {
        this.UpdatePosition(anchor.CurrentTransform);
        anchor.TransformUpdated += UpdatePosition;
    }

    public void OnDisable()
    {
        anchor.TransformUpdated -= UpdatePosition;
    }

    public void UpdatePosition(Anchor.TransformInfo relativeTransform)
    {
        cachedTransform = cachedTransform ?? transform;

        Transform currentParent = cachedTransform.parent;
        
        cachedTransform.localPosition = relativeTransform.position;
        cachedTransform.localScale = relativeTransform.scale;
        cachedTransform.localRotation = relativeTransform.rotation;
        
        cachedTransform.parent = currentParent;
    }
}
