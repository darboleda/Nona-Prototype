using UnityEngine;

public class Shootable : MonoBehaviour
{
    public GameObject Effect;
    public void TakeShot(RaycastHit hitinfo, NonaController source)
    {
        GameObject effect = GameObject.Instantiate(Effect) as GameObject;
        effect.transform.position = hitinfo.point;
        effect.transform.LookAt(hitinfo.point + hitinfo.normal);
    }
}
