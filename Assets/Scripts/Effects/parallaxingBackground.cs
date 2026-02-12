
using UnityEngine;
using UnityEngine.UI;

public class parallaxingBackground : MonoBehaviour
{
    float length, startPos;
    public GameObject camera;
    public float parallaxAmount;

    private void Start()
    {
        startPos = transform.position.x;

        length = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
    }

    private void Update()
    {
        float temp = (camera.transform.position.x * (1 - parallaxAmount));

        float distance = (camera.transform.position.x * parallaxAmount);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if(temp > startPos + length)
        {
            startPos += length;

        }else if(temp< startPos - length)
        {
            startPos -= length;
        }
    }
}
