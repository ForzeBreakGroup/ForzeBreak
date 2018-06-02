using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleMovement : PowerUpMovement
{
    private Rigidbody rb;
    FMOD.Studio.EventInstance attractingSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 12, ForceMode.Impulse);
    }

    public void EnableBlackHole()
    {
        StartCoroutine(DisableBlackHole());
    }

    IEnumerator DisableBlackHole()
    {
        PowerupSound ps = GetComponent<PowerupSound>();
        attractingSound = FMODUnity.RuntimeManager.CreateInstance(ps.soundList[0].Soundref);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(attractingSound, transform, rb);
        attractingSound.start();
        yield return new WaitForSeconds(5);
        attractingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        attractingSound.release();
        ps.PlaySound(1);
        ((BlackHoleCollision)PowerUpCollision).DisableBlackHoleEffect();
        StartCoroutine(DestroyBlackHole());
    }

    IEnumerator DestroyBlackHole()
    {
        yield return new WaitForSeconds(5);
        DestroyPowerUpProjectile();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        attractingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        attractingSound.release();
    }
}
