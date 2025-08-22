using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SwordComboController : MonoBehaviour
{
    Animator animator;
    int comboIndex = 0;
    float comboTimer;
    float maxComboDelay = 1.0f;
    bool canCombo = false;

 
    bool isAttackingWindow = false;
    bool isHitStopping = false;

   
    [Header("Hit Stop Settings")]
    public float hitStopDuration = 0.1f; 
    public float hitStopTimeScale = 0.1f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (comboIndex == 0)
                StartCombo();
            else if (canCombo)
                ContinueCombo();
        }

        if (comboIndex > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > maxComboDelay)
                ResetCombo();
        }
    }

    void StartCombo()
    {
        comboIndex = 1;
        animator.SetInteger("comboIndex", comboIndex);
        //animator.SetTrigger("attack");
        comboTimer = 0f;
    }

    void ContinueCombo()
    {
        canCombo = false;
        comboIndex++;
        comboIndex = Mathf.Clamp(comboIndex, 1, 4);
        animator.SetInteger("comboIndex", comboIndex);
       // animator.SetTrigger("attack");
        comboTimer = 0f;
    }

    public void EnableCombo()
    {
        canCombo = true;
    }

    public void ResetCombo()
    {
        comboIndex = 0;
        animator.SetInteger("comboIndex", 0);
        comboTimer = 0f;
        canCombo = false;
    }

    // StateMachineBehaviour çaðrýlarý:
    public void OnAttackStateEnter()
    {
        isAttackingWindow = true;
        // Eðer istiyorsan burada sadece collider aktivasyonu yapabilirsin.
    }

    public void OnAttackStateExit()
    {
        isAttackingWindow = false;
        // Collider’ý kapat ya da baþka temizleme iþlemi.
    }

    // Örnek: silah Collider’ý ayrý bir GameObject üzerinde olabilir.
    // Eðer bu script silah objesinde deðilse, referans ile bulman gerekebilir.
    private void OnTriggerEnter(Collider other)
    {
        if (!isAttackingWindow) return;
        if (isHitStopping) return; // zaten hit stop içerisindeyse tekrarlama

        // Burada “other” bir düþman katmanýysa:
        if (other.CompareTag("Enemy"))
        {
            // Vuruþ mantýðýný burada yap: hasar, efektler vb.
            // Sonra hit stop’u tetikle:
            StartCoroutine(DoHitStop());
        }
    }

    private IEnumerator DoHitStop()
    {
        isHitStopping = true;
        // Orijinal timeScale ve fixedDeltaTime kaydet
        float originalTimeScale = Time.timeScale;
        float originalFixedDelta = Time.fixedDeltaTime;

        // Uygula
        Time.timeScale = hitStopTimeScale;
        // FixedDeltaTime da timeScale’e göre ayarla, aksi halde physics sorun çýkabilir
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Unscaled bekle: gerçek zamanlý olarak hitStopDuration bekle
        yield return new WaitForSecondsRealtime(hitStopDuration);

        // Geri döndür
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDelta;

        isHitStopping = false;
    }
}
