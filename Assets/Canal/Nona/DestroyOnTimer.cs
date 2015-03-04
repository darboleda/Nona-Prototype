using UnityEngine;
using System.Collections;

public class DestroyOnTimer : MonoBehaviour
{
    public float TimeToDestroy = 1.0f;

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(TimeToDestroy);
        GameObject.Destroy(gameObject);
    }
}
