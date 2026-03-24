// Depricated 

using UnityEngine;
using System.Collections;   

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip fireClip;
    [SerializeField] AudioClip reloadClip;
    [SerializeField] AudioClip inspectClip;
    AudioSource audioSource;

    bool inputEnabled = true;   

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();  
        if (!fireClip || !reloadClip || !inspectClip)
        {
            Debug.LogError("AudioManager: Missing audio clips!");
        }
        if (audioSource == null)
        {
            Debug.LogError("AudioManager: No AudioSource attached!");
        }

    }

    private void Update()
    {

        if (!inputEnabled)
            return; 
        if (Input.GetMouseButtonDown(0))
            Fire();

        if (Input.GetKeyDown(KeyCode.R))
            Reload();

        if (Input.GetKeyDown(KeyCode.T))
            Inspect();
    }

    void Fire()
    {
        audioSource.PlayOneShot(fireClip);  
        Debug.Log($"Fire | audioSource: {audioSource} | enabled: {audioSource.enabled} | volume: {audioSource.volume}");
    }

    private IEnumerator AnimationFinished(float duration)
    {
        yield return new WaitForSeconds(duration);
        inputEnabled = true;
    }


    void Reload()
    {
     audioSource.PlayOneShot(reloadClip);
        StartCoroutine(AnimationFinished(reloadClip.length)); 
    }   

    void Inspect()
    {
        audioSource.PlayOneShot(inspectClip);
        StartCoroutine(AnimationFinished(inspectClip.length)); 
    }   
}
