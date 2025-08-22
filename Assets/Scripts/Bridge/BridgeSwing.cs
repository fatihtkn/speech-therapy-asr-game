using System.Collections.Generic;
using UnityEngine;

public class BridgeSwing : MonoBehaviour
{
    [SerializeField] private Cloth leftRope;
    [SerializeField] private SkinnedMeshRenderer ropeMesh;   
    [SerializeField] private GameObject plank;
    [SerializeField] private Vector3 offset;

    public int vertexIndex = 0;
    private void Start()
    {
        
        
       print(leftRope.coefficients.Length);
    }
    private void Update()
    {
        if (vertexIndex < leftRope.vertices.Length)
        {
            plank.transform.position = leftRope.transform.TransformPoint(leftRope.vertices[vertexIndex])+offset;

        }
    }

}
