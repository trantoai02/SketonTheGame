using System.Collections;
using UnityEngine;

public class PortalHell : MonoBehaviour
{
    [Header("Key định danh cho portal này")]
    public int portalKey;

    [Header("Key cổng đầu ra (player sẽ dịch chuyển tới cổng có key này)")]
    public int exitKey;

    [Header("Offset vị trí khi ra khỏi cổng (tránh đứng chồng vào portal)")]
    public Vector2 exitOffset = new Vector2(0f, 1f);

    [Header("Thời gian fade")]
    public float fadeOutDuration = 0.4f;
    public float fadeInDuration = 0.4f;

    private static PortalHell[] allPortals;
    private static PortalHell lastExitPortal;
    private bool canTele = true;   // mặc định cho phép teleport

    private void Awake()
    {
        if (allPortals == null || allPortals.Length == 0)
            allPortals = FindObjectsOfType<PortalHell>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // Nếu player vừa spawn tại portal này => không teleport
        if (!canTele && lastExitPortal == this)
            return;

        StartCoroutine(TeleportRoutine(collision.transform));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // Khi player rời portal thì cho phép teleport lại
        if (lastExitPortal == this)
            canTele = true;
    }

    private IEnumerator TeleportRoutine(Transform player)
    {
        if (!canTele) yield break;

        if (exitKey == portalKey) yield break;

        canTele = false; // khóa teleport cho portal cũ

        // Fade out
        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeOut(fadeOutDuration);

        // Teleport
        PortalHell exitPortal = FindPortalByKey(exitKey);
        if (exitPortal != null)
        {
            Vector3 targetPos = exitPortal.transform.position + (Vector3)exitPortal.exitOffset;

            // Teleport player
            player.position = targetPos;

            // portal mới là portal spawn → không teleport lại
            lastExitPortal = exitPortal;
            exitPortal.canTele = false;
        }

        yield return new WaitForSeconds(1f);

        // Fade in
        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeIn(fadeInDuration);
    }

    private PortalHell FindPortalByKey(int key)
    {
        foreach (PortalHell p in allPortals)
        {
            if (p.portalKey == key)
                return p;
        }
        return null;
    }
}
