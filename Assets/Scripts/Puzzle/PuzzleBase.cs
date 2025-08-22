using System;
using System.Collections.Generic;
using UnityEngine;

public  class PuzzleBase : MonoBehaviour
{

    public List<WordContainerSO> wordSetContainer = new();
    public bool IsSolved { get; protected set; }
    protected int currentPuzzlePhaseIndex=0;

    public virtual void OnPuzzleSolved(out bool haveAllPhasesCompleted)
    {
        
        GameManager.Instance.ToggleRecording();
        

        if (currentPuzzlePhaseIndex == wordSetContainer.Count-1)
        {
            haveAllPhasesCompleted = true;
            currentPuzzlePhaseIndex = 0;
            return;
        }
        else
        {
            haveAllPhasesCompleted = false;
            currentPuzzlePhaseIndex++;
            return;
        }

       



    }


   
    public List<String> GetCurrentWordSet()
    {

       return wordSetContainer[currentPuzzlePhaseIndex].words;
    }
}
