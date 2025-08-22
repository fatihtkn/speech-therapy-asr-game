using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class BridgeStone : MonoBehaviour
{
    public AnimationCurve curve;
    public bool isVisible;
    MeshRenderer meshRenderer;
    private void Start()
    {
        meshRenderer= GetComponentInChildren<MeshRenderer>();

        if (isVisible) return;


        meshRenderer.enabled= false;
    }


    public async Task MoveBrick(float duration,Vector3 startOffset)
    {
        meshRenderer.enabled = true;
        Vector3 targetPos = transform.localPosition;
        transform.localPosition += startOffset;
        Vector3 startPoint = transform.localPosition;
        
        float timer=0f;
        while (Vector3.Distance(transform.localPosition,targetPos)>=0.01f) 
        {
            float t = Mathf.Clamp01(timer / duration);
            transform.localPosition = Vector3.Lerp(startPoint, targetPos,curve.Evaluate(t));



            timer += Time.deltaTime;
            await Task.Yield();

        }
        transform.localPosition = targetPos;
        
    }

    public IEnumerator LayTheStones(float duration, Vector3 startOffset)
    {
        if(isVisible)
        {
            yield break;
        }

        meshRenderer.enabled = true;
        Vector3 targetPos = transform.localPosition;
        transform.localPosition += startOffset;
        Vector3 startPoint = transform.localPosition;

        float timer = 0f;
        while (Vector3.Distance(transform.localPosition, targetPos) >= 0.01f)
        {
            float t = Mathf.Clamp01(timer / duration);
            transform.localPosition = Vector3.Lerp(startPoint, targetPos, curve.Evaluate(t));



            timer += Time.deltaTime;
            yield return null;

        }
        transform.localPosition = targetPos;

    }

}
