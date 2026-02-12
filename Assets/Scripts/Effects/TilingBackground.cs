
using UnityEngine;


public class TilingBackground : MonoBehaviour
{
    public int offsetx = 2;

    public bool hasARightBuddy = false;
    public bool hasALeftBuddy = false;

    public bool reverseScale = false;

    float spriteWidth = 0f;

    Camera cam;
    Transform myTransform;

    private void Awake()
    {
        cam = Camera.main;

        myTransform = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = spriteRenderer.sprite.bounds.size.x;
        Debug.Log(spriteRenderer.sprite.bounds.size.x);
    }

    // Update is called once per frame
    void Update()
    {
        if(hasALeftBuddy == false || hasARightBuddy == false)
        {
            float camHorizontalExtend = cam.orthographicSize * Screen.width/Screen.height;

            float edgeVisiblePositionRight = (myTransform.position.x + spriteWidth/2) - camHorizontalExtend;
            float edgeVisiblePositionLeft = (myTransform.position.x - spriteWidth/2) + camHorizontalExtend;

            if(transform.position.x >= edgeVisiblePositionRight - offsetx && hasARightBuddy ==false)
            {
                MakeNewBuddy(1);
                hasARightBuddy=true;
                Debug.Log("right!");
            }
            else if(transform.position.x <= edgeVisiblePositionLeft + offsetx && hasALeftBuddy == false)
            {
                MakeNewBuddy(-1);
                hasALeftBuddy = true;
                Debug.Log("right!");
            }
        }
    }

    void MakeNewBuddy(int rightOrLeft)
    {
        //cho sprite moi spawn ra o vi tri cạnh sprite bên phải hoặc trái
        Vector3 newPos = new Vector3(myTransform.position.x + spriteWidth * rightOrLeft, myTransform.position.y, myTransform.position.z);
        Transform newBuddy = Instantiate(myTransform, newPos, myTransform.rotation) as Transform;

        if (reverseScale == true)
        {
            newBuddy.localScale = new Vector3(newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);
        }


        newBuddy.parent = myTransform.parent;

        if(rightOrLeft > 1)
        {
            newBuddy.GetComponent<TilingBackground>().hasALeftBuddy = true;
        }
        else
        {
            newBuddy.GetComponent<TilingBackground>().hasARightBuddy = true;

        }
    }
}
