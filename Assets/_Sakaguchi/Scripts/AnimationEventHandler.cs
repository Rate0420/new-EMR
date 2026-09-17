using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private ParticleSystem ps2;

    public void ShowObject()
    {
        target.SetActive(true);
    }

    public void HideObject()
    {
        target.SetActive(false);
    }

    public void SetObjectActive(bool active)
    {
        target.SetActive(active);
    }

    public void StartParticle()
    { 
        ps.Play();
    }

    public void StartParticle2()
    { 
        ps2.Play();
    }
}