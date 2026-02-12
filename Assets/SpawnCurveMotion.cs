using System.Collections;
using UnityEngine;

public class SpawnCurveMotion : MonoBehaviour
{
    [Header("Spawn Curve Motion")]
    [SerializeField] AnimationCurve animCurve;
    [SerializeField] float height = 1.5f;
    [SerializeField] float duration = 0.8f;
    [SerializeField] float randomRadiusX = 1.5f;
    [SerializeField] float randomRadiusY = 0.5f;

    Vector2 startPoint;
    Vector2 endPoint;

    void OnEnable()
    {
        startPoint = transform.position;

        endPoint = startPoint + new Vector2(
            Random.Range(-randomRadiusX, randomRadiusX),
            Random.Range(-randomRadiusY, randomRadiusY)
        );

        StartCoroutine(SpawnMotionRoutine());
    }

    IEnumerator SpawnMotionRoutine()
    {
        // Nếu có Rigidbody thì tắt physics tạm
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float t = timePassed / duration;

            float curveValue = animCurve.Evaluate(t);
            float yOffset = Mathf.Lerp(0f, height, curveValue);

            transform.position = Vector2.Lerp(startPoint, endPoint, t)
                                + Vector2.up * yOffset;

            yield return null;
        }

        // Bật lại physics sau khi bay xong
        if (rb != null)
        {
            rb.simulated = true;
        }
    }
}
