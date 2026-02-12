using Cinemachine;
using UnityEngine;

public class CameraRegionManager : MonoBehaviour
{
    public CinemachineConfiner2D confiner;
    public PolygonCollider2D[] regions;

    int currentRegion;

    private void Start()
    {
        currentRegion = 0;
    }
    public void UnlockNextRegion()
    {
        
        regions[currentRegion].gameObject.SetActive(true);
        currentRegion++;
        confiner.InvalidateCache();
    }
}
