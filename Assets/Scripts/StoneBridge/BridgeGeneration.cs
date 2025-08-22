using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BridgeGeneration : MonoBehaviour
{

    public static BridgeGeneration Instance { get; private set; }   
    [SerializeField] private List<Bridge> bridges;



    private void Awake()
    {
        Instance = this;
    }

   

    public void GenerateBridge(int index)
    {
        StartCoroutine(bridges[index].StartLayingTheStones());
    }
    
}

