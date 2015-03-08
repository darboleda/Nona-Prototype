using UnityEngine;
using System.Collections;

public class Anchor : MonoBehaviour {
    [System.Serializable]
    public struct TransformInfo
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
    }


    public Transform root;

    public delegate void TransformUpdatedHandler(TransformInfo relativeTransform);
    public event TransformUpdatedHandler TransformUpdated;

    private Transform cachedTransform;
    private Transform testTransform;

    private TransformInfo currentTransform;
    public TransformInfo CurrentTransform
    {
        get { return this.currentTransform; }
    }

    public void OnEnable()
    {
        UpdateTransform(true);
    }

    public void FixedUpdate()
    {
        UpdateTransform();
    }

    public void UpdateTransform(bool force = false)
    {
        cachedTransform = cachedTransform ?? transform;

        TransformInfo transformInfo = new TransformInfo();
        if(root == null || root.hasChanged || force)
        {
            if(cachedTransform.hasChanged || force)
            {
                Transform currentParent = cachedTransform.parent;
                cachedTransform.parent = root;

                transformInfo.position = cachedTransform.localPosition;
                transformInfo.scale = cachedTransform.localScale;
                transformInfo.rotation = cachedTransform.localRotation;

                cachedTransform.parent = currentParent;

                if (!currentTransform.Equals(transformInfo) && TransformUpdated != null)
                {
                    currentTransform = transformInfo;
                    TransformUpdated(transformInfo);
                }
            }
        }
    }
}
