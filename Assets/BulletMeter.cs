using UnityEngine;
using System.Collections;

public class BulletMeter : MonoBehaviour {

    public UnityEngine.UI.Slider Slider;
    public UnityEngine.UI.Text Label;
    public Animator Animator;

    public void SetBulletCount(int bulletCount, int bulletMax, bool updateLabel = true)
    {
        Slider.minValue = 0;
        Slider.maxValue = bulletMax;
        Slider.value = bulletCount;

        if (updateLabel)
        {
            Label.text = bulletCount.ToString();
        }
    }

    public void StartReload(int bulletMax, float reloadTime)
    {
        this.StartCoroutine(DoReload(bulletMax, reloadTime));
    }

    private IEnumerator DoReload(int bulletMax, float reloadTime)
    {
        float currentTime = reloadTime;
        Label.text = "0";
        Animator.SetBool("Reloading", true);
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            SetBulletCount((int)Mathf.Lerp(0, bulletMax, (reloadTime - currentTime) / reloadTime), bulletMax, false);
            yield return null;
        }
        Animator.SetBool("Reloading", false);
        SetBulletCount(bulletMax, bulletMax);
        yield break;
    }
}
