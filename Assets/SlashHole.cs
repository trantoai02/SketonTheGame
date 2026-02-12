using UnityEngine;

public class SlashHole : MonoBehaviour
{
    [Header("Life Time")]
    [SerializeField] private float lifeTime = 1.5f;

    private void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

   
}
