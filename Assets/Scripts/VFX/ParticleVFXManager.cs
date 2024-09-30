using UnityEngine;

public class ParticleVFXManager : MonoBehaviour
{
    [SerializeField] private ParticleManager firePS;

    [SerializeField] private Animator iceAnimator;
    [SerializeField] private ParticleManager icePS;

    [SerializeField] private ParticleManager poisonPS;

    [SerializeField] private ParticleManager staticPS;
    [SerializeField] private ParticleManager stunnedPS;

    [SerializeField] private ParticleManager bleedPS;
    [SerializeField] private ParticleManager bloodLossPS;

    [SerializeField] private ParticleManager gasolinePS;

    public void OnBurning(int count)
    {
        firePS.SetAllEmissionRate(count + 10);
        firePS.PlayAll();
    }
    public void StopBurning()
    {
        firePS.StopAll();
    }

    public void OnFrozen()
    {
        iceAnimator.SetTrigger("activate");
        icePS.PlayAll();
    }
    public void StopFrozen()
    {
        iceAnimator.SetTrigger("melt");
        icePS.StopAll();
    }

    public void OnPoison()
    {
        poisonPS.PlayAll();
    }
    public void StopPoison()
    {
        poisonPS.StopAll();
    }

    public void OnStatic()
    {
        staticPS.PlayAll();
    }
    public void StopStatic() 
    {
        staticPS.StopAll();
    }

    public void OnStunned()
    {
        stunnedPS.PlayAll();
    }

    public void OnBleeding()
    {
        bleedPS.PlayAll();
    }
    public void StopBleeding()
    {
        bleedPS.StopAll();
    }

    public void OnBloodLoss()
    {
        bloodLossPS.PlayAll();
    }

    public void GasolineBurst()
    {
        gasolinePS.PlayAll();
    }

    public void StopEverything()
    {
        StopFrozen();
        StopBurning();
        StopPoison();
        StopStatic();
        StopBleeding();
    }
}