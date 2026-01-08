using UnityEngine;

public class WeaponSwingDetector : MonoBehaviour
{
    public float swingThreshold = 3f;
    public ParticleSystem swingVFX;
    public AudioSource swingSFX;

    Vector3 lastPos;
    bool isSwinging;

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        float speed = (transform.position - lastPos).magnitude / Time.deltaTime;

        if (speed > swingThreshold && !isSwinging)
        {
            isSwinging = true;
            PlaySwingEffects();
        }

        if (speed < swingThreshold * 0.5f)
        {
            isSwinging = false;
        }

        lastPos = transform.position;
    }

    void PlaySwingEffects()
    {
        if (swingVFX) swingVFX.Play();
        if (swingSFX) swingSFX.Play();
    }
}
