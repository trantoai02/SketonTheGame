using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public bool isDead {  get; private set; }

    public static PlayerHealth Instance;
    [SerializeField] public int currentHealth=0;
    [SerializeField] public int maxHealth =0;
    [SerializeField] float knockBackThrustAmount = 10f;
    [SerializeField] float damageRecoveryTime = 2f;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    // public Sprite deathStateSprite;

    [Header("Events")]
    public UnityEvent onPlayerDie; // ✅ event có thể gán trong Inspector
    public static event System.Action OnPlayerDied; // ✅ event code-based (cho script khác bắt)


    public bool canTakeDamage = true;
    Knockback knockback;
    Flash flash;
    TransparentFlicker transparentFlicker;
 

    [SerializeField] Animator animator;


    Vector2 checkpointPos;

    IEnumerator flickerRountine;

    public GameObject spriteRenderer;
    private void Awake()
    {
        Instance = this;

        flash = GetComponentInParent<Flash>();
        transparentFlicker = GetComponentInParent<TransparentFlicker>();
        knockback = GetComponentInParent<Knockback>();
        animator = spriteRenderer.GetComponent<Animator>();

       
        maxHealth = PlayerPrefs.GetInt("playerHealth", 3);
        currentHealth = maxHealth;

        flickerRountine = transparentFlicker.FlickerRoutine();

    }

    private void Start()
    {
        isDead = false;

        // tải số tim hiện tại (<= maxHealth) lên từ PlayerPrefs
        currentHealth = PlayerPrefs.GetInt("currentPlayerHealth", maxHealth);
    }
    public void HealPlayer(int amount)
    {

        currentHealth += amount;
        AudioManager.instance.PlaySFX("healing", transform);
        PlayerPrefs.SetInt("currentPlayerHealth", currentHealth);

    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if ((collision.collider.transform.tag == "Enemy" && !Player.instance.isRolling && canTakeDamage) || collision.collider.transform.tag == "EnemyBoss")
    //    {
    //        //tính chỉ số dmg scale theo level kẻ địch
    //        int damage = (int)Mathf.Floor((float)EnemyStats.Instance.damage / 10);
    //        damage = Mathf.Max(damage, 1);

    //        TakeDamage(damage, collision.transform);
    //        if (knockback != null)
    //        {
    //            knockback.GetKnockedBack(collision.gameObject.transform, knockBackThrustAmount);

    //        }
    //        StartCoroutine(flash.FlashRoutine());

    //    }

    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {

   
        if ((collision.transform.tag == "Enemy" && !Player.instance.isRolling && canTakeDamage) || collision.transform.tag == "EnemyBoss")
        {
            //tính chỉ số dmg scale theo level kẻ địch
            int damage = (int)Mathf.Floor((float)EnemyStats.Instance.damage / 10);
            damage = Mathf.Max(damage, 1);

            TakeDamage(damage, collision.transform);
            if (knockback != null)
            {
                knockback.GetKnockedBack(collision.gameObject.transform, knockBackThrustAmount);

            }
            StartCoroutine(flash.FlashRoutine());

        }
    }



    private void Update()
    {
        UpdatePlayerHealthUI();
    }


    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;

        PlayerPrefs.SetInt("playerHealth", maxHealth);
        PlayerPrefs.Save();
    }


    public void UpdatePlayerHealthUI()
    {
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;

            }

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    public void TakeDamage(int damageAmount, Transform hitSource)
    {
        Debug.Log(damageAmount);
        if (!canTakeDamage) return;

        StartCoroutine(DamageRecoveryRoutine());
        canTakeDamage = false;
        AudioManager.instance.PlaySFX("hurt", transform);
        currentHealth -= damageAmount;

        if (currentHealth > 0)
            PlayerPrefs.SetInt("currentPlayerHealth", currentHealth);

        // ✅ kiểm tra null an toàn
        if (knockback != null)
            knockback.GetKnockedBack(hitSource, knockBackThrustAmount);

        if (flash != null)
            StartCoroutine(flash.FlashRoutine());

        CheckDeath();
    }

    void CheckDeath()
    {
        if (currentHealth <= 0 && !isDead)
        {

            AudioManager.instance.PlaySFX("player_death", transform);
            AudioManager.instance.PlaySFX("hurt", transform);

            // biểu diễn animation die
            isDead = true;
            spriteRenderer.GetComponent<SpriteRenderer>().sortingOrder = 10;

            animator.SetBool("isDead", true);

            // để animation không bị che bởi bất cứ thứ gì

            //ngừng chớp
            StopCoroutine(flickerRountine);
            transparentFlicker.sr.color = transparentFlicker.defaultColor;
            canTakeDamage = false;

         

            StartCoroutine( Respawn());
        }
  
    }
   
    IEnumerator Respawn()
    {

        yield return new WaitForSeconds(2);

        StartCoroutine(ImmuneRoutine());

        isDead = false;

        int currentCoin = EconomyManager.instance.currentCoin;

        // 20%
        int percentLoss = Mathf.FloorToInt(currentCoin * 0.2f);

        // ép trong khoảng 5 → 20
        int coinToTake = Mathf.Clamp(percentLoss, 5, 20);

        EconomyManager.instance.SpendMoney(coinToTake);
        EconomyManager.instance.SaveCoins();
        EconomyManager.instance.UpdateCoinText();

        // khôi phục đầy đủ số tim
        currentHealth = maxHealth;
        PlayerPrefs.SetInt("currentPlayerHealth", maxHealth);
        spriteRenderer.GetComponent<SpriteRenderer>().sortingOrder = 0;

        animator.SetBool("isDead", false);


        //trở lại vị trí tại điểm lưu - checkpoint
        Player.instance.transform.position = GameManager.instance.lastCheckpointPosition;
       // transform.position = GameManager.instance.lastCheckpointPosition;

        //cân nhắc tùy chọn mất XP hoặc giữ nguyên sau khi respawn
        PlayerXPLevelUpManager.instance.LoadXP();
        //StartCoroutine(ImmuneRoutine());
        onPlayerDie?.Invoke();    // Cho UnityEvent trong Inspector
        OnPlayerDied?.Invoke();

        yield return null;
    }

    IEnumerator ImmuneRoutine()
    {
        canTakeDamage = false;
        IEnumerator respawnFlicker = transparentFlicker.FlickerRoutine();
       
        StartCoroutine(respawnFlicker);
        yield return new WaitForSeconds(3);
        StopCoroutine(respawnFlicker);
        transparentFlicker.sr.color = transparentFlicker.defaultColor;
        canTakeDamage = true;
    }

    IEnumerator DamageRecoveryRoutine()
    {
        
        StartCoroutine(flickerRountine);
        yield return new WaitForSeconds(damageRecoveryTime);
        StopCoroutine(flickerRountine);
        transparentFlicker.sr.color = transparentFlicker.defaultColor;
        canTakeDamage = true;

    }

    // ✅ Gọi thay vì TakeDamage để hỗ trợ cơ chế dodge
    public void TryTakeDamage(int damageAmount, Transform hitSource)
    {
        // Nếu đang lăn thì không nhận damage
        if (Player.instance != null && Player.instance.isRolling)
        {
            Debug.Log("🌀 Player đang dodge - không nhận sát thương");
            return;
        }

        TakeDamage(damageAmount, hitSource);
    }

}
