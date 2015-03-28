using UnityEngine;

public class Shootable : MonoBehaviour
{
    public class WallHitFactory : TyplessGameObjectFactory
    {

    }

    [Zenject.Inject]
    public WallHitFactory wallHit;

    public void TakeShot(RaycastHit hitinfo, TwinGunFire source)
    {
        GameObject effect = wallHit.Create();
        effect.transform.position = hitinfo.point;
        effect.transform.LookAt(hitinfo.point + hitinfo.normal);
    }
}
