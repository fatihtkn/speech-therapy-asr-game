using System.Collections.Generic;
using UnityEngine;
using Vosk;
public class GameManager : MonoBehaviour
{
   
    public static GameManager Instance { get; private set; }
    public VoskSpeechToText VoskSpeechToText;
    public VoskResultText VoskResultText;
    public int currentPuzzleIndex = 0;


    [SerializeField]private bool startVosk;


    [SerializeField] private Transform swing;

    [SerializeField] private List<GameObject> starters;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerMovementController.Instance.transform.parent= swing;



     

        if(startVosk) VoskSpeechToText.StartVoskStt(startMicrophone: true);



    }

   

    public void ToggleRecording()
    {
        
        VoskSpeechToText.ToggleRecording();
   
    }
    public void StartGame()
    {
       
        PlayerMovementController.Instance.SetAnimation("IsGrounded", false); 
        PlayerMovementController.Instance.transform.parent = null;
        StartCoroutine(PlayerMovementController.Instance.PushOverTime());
        starters.ForEach(starter => starter.SetActive(false));

    }

    public void RestartGame()
    {
        SceneController.Instance.LoadScene("MainScene");
    }
}
