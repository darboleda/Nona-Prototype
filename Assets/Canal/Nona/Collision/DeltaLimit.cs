using UnityEngine;
using System.Collections;

public abstract class DeltaLimit<T> : MonoBehaviour {

    public struct ApplicationResult
    {
        public T updatedInfo;
        public Vector2 updatedDelta;
    }

    public abstract ApplicationResult Apply(Vector2 desiredDelta, T previousInfo, T currentInfo);
}
