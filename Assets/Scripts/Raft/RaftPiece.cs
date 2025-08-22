using System.Collections;
using UnityEngine;

public class RaftPiece : MonoBehaviour
{
    public AnimationCurve curve;







    public IEnumerator Move(float duration,float speed=1)
    {
        Vector3 startPos = transform.localPosition;
        Vector3 startRot= transform.localEulerAngles;

        Vector3 targetRot = new(-89.98f, 0, 0);
        float timer= 0f;
        while (timer<duration)
        {
            float t= Mathf.Clamp01(  speed * timer / duration); 
            transform.localPosition= Vector3.Lerp(startPos, Vector3.zero, curve.Evaluate(t));
            transform.localEulerAngles = Vector3.Lerp(startRot, targetRot, t);
            timer += Time.deltaTime;
            yield return null; 
        }
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = targetRot;
    }
}
