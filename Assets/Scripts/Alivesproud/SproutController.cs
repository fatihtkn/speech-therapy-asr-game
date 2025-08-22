using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;

public class SproutController : MonoBehaviour
{
    public float speed = 1.5f;
    Animator animator;
    [SerializeField] private Transform targetDestination;
    NavMeshAgent agent;
    [SerializeField]private Material material;
    private SkinnedMeshRenderer meshRenderer;
    private Renderer rend;
    private MaterialPropertyBlock block;
    [SerializeField]private GameObject bloomForm;
    
    private static readonly int SelfShadingSize = Shader.PropertyToID("_SelfShadingSize");

    bool canMove;
    bool isReachedDestination;


    public event Action OnReachedDestination;
    public event Action OnWatered;
   

    [Header("Easing")]
    [SerializeField] private Ease easeType = Ease.InOutQuad;
    private void Awake()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rend = GetComponentInChildren<Renderer>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }
    void Start()
    {
        agent.SetDestination(targetDestination.position);
        rend.GetPropertyBlock(block);
        agent.speed = 0f;
       
        block.SetFloat(SelfShadingSize, 1f);
        rend.SetPropertyBlock(block);
     

    }

 

    void Update()
    {


        if (!agent.enabled) return;

        if (agent.remainingDistance<agent.stoppingDistance&&!isReachedDestination)
        {

            isReachedDestination = true;
            agent.enabled = false;
            animator.enabled = false;

            OnReachedDestination?.Invoke();

        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(Green());
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("RainCloud"))
        {
            OnWatered?.Invoke();
            StartCoroutine(Green());
            GetComponent<Collider>().enabled = false;
        }
    }

    public IEnumerator Green()
    {

        Vector3 startPos = transform.position;
        canMove = false;

        yield return ChangeSelfShadingSize();

        Sequence jumpSeq = DOTween.Sequence();

        float targetY = startPos.y + 2f;
        jumpSeq.Append(transform.DOMoveY(targetY, 0.4f).SetEase(Ease.OutQuad));
        
        jumpSeq.Append(transform.DORotate(new Vector3(0f, 180f, 0f), 0.3f, RotateMode.WorldAxisAdd).SetEase(Ease.InOutQuad));

        jumpSeq.Append(transform.DOScaleY(1f, 0.4f).SetEase(Ease.OutElastic).OnPlay(() =>
        {
            //transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f, 10, 0.5f).SetEase(Ease.OutElastic); 
        }));
     
        jumpSeq.Append(transform.DOMoveY(startPos.y, 0.4f).SetEase(Ease.InQuad));


        jumpSeq.OnComplete(() =>
        {
            agent.enabled = true;
            animator.enabled = true;
            agent.speed = 5f;

            canMove = true;

        }).SetDelay(0.5f);
    }



    public void Bloom()
    {
        GameObject newForm = null;
        Vector3 newFormStartSize = Vector3.zero;
        Transform currentParent= StoneHandPuzzle.Instance.targetPlayerPos;
        Sequence bloomSeq = DOTween.Sequence();

      
        bloomSeq.Append(transform.DOScale(0f, 0.4f).SetEase(Ease.InOutBounce));

      
        bloomSeq.OnComplete(() =>
        {
            meshRenderer.enabled = false;

            newForm = Instantiate(bloomForm, transform.position, bloomForm.transform.rotation);
         
            newFormStartSize = newForm.transform.localScale;
            newForm.transform.localScale = Vector3.zero;

            // Yeni bir sequence baþlat
            Sequence newFormSeq = DOTween.Sequence();
            newFormSeq.Append(newForm.transform.DOScale(newFormStartSize, 0.3f).SetDelay(0.2f).SetEase(Ease.InOutBounce));
            newFormSeq.Append(newForm.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f, 10, 0.5f).SetEase(Ease.OutElastic)).OnComplete(() =>
            {
                newForm.transform.parent = currentParent;
            });
        });
    }

    private IEnumerator ChangeSelfShadingSize()
    {
        float currentSelfShadingSize = 1f;
        float targetSize = 0f;

        while (currentSelfShadingSize > targetSize)
        {
            currentSelfShadingSize= Mathf.Clamp(currentSelfShadingSize, 0f, 1f);
            block.SetFloat(SelfShadingSize, currentSelfShadingSize);
            rend.SetPropertyBlock(block);
            currentSelfShadingSize -= Time.deltaTime * 0.5f;
            yield return null;
        }

      
    }


}
