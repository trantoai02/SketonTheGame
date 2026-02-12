using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    enum PickUpType
    {
        GoldCoin,
        Heart,
        Star,
        XP,
        Weapon,
        CommonItem,

    }
    [SerializeField] PickUpType pickUpType;
    [SerializeField] float pickUpDistance = 5f;
    [SerializeField] float accelartionRate = 0.2f;
    [SerializeField] float moveSpeed = 3f;

    [SerializeField] AnimationCurve animCurve;
    [SerializeField] float heightY = 1.5f;
    [SerializeField] float popDuration = 1f;


    Vector3 moveDir;
    Rigidbody2D rb;

    PlayerHealth playerHealth;

    PickupItemSetting pickUpSetting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = Player.instance.GetComponentInChildren<PlayerHealth>();
        pickUpSetting = GetComponent<PickupItemSetting>();
    }

    private void OnEnable()
    {
        StartCoroutine(AnimCurveSpamRoutine());

    }

    IEnumerator AnimCurveSpamRoutine()
    {
        Vector2 startPoint = transform.position;
        float randomX = transform.position.x + Random.Range(-2f, 2f);
        float randomY = transform.position.y + Random.Range(-1f, 1f);

        Vector2 endPoint = new Vector2(randomX, randomY);

        float timePassed = 0f;

        while (timePassed < popDuration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / popDuration;
            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            transform.position = Vector2.Lerp(startPoint, endPoint, linearT) 
                + new Vector2(0f, height);
            yield return null;
        }
    }

    private void Update()
    {
        rb.velocity = moveDir * moveSpeed * Time.deltaTime;

        Vector3 playerPos = Player.instance.transform.position;

        if(Vector3.Distance(transform.position, playerPos) < pickUpDistance )
        {
            moveDir = (playerPos - transform.position).normalized;
            moveSpeed += accelartionRate;

        }
        else
        {
            moveDir = Vector3.zero;
            moveSpeed = 0;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInParent<Player>())
        {
            if (pickUpSetting)
            {
                if (pickUpSetting.isAllowToPickup)
                {
                    DetecPickUpType();
                    Destroy(gameObject);
                }
            }
            else
            {
                DetecPickUpType();
                Destroy(gameObject);
            }

        }
    }



    void DetecPickUpType()
    {
        switch (pickUpType)
        {
            case PickUpType.GoldCoin:
                AudioManager.instance.PlaySFX("coin_collect", transform);
                EconomyManager.instance.UpdateCurrentCoin();
                Debug.Log("Gold Coin");
                break;
            case PickUpType.Heart:
                AudioManager.instance.PlaySFX("heart_collect", transform);
                playerHealth.HealPlayer(1);
                Debug.Log("Heart");
                break;
            case PickUpType.Star:
                Debug.Log("Star");
                break;
            case PickUpType.XP:
                AudioManager.instance.PlaySFX("XP_collect", transform);
                PlayerXPLevelUpManager.instance.GainXPRate(25);
                Debug.Log("XP erned!");
                break;
            case PickUpType.Weapon:
                
                Debug.Log("Weapon erned!");
                break;
            case PickUpType.CommonItem:
                AudioManager.instance.PlaySFX("item_collect", transform);
                Debug.Log("Weapon erned!");
                break;

        }
    }
}
