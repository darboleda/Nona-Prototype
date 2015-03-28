using UnityEngine;
using System.Collections;

public abstract class ActionBehavior : MonoBehaviour {

    public virtual void Tick() { }
    public virtual void FixedTick() { }
}
