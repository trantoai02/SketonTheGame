using Inventory.Model;
using System.Collections;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public static ActiveWeapon Instance;

    [field: SerializeField] 
    public MonoBehaviour CurrentActiveWeapon {  get; private set; }

    CustomInput playerControls;

    public float timeBetweenAttacks;

    public bool attackButtonDown, isAttacking = false;

    public bool isHoldingWeapon;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        playerControls = new CustomInput();
       // timeBetweenAttacks = 0;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        playerControls.Player.Attack.started += _ => StartAttacking();
        playerControls.Player.Attack.canceled += _ => StopAttacking();
    }

    private void Update()
    {
        if (attackButtonDown)
        {
            Attack();
        }
    }
    public void NewWeapon(MonoBehaviour newWeapon)
    {
        CurrentActiveWeapon = newWeapon;
        if (CurrentActiveWeapon is IWeapon)
        {
            isHoldingWeapon = true;
            IWeapon weapon = CurrentActiveWeapon as IWeapon;

            if (weapon.GetEquippableItemInfo() != null)
            {

                EquippableItemSO equippableItem = weapon.GetEquippableItemInfo();
                timeBetweenAttacks = equippableItem.weaponCooldown;
            }
        }
        else
        {
            isHoldingWeapon = false;
            timeBetweenAttacks = 0f;
        }
    }

    public void WeaponNull()
    {
        CurrentActiveWeapon = null;
        isHoldingWeapon = false;
    }

    public IEnumerator AttackCooldown()
    {
        isAttacking = true;
        yield return new WaitForSeconds(timeBetweenAttacks);
        isAttacking = false;

    }

    void StartAttacking()
    {
        attackButtonDown = true;
       
    }

    void StopAttacking()
    {
        attackButtonDown = false;
    }
    void Attack()
    {
        if (!isAttacking && CurrentActiveWeapon is IWeapon weapon)
        {
            weapon.Attack();
            StartCoroutine(AttackCooldown());

        }
    }
}
