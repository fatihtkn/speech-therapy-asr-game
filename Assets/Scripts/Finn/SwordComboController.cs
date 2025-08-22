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

    // StateMachineBehaviour �a�r�lar�:
    public void OnAttackStateEnter()
    {
        isAttackingWindow = true;
        // E�er istiyorsan burada sadece collider aktivasyonu yapabilirsin.
    }

    public void OnAttackStateExit()
    {
        isAttackingWindow = false;
        // Collider�� kapat ya da ba�ka temizleme i�lemi.
    }

    // �rnek: silah Collider�� ayr� bir GameObject �zerinde olabilir.
    // E�er bu script silah objesinde de�ilse, referans ile bulman gerekebilir.
    private void OnTriggerEnter(Collider other)
    {
        if (!isAttackingWindow) return;
        if (isHitStopping) return; // zaten hit stop i�erisindeyse tekrarlama

        // Burada �other� bir d��man katman�ysa:
        if (other.CompareTag("Enemy"))
        {
            // Vuru� mant���n� burada yap: hasar, efektler vb.
            // Sonra hit stop�u tetikle:
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
        // FixedDeltaTime da timeScale�e g�re ayarla, aksi halde physics sorun ��kabilir
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Unscaled bekle: ger�ek zamanl� olarak hitStopDuration bekle
        yield return new WaitForSecondsRealtime(hitStopDuration);

        // Geri d�nd�r
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDelta;

        isHitStopping = false;
    }
}
