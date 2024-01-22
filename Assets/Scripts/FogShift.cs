using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogShift : MonoBehaviour
{
    public float targetDensity;
    private float initDensity;
    private Collider triggerCollider;

    void Start()
    {
        triggerCollider = GetComponent<Collider>();
        initDensity = RenderSettings.fogDensity;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag != null && other.tag == "Player")
        {
            RenderSettings.fogDensity = targetDensity;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != null && other.tag == "Player")
        {
            RenderSettings.fogDensity = initDensity;
        }
    }

    private IEnumerator FadeFog(float targetDensity)
    {
        Debug.Log("Started");
        for (float x=0; x < 100; x++)
        {
            float lerpScale = x / 100;
             Mathf.Lerp(RenderSettings.fogDensity, targetDensity, lerpScale);
            Debug.Log("A");
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("Finished");
        yield return null;
    }
}
