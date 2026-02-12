using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class ElectricPower : MonoBehaviour
{
    [Header("Trạng thái nguồn điện")]
    public static bool isPowered;

    [Header("Đèn chiếu sáng")]
    [SerializeField] private Light2D light2D;

    [Header("Sự kiện khi đèn sáng hoàn toàn")]
    public UnityEvent onFullyPowered; // 👈 Thêm sự kiện ở đây

    private void Start()
    {
        isPowered = false;
        light2D = GetComponent<Light2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ElectricSource") || collision.gameObject.GetComponent<lightningStriker>() != null)
        {
            isPowered = true;

            if (isPowered)
            {
                if (light2D != null)
                {
                    StartCoroutine(ThapSang());
                }
            }
        }
    }

    IEnumerator ThapSang()
    {
        yield return new WaitForSeconds(1f);
        light2D.intensity = 1;
        yield return new WaitForSeconds(.1f);
        light2D.intensity = .1f;
        yield return new WaitForSeconds(.2f);
        light2D.intensity = 1;
        yield return new WaitForSeconds(.2f);
        light2D.intensity = .1f;
        yield return new WaitForSeconds(.2f);
        light2D.intensity = 1;
        yield return new WaitForSeconds(.1f);
        light2D.intensity = .1f;
        yield return new WaitForSeconds(1f);

        // Đèn sáng hoàn toàn
        light2D.intensity = 1;
        Debug.Log("⚡ Nguồn điện đã được bật hoàn toàn!"); // 👈 Debug log
        onFullyPowered?.Invoke(); // 👈 Gọi sự kiện ở đây
    }
}
