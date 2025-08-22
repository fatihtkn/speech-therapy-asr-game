using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public List<Checkpoint> checkpoints;

    public static CheckpointController Instance { get; private set; }

    public int currentCheckpointIndex;
    private void Awake()
    {
        Instance = this;
    }

    
    public Vector3 GetCheckpointPos()
    {
        Transform pos = checkpoints[currentCheckpointIndex].GetPos();

        return pos.position;
    }





}
