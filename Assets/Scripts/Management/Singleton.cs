using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton: MonoBehaviour
{
    public static Singleton instance;
    

    private void Awake()
    {
        if(instance != null && this.gameObject !=null)
            Destroy(this.gameObject);
        else

            instance = this;

        DontDestroyOnLoad(gameObject);
    }



}
