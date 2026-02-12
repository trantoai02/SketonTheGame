using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class Tentacle : MonoBehaviour
{
    public UnityEvent onGrabEnd;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform handlePoint;
    [SerializeField] private Transform moveTarget; // điểm kéo về sau khi bắt player

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float reachDistance = 0.1f;

    [Header("Slash Sequence")]
    [SerializeField] private GameObject slashHolePrefab;  // size = 3
    [SerializeField] private Transform[] slashHolePoints;   // size = 3

    [SerializeField] private float slashDelay = 0.25f;
   


    [Header("Bone Physics")]
    [SerializeField] private GameObject boneRoot;

    [Header("Player Fall")]
    [SerializeField] private Transform playerFallPos;
    [SerializeField] private float playerMoveSpeed = 5f;

    [Header("Skeleton Spawn")]
    [SerializeField] private GameObject slashHoleSketon;  // size = 3

    [SerializeField] private Transform skeletonSpawnPoint;
    [SerializeField] private GameObject skeletonClonePrefab;
    [SerializeField] private float skeletonMoveSpeed = 6f;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vcam;

    private enum TentacleState
    {
        Idle,
        MoveToPlayer,
        MoveToTarget
    }

    private TentacleState state = TentacleState.Idle;
    private bool hasGrabbedPlayer = false;
    private bool hasDone = false;

    private void Start()
    {
        state = TentacleState.MoveToPlayer;
    }
    void Update()
    {
        if (hasDone) return;
       
        if (state == TentacleState.MoveToPlayer)
        {
            MoveTo(player.position);

            if (Vector2.Distance(transform.position, player.position) <= reachDistance)
            {
                GrabPlayer(Player.instance.transform);
                Player.instance.LockPlayerMovement();
                state = TentacleState.MoveToTarget;
            }
        }
        else if (state == TentacleState.MoveToTarget)
        {
            MoveTo(moveTarget.position);

            if (Vector2.Distance(transform.position, moveTarget.position) <= reachDistance)
            {
                hasDone = true;
                Debug.Log("xong giòi!");
                onGrabEnd?.Invoke();
            }
        }
    }

    private void MoveTo(Vector2 targetPos)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
    }

    // Gọi hàm này từ Boss / Enemy / Animation Event
    public void ActivateTentacle()
    {
        hasGrabbedPlayer = false;
        state = TentacleState.MoveToPlayer;
    }

 

    private void GrabPlayer(Transform playerTransform)
    {
        hasGrabbedPlayer = true;

        playerTransform.SetParent(handlePoint);
        playerTransform.localPosition = Vector3.zero;

        state = TentacleState.MoveToTarget;
    }

    // Gọi khi muốn thả player
    public void ReleasePlayer()
    {
        if (hasGrabbedPlayer)
        {
            player.SetParent(null);
            hasGrabbedPlayer = false;
        }

        state = TentacleState.Idle;
    }

    public void SlashSequence()
    {
        StartCoroutine(SlashSequenceCoroutine());
    }

    private IEnumerator SlashSequenceCoroutine()
    {
        if (slashHolePoints.Length < 3)
        {
            Debug.LogWarning("Cần đủ 3 slashHolePoints");
            yield break;
        }

        for (int i = 0; i < 3; i++)
        {
            Instantiate(
                slashHolePrefab,
                slashHolePoints[i].position,
                Quaternion.identity
            );

            yield return new WaitForSeconds(slashDelay);
        }
    }

    public void SpawnSkeletonSlash()
    {
        StartCoroutine(SpawnSkeletonSlashCoroutine());
    }


    private IEnumerator SpawnSkeletonSlashCoroutine()
    {
        // Spawn slashHole tại điểm riêng
        Instantiate(
            slashHoleSketon,
            skeletonSpawnPoint.position,
            Quaternion.identity
        );

        yield return new WaitForSeconds(0.1f); // delay nhỏ cho cảm giác xuất hiện

        // Spawn skeletonClone
        GameObject skeletonClone = Instantiate(
            skeletonClonePrefab,
            skeletonSpawnPoint.position,
            Quaternion.identity
        );

        StartCoroutine(MoveSkeletonToPlayer(skeletonClone));
    }

    private IEnumerator MoveSkeletonToPlayer(GameObject skeleton)
    {
        Transform playerTarget = Player.instance.transform;

        // Camera follow skeleton
        if (vcam != null)
            vcam.Follow = skeleton.transform;

        while (skeleton != null &&
               Vector2.Distance(skeleton.transform.position, playerTarget.position) > 0.05f)
        {
            skeleton.transform.position = Vector2.MoveTowards(
                skeleton.transform.position,
                playerTarget.position,
                skeletonMoveSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Khi skeleton chạm target → camera quay lại player
        if (vcam != null)
            vcam.Follow = playerTarget;

        if (skeleton != null)
            Destroy(skeleton);
    }


    public void EnableBoneGravity()
    {
        if (boneRoot == null)
        {
            Debug.LogWarning("BoneRoot chưa được gán!");
            return;
        }

        Player.instance.UnlockPlayerMovement();


        SetGravityRecursive(boneRoot.transform);
    }

    private void SetGravityRecursive(Transform current)
    {
        Rigidbody2D rb = current.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1f;
        }

        foreach (Transform child in current)
        {
            SetGravityRecursive(child);
        }
    }

    public void MovePlayerToFallPos()
    {
        StartCoroutine(MovePlayerToFallPosCoroutine());
    }

    private IEnumerator MovePlayerToFallPosCoroutine()
    {
        Transform playerTransform = Player.instance.transform;

        // Nếu player đang là con của tentacle thì tách ra
        playerTransform.SetParent(null);

        Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        while (Vector2.Distance(playerTransform.position, playerFallPos.position) > 0.05f)
        {
            playerTransform.position = Vector2.MoveTowards(
                playerTransform.position,
                playerFallPos.position,
                playerMoveSpeed * Time.deltaTime
            );

            yield return null;
        }

        // đảm bảo snap đúng vị trí
        playerTransform.position = playerFallPos.position;

        if (rb != null)
        {
            rb.simulated = true;
        }
    }


}
