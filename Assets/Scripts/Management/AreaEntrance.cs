using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEntrance : MonoBehaviour
{
    [SerializeField] string transitionName;

    private void Start()
    {
        if(transitionName == SceneManagement.instance.SceneTransitionName)
        {
            Player.instance.transform.position = this.transform.position;
            UIFade.instance.FadeIn();
        }
    }
}
