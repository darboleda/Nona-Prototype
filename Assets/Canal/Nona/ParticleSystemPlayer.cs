using UnityEngine;


public class ParticleSystemPlayer : MonoBehaviour
{
    [System.Serializable]
    public struct ParticlePair
    {
        public string Key;
        public ParticleSystem Target;
    }

    public ParticlePair[] Targets;

    public void Play(string key)
    {
        foreach (var target in Targets)
        {
            if (target.Key == key)
            {
                target.Target.gameObject.SetActive(false);
                target.Target.gameObject.SetActive(true);
                target.Target.Play();
            }
        }
    }
}
