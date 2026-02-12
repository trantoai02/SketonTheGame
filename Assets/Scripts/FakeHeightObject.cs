
using UnityEngine;
using UnityEngine.Events;

public class FakeHeightObject : MonoBehaviour
{
  
    public UnityEvent onGroundHitEvent;
    public Transform trnsObject;
    public Transform trnsBody;
    public Transform trnsShadow;

    public float gravity = -10f;
    public float bounceFactor = 1;
    public Vector2 groundVelocity;
    public float verticalVelocity;
    public float lastInitialVerticalVelocity;
    public float initialVelo;

    public bool isGrounded;

    private void Start()
    {
        initialVelo = verticalVelocity;
    }
    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        CheckGroundHit();
       
    }

  

    public void Initialize(Vector2 groundVelocity, float verticalVelocity)
    {
        isGrounded = false;
        this.groundVelocity = groundVelocity;
        this.verticalVelocity = verticalVelocity;
        lastInitialVerticalVelocity = verticalVelocity;


    }
    void UpdatePosition()
    {
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
            trnsBody.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;

        }
        trnsObject.position += (Vector3)groundVelocity * Time.deltaTime;
    }

    void CheckGroundHit()
    {
        if (trnsBody.position.y < trnsObject.position.y && !isGrounded)
        {
            trnsBody.position = trnsObject.position;
            isGrounded = true;
            GroundHit();
        }
    }

    void GroundHit()
    {
        if(groundVelocity != Vector2.zero)
            AudioManager.instance.PlaySFX("drafle_bullet_hit", transform);

        onGroundHitEvent.Invoke();
    }

    public void Stick()
    {
        groundVelocity = Vector2.zero;
    }

    public void Bounce()
    {
        if(lastInitialVerticalVelocity < initialVelo / 1.5)
        {
            lastInitialVerticalVelocity = 0;
            groundVelocity = Vector2.zero;
        }
        Initialize(groundVelocity, lastInitialVerticalVelocity / bounceFactor);
    }

    public void SlowDownGroundVelocity()
    {
        groundVelocity = groundVelocity / bounceFactor;
    }

  
}
