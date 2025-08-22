using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Leg
{
    public Transform target;        // IK target objemiz
    public Transform restPosition;  // Bacağın ağırlık noktası (default duruş)
    [HideInInspector] public Vector3 footPosition; // Güncel yer konumu
    [HideInInspector] public bool isStepping;      // O an adımda mı?
}

public class ProceduralTripodWalker : MonoBehaviour
{
    [Header("Legs & Gait Settings")]
    public List<Leg> legs;               // 3 bacak (Leg[0],Leg[1],Leg[2])
    public float stepHeight = 0.2f;      // Ayağın yükseklik tepe noktası
    public float stepDistance = 0.5f;    // Adım uzunluğu
    public float stepSpeed = 2f;         // Adım periyodu (hertz)

    [Header("Body Movement")]
    public float moveSpeed = 1.5f;       // İleri hareket hızı

    private float gaitTimer = 0f;

    void Start()
    {
        // Başlangıçta her bacağı ağırlık noktasına yerleştir
        foreach (var leg in legs)
        {
            leg.footPosition = leg.restPosition.position;
            leg.isStepping = false;
            leg.target.position = leg.footPosition;
        }
    }

    void Update()
    {
        // 1) Gait periyodunu döngüle
        gaitTimer += Time.deltaTime * stepSpeed;
        if (gaitTimer > Mathf.PI * 2f) gaitTimer -= Mathf.PI * 2f;

        // 2) Her bacağa sin, cos fazı atayarak tripod gait oluştur
        for (int i = 0; i < legs.Count; i++)
        {
            // Bacak fazını eşit böldük: 3 bacak için 120° (2π/3)
            float phase = gaitTimer + (2 * Mathf.PI / legs.Count) * i;
            StepLeg(legs[i], phase);
        }

        // 3) Gövdeyi ileri taşı
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void StepLeg(Leg leg, float phase)
    {
        // sin fazı: -1…1 arasında salınım
        float s = Mathf.Sin(phase);
        // ayağın yatayda ne kadar ileri geri kayacağı
        float xOffset = Mathf.Cos(phase) * (stepDistance / 2f);

        Vector3 targetPos = leg.restPosition.position
                            + transform.forward * xOffset
                            + transform.up * Mathf.Max(0, s) * stepHeight;

        // Adım aşaması: ayağı hızlıca ileri taşırken
        if (s > 0f)
        {
            // ayağı kaldırıyoruz
            leg.isStepping = true;
            // ayağı hedefe yaklaştır
            leg.footPosition = Vector3.Lerp(leg.footPosition, targetPos, Time.deltaTime * stepSpeed * 2f);
        }
        else
        {
            // ayağı yere indirince
            if (leg.isStepping)
            {
                // sert iniş efekti vermek istersen ek hareket ekleyebilirsin
                leg.isStepping = false;
            }
            // ayağı dinamik destek noktasına çek
            leg.footPosition = Vector3.Lerp(leg.footPosition, leg.restPosition.position
                                            + transform.forward * xOffset, Time.deltaTime * stepSpeed * 1.5f);
        }

        // IK target’ı güncelle
        leg.target.position = leg.footPosition;
    }
}
