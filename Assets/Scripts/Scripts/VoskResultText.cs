using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoskResultText : MonoBehaviour 
{
    public VoskSpeechToText VoskSpeechToText;
    public TextMeshProUGUI ResultText;
    public string targetWord;
    public bool detectTargetWord;

    public event Action OnDetectedWordCorrect;
    void Awake()
    {
        VoskSpeechToText.OnTranscriptionResult += OnTranscriptionResult;
    }

    private void OnTranscriptionResult(string obj)
    {
       //print($"RawOBJ= {obj}");
        var result = new RecognitionResult(obj);
       
        for (int i = 0; i < result.Phrases.Length; i++)
        {
            

            if (detectTargetWord)
            {
                
                if (result.Phrases[i].Text == targetWord)
                {
                    print($"Text= {result.Phrases[i].Text}, Confidence= {result.Phrases[i].Confidence}");
                    //ResultText.text = result.Phrases[i].Text;
                   
                    OnDetectedWordCorrect?.Invoke();
                }
            }
            else
            {
                if (i > 0)
                {
                    //ResultText.text += ", ";
                }
                print($"Text= {result.Phrases[i].Text}, Confidence= {result.Phrases[i].Confidence}");
                //ResultText.text += result.Phrases[i].Text;
                //ResultText.text += "\n";
            }
           
                
        }
        

     //   for (int i = 0; i < result.Phrases.Length; i++)
     //   {
     //       if (i > 0)
     //       {
     //           ResultText.text += ", ";
     //       }
     //       if(result.Phrases[i].Text != "[unk]")
     //           ResultText.text += result.Phrases[i].Text;
     //   }
    	//ResultText.text += "\n";
    }
}
