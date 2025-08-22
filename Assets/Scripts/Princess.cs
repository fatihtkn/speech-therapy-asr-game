using System.Collections;
using UnityEngine;

public class Princess : MonoBehaviour
{
    public static Princess Instance { get; private set; }
    public AudioSource audioSource; // karakterin ses çýkýþ kaynaðý
    public AudioClip[] voiceLines;  // karakterin replik sesleri
    private bool firstTime;
    private void Awake()
    {
        Instance = this;
        firstTime = true;
    }


    public void PlayVoiceLine(int index)
    {
        if (index < 0 || index >= voiceLines.Length)
        {
            Debug.LogWarning("Geçersiz ses indeksi!");
            return;
        }

        audioSource.clip = voiceLines[index];
        audioSource.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&firstTime)
        {
            firstTime = false;
           StartCoroutine(StartSeq());
             
        }
    }
    private IEnumerator StartSeq()
    {
        PlayerMovementController.Instance.SetMove(false);
        yield return new WaitForSeconds(0.3f);
        CameraController.Instance.SwitchToCamera(CameraType.FinalCam);
        yield return new WaitForSeconds(0.6f);    
        PlayVoiceLine(0);
        
        yield return new WaitForSeconds(6f);
        CameraController.Instance.SwitchToCamera(CameraType.PlayerCam);
        PlayerMovementController.Instance.SetMove(true);
    }
}
