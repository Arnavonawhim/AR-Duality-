using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float launchForce = 25f;
    [SerializeField] private AudioSource bounceSound;
    [SerializeField] private ParticleSystem bounceEffect;

    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ApplyJumpPadForce(launchForce);
            
            if (bounceSound != null) bounceSound.Play();
            if (bounceEffect != null) bounceEffect.Play();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ApplyJumpPadForce(launchForce);
            
            if (bounceSound != null) bounceSound.Play();
            if (bounceEffect != null) bounceEffect.Play();
        }
    }
}
