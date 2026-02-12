using Inventory;

using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public int defaultDamageAmount = 5; // Damage mặc định nếu không có vũ khí
    private int damageAmount;

    private void Start()
    {
        MonoBehaviour currentActiveWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;

        if (currentActiveWeapon != null && currentActiveWeapon is IWeapon weapon)
        {
            damageAmount = weapon.GetEquippableItemInfo().weaponDamage + PlayerStats.Instance.strength;
        }
        else
        {
            Debug.LogWarning("Không có vũ khí đang được trang bị. Dùng damage mặc định.");
            damageAmount = defaultDamageAmount;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

        handHurt hurtHand = collision.GetComponent<handHurt>();

        if( hurtHand != null && TheForestMan.Instance !=null && TheForestMan.Instance.IsHandCanHurt())
        {
            Debug.Log("tay bi dou: " + damageAmount);
            hurtHand.health.TakeDamage(damageAmount);

            // Chỉ giảm độ bền nếu đang cầm vũ khí
            if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
            {
                InventoryController.instance.ModifyCurrentWeaponParameters();
            }
        }



        if (enemyHealth != null)
        {
            Debug.Log("damage amount: " + damageAmount);
            enemyHealth.TakeDamage(damageAmount);

            // Chỉ giảm độ bền nếu đang cầm vũ khí
            if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
            {
                InventoryController.instance.ModifyCurrentWeaponParameters();
            }
        }
    }
}
