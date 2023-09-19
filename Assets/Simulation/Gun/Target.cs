using UnityEngine;

public class Target : MonoBehaviour
{
    public Transform bulletHole;
    private bool bulletHoleIsVisible;

    [Header("Sound Effects")]
    public SoundEffect targetCollision;
    private AudioSource audioSource;

    private bool soundIsOn = true;

    private void Awake()
    {
        TryGetComponent(out audioSource);

        if (bulletHole) bulletHole.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Bullet.OnHitTarget += HandleBulletHitTarget;
    }

    private void OnDisable()
    {
        Bullet.OnOutOfBounds -= HandleBulletHitTarget;
    }

    public void HandleBulletHitTarget(Bullet bullet)
    {
        // Sound effect
        if (soundIsOn) PlayTargetCollisionAudio();

        if (bulletHole)
        {
            if (!bulletHoleIsVisible)
            {
                bulletHole.gameObject.SetActive(true);
                bulletHoleIsVisible = true;
            }

            Vector3 hitPosition = transform.InverseTransformPoint(bullet.Position);
            Vector3 hitVelocity = transform.InverseTransformVector(bullet.Velocity);

            float slope = hitVelocity.x / hitVelocity.z;
            if (hitPosition.z < 0)
            {
                hitPosition.x += slope * hitPosition.z;
            }
            else
            {
                hitPosition.x -= slope * hitPosition.z;
            }

            hitPosition.z = -0.6f;
            bulletHole.localPosition = hitPosition;
        }
    }

    private void PlayTargetCollisionAudio()
    {
        if (audioSource && targetCollision) targetCollision.Play(audioSource);
    }

    public void ToggleSound(bool isOn)
    {
        soundIsOn = isOn;
    }
}
