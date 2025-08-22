using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    
    public int checkpointIndex;

    public Transform checkpoint;

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.CompareTag("Player"))
        {
            CheckpointController.Instance.currentCheckpointIndex = checkpointIndex;
            
        }
    }
    public Transform GetPos()
    {
       
        return checkpoint;
    }
}
