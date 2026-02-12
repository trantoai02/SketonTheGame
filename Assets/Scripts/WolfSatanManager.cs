using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSatanManager : MonoBehaviour
{
    [Header("References")]
    public GameObject wolf;
    public GameObject satan;
    public GameObject sword;
    public GameObject soulPrefab;

    [Header("Danh sách object chạy run/idle")]
    public List<GameObject> runIdleObjects;

    private SpriteRenderer wolfSpriteRenderer;
    private GoodSatanAction satanAction;

    void Start()
    {
        wolfSpriteRenderer = wolf.GetComponent<SpriteRenderer>();
        satanAction = satan.GetComponent<GoodSatanAction>();

        foreach (GameObject obj in runIdleObjects)
        {
            GoodSatanAction others = obj.GetComponent<GoodSatanAction>();
            if (others != null)
            {
                others.Run();
            }
        }
    }

    // Gọi khi player đánh wolf
    public void OnWolfHit()
    {
        // 1️⃣ Ẩn wolf
        wolfSpriteRenderer.enabled = false;

        // 2️⃣ Satan xử lý
        satanAction.Roam();

        // 3️⃣ Spawn linh hồn
        GameObject soul = Instantiate(soulPrefab, wolf.transform.position, Quaternion.identity);
        StartCoroutine(SoulMoveToSword(soul));

        foreach (GameObject obj in runIdleObjects)
        {
            GoodSatanAction others = obj.GetComponent<GoodSatanAction>();
            if (others != null)
            {
                others.Roam();
            }
        }
    }

    private IEnumerator SoulMoveToSword(GameObject soul)
    {
        float speed = 1.5f;

        while (soul != null && sword != null)
        {
            soul.transform.position = Vector3.MoveTowards(soul.transform.position, sword.transform.position, speed * Time.deltaTime);

            if (Vector3.Distance(soul.transform.position, sword.transform.position) < 0.05f)
                break;

            yield return null;
        }

        Destroy(soul);
    }
}
