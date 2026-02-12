using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public class GrabSequenceController : MonoBehaviour
{
    [Header("Target Positions")]
    public Transform targetPoint;
    public Transform endPoint;
    public Transform endPointZone1;
    public Transform endPointZone2;
    public Transform endPointZone3;
    public Transform endPointZone4;

    [Header("Settings")]
    public float moveSpeed = 3f;
    public string grabBoolName = "isGrab";
    public string newIdleBoolName = "newIdle";

    private Animator anim;
    public GameObject krixim;

    public PuzzleManager puzzleManager;

    public Cinemachine.CinemachineVirtualCamera vcam;
    public Transform player;

    List<PuzzleTile> correctTiles;


    public UnityEvent afterGrabKrixim;
    void Start()
    {
        anim = GetComponent<Animator>();

        // Lấy danh sách tile đúng

       

    }

    public void RunInZone(int zoneIndex)
    {
        vcam.Follow = this.transform;

        switch (zoneIndex)
        {
            case 1:
                correctTiles = puzzleManager.GetCorrectTilesZone1();
                break;

            case 2:
                correctTiles = puzzleManager.GetCorrectTilesZone2();
                break;

            case 3:
                correctTiles = puzzleManager.GetCorrectTilesZone3();
                break;
        }
        // Chạy path theo thứ tự tile đã được đánh dấu
        StartCoroutine(FollowTiles(correctTiles, zoneIndex));
    }

    //public void RunInZone2()
    //{
    //    vcam.Follow = this.transform;
    //    correctTiles = puzzleManager.GetCorrectTilesZone2();
    //    // Chạy path theo thứ tự tile đã được đánh dấu
    //    StartCoroutine(FollowTiles(correctTiles));
    //} 

    //public void RunInZone3()
    //{
    //    vcam.Follow = this.transform;
    //    correctTiles = puzzleManager.GetCorrectTilesZone3();
    //    // Chạy path theo thứ tự tile đã được đánh dấu
    //    StartCoroutine(FollowTiles(correctTiles));
    //}

    IEnumerator FollowTiles(List<PuzzleTile> tiles, int zoneIndex)
    {
        // Enemy đi lần lượt các tile có isCorrectStep = true
        foreach (PuzzleTile tile in tiles)
        {
            yield return MoveTo(tile.transform.position);
        }
        switch (zoneIndex)
        {
            case 1:
                yield return MoveTo(endPointZone1.transform.position);
                break;

            case 2:
                yield return MoveTo(endPointZone2.transform.position);
                break;

            case 3:
                yield return MoveTo(endPointZone3.transform.position);
                break;
        }
       

        vcam.Follow = player;
    }

    IEnumerator MoveTo(Vector3 target)
    {

        while (Vector2.Distance((Vector2)transform.position, (Vector2)target) > 0.05f)

        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Gọi hàm này để chạy toàn bộ chuỗi
    public void StartGrabSequence()
    {
  
            StartCoroutine(GrabSequenceRoutine());
    }

    public void MoveToEndPos()
    {
        StartCoroutine(MoveTo(endPointZone4.transform.position));
    }

    IEnumerator GrabSequenceRoutine()
    {
     

        // 1. Move → targetPoint
        yield return MoveToPoint(targetPoint.position);

        // 2. Bật animation Grab
        if (Vector3.Distance(transform.position, targetPoint.position) <= 0.05f)
        {
            anim.SetBool(grabBoolName, true);
        }

        // 3. Chờ animation Grab kết thúc
        yield return WaitForCurrentAnimationEnd();

        // Tắt grab, bật new idle
        //anim.SetBool(grabBoolName, false);
        anim.SetBool(newIdleBoolName, true);

        // 4. Chờ 2 giây
        yield return new WaitForSeconds(3f);

        // 5. Move → endPoint
        yield return MoveToPoint(endPoint.position);


        afterGrabKrixim?.Invoke();


    }

    public void HideKrixim()
    {
        krixim.SetActive(false);
    }

    // --- Helper: Move tới 1 vị trí ---
    IEnumerator MoveToPoint(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // --- Helper: Chờ animation hiện tại chạy xong ---
    IEnumerator WaitForCurrentAnimationEnd()
    {
        // Lấy thông tin clip hiện tại
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        // Chờ đến khi clip qua frame cuối
        while (state.normalizedTime < 1f)
        {
            yield return null;
            state = anim.GetCurrentAnimatorStateInfo(0);
        }
    }


}
