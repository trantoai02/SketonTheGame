
using UnityEngine;

public class CameraMovingMainMenu : MonoBehaviour
{

    public float speed = 1f;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x-1, transform.position.y, transform.position.z), speed * Time.deltaTime);
    }
}
