using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(NavMeshAgent))]
public class RoamGhost : MonoBehaviour
{
    [Header("Alpha Settings")]
    [Range(0f, 255f)] public float minAlpha = 0f;
    [Range(0f, 255f)] public float maxAlpha = 128f;
    public float fadeDuration = 1.5f; // thời gian chuyển alpha

    [Header("Roam Settings")]
    public float roamRadius = 5f;
    public float roamInterval = 3f;
    public float fadePaceTime = 5f;

    private SpriteRenderer spriteRenderer;
    private NavMeshAgent agent;
    private Coroutine fadeCoroutine;
    private bool playerTriggered = false;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();

        // NavMeshAgent cho 2D cần set chế độ này để ko tự xoay
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        StartCoroutine(RoamRoutine());
        StartCoroutine(AlphaRoutine());
    }

    void Update()
    {
        // flip X theo hướng di chuyển
        if (agent.velocity.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (agent.velocity.x < -0.01f)
            spriteRenderer.flipX = true;
    }

    IEnumerator RoamRoutine()
    {
        while (true)
        {
            if (!playerTriggered)
            {
                Vector2 randomDirection = Random.insideUnitCircle * roamRadius;
                Vector3 destination = transform.position + new Vector3(randomDirection.x, randomDirection.y, 0);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(destination, out hit, roamRadius, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
            yield return new WaitForSeconds(roamInterval);
        }
    }

    IEnumerator AlphaRoutine()
    {
        while (true)
        {
            if (!playerTriggered)
            {
                float targetAlpha = Mathf.Approximately(spriteRenderer.color.a, maxAlpha / 255f) ?
                                    minAlpha / 255f : maxAlpha / 255f;
                yield return FadeTo(targetAlpha);
               //yield return new WaitForSeconds(fadePaceTime);
            }
            else
            {
                yield return FadeTo(minAlpha / 255f); // Player trigger → fade về 0
                yield return new WaitForSeconds(Random.Range(2f, 5f));
                playerTriggered = false; // cho phép fade lại
            }
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }

    IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        float startAlpha = spriteRenderer.color.a;
        float elapsed = 0f;

        fadeDuration = Random.Range(5, 10);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            Color c = spriteRenderer.color;
            c.a = newAlpha;
            spriteRenderer.color = c;
            yield return null;
        }
    }

    // Trigger với Player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTriggered = true;
        }
    }
}
