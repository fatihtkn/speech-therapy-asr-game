using System.Collections;
using UnityEngine;

public class SphericalBuoyancy : MonoBehaviour
{
    [Header("Sphere Settings")]
    public Transform sphereCenterTransform;
    public float sphereRadius = 50f;

    [Header("Wave Settings")]
    public float waveAmplitude = 1f;
    public float waveSpeed = 1f;
    public float waveScale = 1f;

    [Header("Manual Movement Settings")]
    public float moveSpeed = 5f;
    public Vector3 inputDirection = Vector3.forward; 

    private Vector3 center;
    public bool canMove;

    
    void Start()
    {
        center = sphereCenterTransform != null ? sphereCenterTransform.position : Vector3.zero;
    }

    void Update()
    {
        if (!canMove) return;

        if (sphereCenterTransform != null)
            center = sphereCenterTransform.position;

        // Normal vector from center to current position
        Vector3 normal = (transform.position - center).normalized;
        
        // Teðet yön: objenin kendi ileri yönünü kullan (her zaman güncel olacak)
        Vector3 tangentDir = Vector3.Cross(Vector3.Cross(normal, transform.forward), normal).normalized;

        // Küresel yay boyunca ilerleme
        float angle = moveSpeed * Time.deltaTime / sphereRadius;

        
       
        Vector3 axis = Vector3.Cross(normal, tangentDir).normalized;
        Quaternion rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis);
        Vector3 newPos = rotation * (transform.position - center);
        Vector3 newNormal = newPos.normalized;

        // Dalga yüksekliði
        float t = Time.time * waveSpeed;
        float noise = Mathf.PerlinNoise(newNormal.x * waveScale + t, newNormal.z * waveScale + t);
        float waveHeight = (noise * 2f - 1f) * waveAmplitude;
        float desiredDist = sphereRadius + waveHeight;

        // Yeni pozisyonu ata
        transform.position = center + newNormal * desiredDist;

        // Yeni teðet yön: bir sonraki pozisyondaki yönü hesapla
        Vector3 nextDir = Vector3.Cross(Vector3.Cross(newNormal, tangentDir), newNormal).normalized;
        if (nextDir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(nextDir, newNormal);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }
    }

    public float GetAngleBySphere()
    {
        Vector3 normal = (transform.position - center).normalized;
        float angle = Vector3.Angle(normal, sphereCenterTransform.forward);
        return angle;
    }

    public IEnumerator SetInitialPosAndRot()
    {
        if (sphereCenterTransform != null)
            center = sphereCenterTransform.position;

        Vector3 startPos= transform.position;
       
        Vector3 normal = (transform.position - center).normalized;

       
        Vector3 tangentDir = Vector3.Cross(Vector3.Cross(normal, transform.forward), normal).normalized;

      
        float angle = moveSpeed * Time.deltaTime / sphereRadius;



        Vector3 axis = Vector3.Cross(normal, tangentDir).normalized;
        Quaternion rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis);
        Vector3 newPos = rotation * (transform.position - center);
        Vector3 newNormal = newPos.normalized;

    
        float t = Time.time * waveSpeed;
        float noise = Mathf.PerlinNoise(newNormal.x * waveScale + t, newNormal.z * waveScale + t);
        float waveHeight = (noise * 2f - 1f) * waveAmplitude;
        float desiredDist = sphereRadius + waveHeight;

        Vector3 targetPos= center + newNormal * desiredDist;
       
        Vector3 nextDir = Vector3.Cross(Vector3.Cross(newNormal, tangentDir), newNormal).normalized;
        Quaternion rot;
   
       
        rot = Quaternion.LookRotation(nextDir, newNormal);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
       


        float timer = 0f;
        while (timer < 1f)
        {
            transform.SetPositionAndRotation(Vector3.Lerp(startPos, targetPos, timer / 1f), 
                Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f));
            timer += Time.deltaTime;
            yield return null;
        }
        canMove = true;

    }
}
