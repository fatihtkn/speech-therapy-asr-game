using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Sýralý Þekilde Kameralarý Ekleyin")]
    public CinemachineCamera[] cameras;

    public static CameraController Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // Baþlangýçta StartCam'e geç
       
    }

    public void SwitchToCamera(CameraType cameraType)
    {
        int index = (int)cameraType;
        

        if (cameras.Length > 0)
        {
            foreach (var cam in cameras)
            {
                cam.gameObject.SetActive(false);
            }
            cameras[index].gameObject.SetActive(true);
        }
    }

    
}
public enum CameraType
{
    StartCam,
    PlayerCam,
    WatersphereCam,
    BridgeCam,
    FinalCam
}