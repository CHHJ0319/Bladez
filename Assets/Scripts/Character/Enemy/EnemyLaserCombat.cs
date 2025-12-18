using UnityEngine;
using System.Collections;

public class EnemyLaserCombat : MonoBehaviour
{
    [Header("Components")]
    public GameObject laserModel;      
    //public ParticleSystem sparksVFX;   

    [Header("Settings")]
    public float growthSpeed = 5;    
    public float maxRange = 1f;       
    public float duration = 0.05f;

    private float fireDelay;
    private float fireRate = 2f;
    private bool isFireReady = false;

    void Start()
    {
        laserModel.SetActive(false);
    }

    private void Update()
    {
        fireDelay += Time.deltaTime;
        isFireReady = fireDelay > fireRate;
    }

    public void Fire()
    {
        if (isFireReady) StartCoroutine(ShootSequence());
    }

    IEnumerator ShootSequence()
    {
        laserModel.SetActive(true);

        float currentLength = 0f;
        RaycastHit hit;

        while (currentLength < maxRange)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxRange))
            {
                if (currentLength >= hit.distance)
                {
                    currentLength = hit.distance;
                    // 충돌 지점에 스파크 파티클 재생
                    //sparksVFX.transform.position = hit.point;
                    //if (!sparksVFX.isPlaying) sparksVFX.Play();
                    break;
                }
            }

            currentLength += growthSpeed * Time.deltaTime;
            UpdateLaserScale(currentLength);
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        laserModel.SetActive(false);
        //sparksVFX.Stop();
        fireDelay = 0f;
    }

    void UpdateLaserScale(float length)
    {
        laserModel.transform.localScale = new Vector3(0.1f, length * 0.5f, 0.1f);
        laserModel.transform.localPosition = new Vector3(0, 0, length * 0.5f);
    }
}