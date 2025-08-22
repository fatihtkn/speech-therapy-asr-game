using System.Collections;
using UnityEngine;

public class RaftPuzzle : PuzzleBase
{

    public Raft raft;
    public SphericalBuoyancy envRaft;
    private bool firstTime = true;
    public override void OnPuzzleSolved(out bool haveAllPhasesCompleted)
    {
        base.OnPuzzleSolved(out haveAllPhasesCompleted);
        raft.StartRaftGeneration();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (firstTime)
            {
                firstTime = false;
                StartCoroutine(LookAtRaft());
                
            }
            
        }
    }

    private IEnumerator LookAtRaft()
    {
        envRaft.canMove = true;
        PlayerManager.Instance.PlayVoiceLine(1);
        yield return null;
        //CameraController.Instance.SwitchToCamera(CameraType.WatersphereCam);
        //PlayerMovementController.Instance.canMove = false;
        //yield return new WaitForSeconds(2.5f);
        //PlayerMovementController.Instance.canMove = true;
        //CameraController.Instance.SwitchToCamera(CameraType.PlayerCam);
    }
}
