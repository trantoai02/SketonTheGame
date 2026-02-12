using Inventory.Model;
using UnityEngine;

public class GlowSword : MonoBehaviour, IWeapon
{
    [SerializeField] EquippableItemSO equippableItemInfo;

    public static GlowSword Instance;


    public TrailRenderer trailRenderer;

    //public Collider2D glowSwordCollider;
    public DamageSource damageSource;

    public Collider2D collider2D;

    public Vector2 pointerPosition { get; set; }
    public float staminaPerAttackCost = 0.2f;

    public Animator animator;

    private void Awake()
    {
        Instance = this;

    }
    private void Start()
    {
       
        animator = GetComponentInChildren<Animator>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        trailRenderer.enabled = false;
        collider2D = GetComponentInChildren<Collider2D>();
        collider2D.enabled = false;
    }

    public EquippableItemSO GetEquippableItemInfo()
    {
        return equippableItemInfo;
    }

    public void Attack()
    {
        if (PlayerStaminaManager.instance.currentStamina < staminaPerAttackCost)
        {
            return;
        }
        trailRenderer.enabled = true;
        //glowSwordCollider.enabled = true;
        //damageSource.enabled = true;
        collider2D.enabled = true;

        PlayerStaminaManager.instance.StaminaConsume(staminaPerAttackCost);

        AudioManager.instance.PlaySFX("meele_attack", transform);
        Debug.Log("attacking");
        animator.SetTrigger("Attack");

    }

    public void DoneAttackingAnimationEvent()
    {
        if (animator != null)
        {
            trailRenderer.enabled = false;
            //glowSwordCollider.enabled = false;
            //damageSource.enabled = false;
            collider2D.enabled = false;


        }
        else
        {
            Debug.LogWarning("Animator is null when trying to set isAttack to false.");
        }
    }

    public FoodItemSO GetFoodItemInfo()
    {
        return null;
    }

    public ItemSO GetItemInfo()
    {
        return equippableItemInfo;
    }


    public void PlayAttackSound()
    {
        AudioManager.instance.PlaySFX("meele_attack", transform);
    }

    public void ConsumeStamina()
    {
        if (PlayerStaminaManager.instance.currentStamina >= staminaPerAttackCost)
            PlayerStaminaManager.instance.StaminaConsume(staminaPerAttackCost);
    }
}
