using System.Collections;
using UnityEngine;

public class BridgePuzzle : PuzzleBase
{


    private bool firstTime=true;
    private void Start()
    {
        firstTime = true;
    }
    public override void OnPuzzleSolved(out bool haveAllPhasesCompleted)
    {
        base.OnPuzzleSolved(out haveAllPhasesCompleted);
        BridgeGeneration.Instance.GenerateBridge(0);
        IsSolved = true;
        Debug.Log("Bridge puzzle solved!");
        
    }

    private void OnTriggerEnter(Collider other)
    {

        
        if(other.CompareTag("Player")&firstTime)
        {
            firstTime = false;
            StartCoroutine(LookTheBridge());
        }
    }

    private IEnumerator LookTheBridge()
    {
        PlayerMovementController.Instance.SetMove(false);
        yield return new WaitForSeconds(0.2f);
        CameraController.Instance.SwitchToCamera(CameraType.BridgeCam);
        yield return new WaitForSeconds(0.5f);
        PlayerManager.Instance.PlayVoiceLine(0);
        yield return new WaitForSeconds(3.7f);
        CameraController.Instance.SwitchToCamera(CameraType.PlayerCam);
        PlayerMovementController.Instance.SetMove(true);

    }

}
