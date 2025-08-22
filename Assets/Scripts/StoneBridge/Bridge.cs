using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Bridge : MonoBehaviour
{
	private List<BridgeStone> stones;
    [SerializeField] private Vector3 startOffset;
    [SerializeField] private Collider col;
   

    private void Awake()
    {
        
        stones = GetComponentsInChildren<BridgeStone>().ToList();
    }


   

    public IEnumerator StartLayingTheStones()
    {
        col.enabled = true;
        foreach (var stone in stones)
        {
           
            StartCoroutine(stone.LayTheStones(duration: 1.5f, startOffset));
            yield return new WaitForSeconds(0.3f);
        }

    }

}
