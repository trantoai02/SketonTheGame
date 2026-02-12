using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab, heartPrefab, xpPrefab;
    public Transform spawnPos;

    [Header("🔥 Slime Flame Drop")]
    [SerializeField] GameObject customPrefab;
    [SerializeField] int slimeFlameAmount = 4;

    public void DropHeartOrCoin()
    {
        int randomNum = Random.Range(1, 5);

        if(randomNum == 1)
        {
            Instantiate(heartPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }

    public void DropExpPoints(int xpQuantity)
    {
        for (int i = 0; i < xpQuantity; i++) 
        {
            if(spawnPos==null)
            {
                Instantiate(xpPrefab, transform.position, Quaternion.identity);

            }
            else
            {
                Instantiate(xpPrefab, spawnPos.transform.position, Quaternion.identity);

            }

        }
    }

    public void DropCustomItem()
    {
        if (customPrefab == null) return;

        Vector3 pos = spawnPos == null ? transform.position : spawnPos.position;

        for (int i = 0; i < slimeFlameAmount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            Instantiate(customPrefab, pos + (Vector3)randomOffset, Quaternion.identity);
        }
    }

}
