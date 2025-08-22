
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raft : MonoBehaviour
{
    [SerializeField] private List<RaftPiece> raftParts;
    [SerializeField] private float partGenerationInterval=0.3f;
    [SerializeField] private GameObject sphere;
    public SphericalBuoyancy sphericalBuoyancy;
    [SerializeField] private Material ropeMat;
    private bool isPassengerOnRaft ;
    float radius;
    public static event Action OnDestinationReached;
    private void Start()
    {
        ropeMat.SetFloat("_Grow", 0f);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(StartGeneration());
        }

        if (isPassengerOnRaft)
        {
            float angle = sphericalBuoyancy.GetAngleBySphere();
            if (angle<45f)
            {
                isPassengerOnRaft = false;
                //PlayerMovementController.Instance.
                OnDestinationReached?.Invoke();
                print("Destination Reached! ");
            }
        }
    }

    public void StartRaftGeneration()
    {
        if (raftParts.Count == 0)
        {
            Debug.LogError("No raft parts assigned!");
            return;
        }
        StartCoroutine(StartGeneration());
    }

    private IEnumerator StartGeneration()
    {
        
        foreach (var part in raftParts)
        {
            StartCoroutine(part.Move(0.5f));
            yield return new WaitForSeconds(partGenerationInterval);  
        }

        yield return FadeInRopes();
        yield return PlayerMovementController.Instance.SetSail(transform, () => { isPassengerOnRaft = true; print("Started to sail"); });
        yield return sphericalBuoyancy.SetInitialPosAndRot();
    }

    private IEnumerator FadeInRopes()
    {
        float duration = 1f;
        float timer = 0f;
        while(timer < duration)
        {
            float t = Mathf.Clamp01(timer/duration);
            ropeMat.SetFloat("_Grow", t);
            timer += Time.deltaTime;
            yield return null;
        }
       
    }
    

   


}
