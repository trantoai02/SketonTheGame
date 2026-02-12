using Inventory.Model;
using UnityEngine;

public class Stick : MonoBehaviour, IWeapon
{
    [SerializeField] EquippableItemSO equippableItemInfo;

    public static Stick Instance;

    public Transform weaponCollider;

    public Vector2 pointerPosition { get; set; }
    public float staminaPerAttackCost = 0.2f;

    public Animator animator;
 
    private void Awake()
    {
        Instance = this;

        animator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
      //  weaponCollider = Player.instance?.GetWeaponCollider();
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
        PlayerStaminaManager.instance.StaminaConsume(staminaPerAttackCost);

        AudioManager.instance.PlaySFX("meele_attack", transform);
        Debug.Log("attacking");
        animator.SetBool("isAttack", true);

    }

    public void DoneAttackingAnimationEvent()
    {
        if (animator != null)
        {
            animator.SetBool("isAttack", false);
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

    public void EnableWeaponCollider()
    {
        weaponCollider.gameObject.SetActive(true);
    }
    public void DisableWeaponCollider()
    {
        weaponCollider.gameObject.SetActive(false);
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
