using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public void OnCollide()
    {
        Stick.Instance.EnableWeaponCollider();
    }

    public void OffCollide()
    {
        Stick.Instance.DisableWeaponCollider();
    }
}
