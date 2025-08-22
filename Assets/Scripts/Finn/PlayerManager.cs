using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get;private set; }
    public AudioSource audioSource; // karakterin ses çýkýþ kaynaðý
    public AudioClip[] voiceLines;  // karakterin replik sesleri
    private void Awake()
    {
        Instance = this;
    }


    public void PlayVoiceLine(int index)
    {
        if (index < 0 || index >= voiceLines.Length)
        {
            Debug.LogWarning("voice index out of range bra");
            return;
        }

        audioSource.clip = voiceLines[index];
        audioSource.Play();
    }

    


}
