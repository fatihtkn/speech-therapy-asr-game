using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class StoneHandPuzzle : PuzzleBase
{
    public GameObject rainCloud;
    public Material rainCloudMaterial;
    public GameObject rockHand;
    [SerializeField] private Animator handAnimator;
    private bool isRainCloudActive = false;
    private Vector3 velocity;
    [SerializeField] private float rainCloudSmoothTime = 0.5f;
    [SerializeField] private Vector3 trackOffset;
    public static StoneHandPuzzle Instance { get; private set; }
    private int wateredSproutCount;
    private  int arrivedSproutCount;

    public List<SproutController> sprouts;

    [SerializeField]private VisualEffect sunRay;

     public Transform targetPlayerPos;
    [SerializeField] private GameObject altarFlower;
    private void Awake()
    {
        Instance = this;
       


    }
    private void Start()
    {
        rainCloud.SetActive(false);
        rainCloudMaterial.DOFade(0f, 0f);
        foreach (var sprout in sprouts)
        {
            sprout.OnReachedDestination += Sprout_OnReachedDestination;
            sprout.OnWatered += Sprout_OnWatered;
        }
    }

    private void Sprout_OnWatered()
    {
        wateredSproutCount++;
        if (wateredSproutCount >= sprouts.Count)
        {
            isRainCloudActive = false;
            rainCloudMaterial.DOFade(0f, 1f).SetDelay(0.5f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                rainCloud.SetActive(false);
            });
        }
    }

    private void Sprout_OnReachedDestination()
    {
        arrivedSproutCount++;
        if (arrivedSproutCount >= sprouts.Count)
        {
            
        }
    }

    public override void OnPuzzleSolved(out bool haveAllPhasesCompleted)
    {
        int phaseIndex= currentPuzzlePhaseIndex;
        base.OnPuzzleSolved(out haveAllPhasesCompleted);

        CheckPuzzleState(phaseIndex);
    }


    private void Update()
    {
        if (isRainCloudActive)
        {
            rainCloud.transform.position=Vector3.SmoothDamp(rainCloud.transform.position, PlayerMovementController.Instance.transform.position+ trackOffset, ref velocity, rainCloudSmoothTime);
        }
    }

    private void CheckPuzzleState( int index)
    {
        print(index);
        switch (index)
        {
            case 0:
                isRainCloudActive = true;
                rainCloud.SetActive(true);
                rainCloudMaterial.DOFade(1f, 0.5f);
                break;

            case 1:
                StartCoroutine(BloomAllFlowers());
                sunRay.gameObject.SetActive(true);
                break;

            default:
               
                break;
        }
    }
    private IEnumerator BloomAllFlowers()
    {
        yield return new WaitForSeconds(3f);
        foreach (var sprout in sprouts)
        {
            sprout.Bloom();
            yield return new WaitForSeconds(0.2f);
        }
        
        yield return PlayerMovementController.Instance.Elevate(this);
        yield return new WaitForSeconds(0.5f);
        altarFlower.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic);

        sunRay.enabled = false;
        handAnimator.SetTrigger("Elevate");
        yield return new WaitForSeconds(3f);
        PlayerMovementController.Instance.transform.parent = null;
        PlayerMovementController.Instance.canMove = true;
        


    }
    
    
}
