
using UnityEngine;

public class DeathState : MonoBehaviour
{
    
    public void DeadSoDestroy()
    {
        gameObject.GetComponent<Animator>().enabled = false;
    }
}
