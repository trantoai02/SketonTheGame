using UnityEngine;

public class DestroyIfNoChildren : MonoBehaviour
{
    void Update()
    {
        if (transform.childCount == 1)
        {
            Destroy(gameObject);
        }
    }
}
