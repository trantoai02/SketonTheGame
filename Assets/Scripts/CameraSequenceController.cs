using UnityEngine;
using Cinemachine;

public class CameraSequenceController : MonoBehaviour
{
    public static CameraSequenceController Instance { get; private set; }

    private void Awake()
    {
        // Tạo instance singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("Tham chiếu Virtual Camera")]
    public CinemachineVirtualCamera vcam;

    [Header("Các đối tượng cần follow")]
    public Transform player;
    public Transform npc;
    public Transform book;
    public Transform egoDemon;
    public Transform gem;
    

    /// <summary>
    /// Follow NPC
    /// </summary>
    public void FollowNPC()
    {
        if (vcam != null && npc != null)
            vcam.Follow = npc;
    }

    /// <summary>
    /// Follow quyển sách đang bay
    /// </summary>
    public void FollowBook()
    {
        if (vcam != null && book != null)
            vcam.Follow = book;
    } 
    
    public void FollowGem()
    {
        if (vcam != null && gem != null)
            vcam.Follow = gem;
    }

    /// <summary>
    /// Follow con EgoDemon được triệu hồi
    /// </summary>
    public void FollowEgoDemon()
    {
        if (vcam != null && egoDemon != null)
            vcam.Follow = egoDemon;
    }

    /// <summary>
    /// Trở lại player
    /// </summary>
    public void FollowPlayer()
    {
        if (vcam != null && player != null)
            vcam.Follow = player;
    }

    /// <summary>
    /// Dừng follow tạm thời (camera giữ nguyên góc nhìn)
    /// </summary>
    public void StopFollow()
    {
        if (vcam != null)
            vcam.Follow = null;
    }
}
