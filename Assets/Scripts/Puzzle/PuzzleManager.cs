using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public VoskSpeechToText VoskSpeechToText;
    public static PuzzleManager Instance{ get; private set; }

    [SerializeField] private List<PuzzleBase> puzzles=new();

    private int puzzleIndex;
    [SerializeField]private PuzzleBase currentPuzzle;
    
    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        VoskSpeechToText.OnTranscriptionResult += OnTranscriptionResult;
    }

  
    private void Start()
    {
        currentPuzzle = puzzles[puzzleIndex];
    }


    private void OnTranscriptionResult(string obj)
    {
        var result = new RecognitionResult(obj);

        for (int i = 0; i < result.Phrases.Length; i++)
        {

            if (currentPuzzle.GetCurrentWordSet() == null)
            {
                Debug.Log("All phases of the puzzle completed.");
                Debug.Log("List returned null");
                return;
            } 

            if (currentPuzzle.GetCurrentWordSet().Contains(result.Phrases[i].Text))
            {
                currentPuzzle.OnPuzzleSolved(out bool allPhasesCompleted);
                if (allPhasesCompleted)
                {
                    Debug.Log("All phases of the puzzle completed.");
                    UpdateCurrentPuzzle();
                    Debug.Log($"Puzzle solved with word: {result.Phrases[i].Text}");
                }
                else
                {
                    Debug.Log($"Current phase completed with word: {result.Phrases[i].Text}");
                }
                
                break;
            }
        }
    }

    private void UpdateCurrentPuzzle()
    {
        if (puzzleIndex + 1 < puzzles.Count)
        {
            puzzleIndex++;
            currentPuzzle = puzzles[puzzleIndex];
            Debug.Log($"Current puzzle updated to: {currentPuzzle.name}");
            
        }
        else
        {
            Debug.Log("No more puzzles available.");
         }
    }



}
