using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //khai báo biến instance cho 1 thể hiện duy nhất của class Player
    public static Player instance;

    //khai báo các state cho Player
    enum State
    {
        Normal,
        Rolling,
        Flat
    }

    State state;

    //References 
    //khai báo các tham chiếu cần thiết
    CustomInput input = null;

    public Vector2 moveVector = Vector2.zero;
    Vector2 rollDir;

    //Animation
    public Animator animator;

    //collider
    public Transform weaponCollider;
    public Transform parryCollider;

    //Movement

    public Vector2 pointerInput;
    public InputActionReference pointerPosition;

    public Rigidbody2D rb;
    public Transform trnsPlayer;
    public float movementSpeed = 5f;
    public float initialRollSpeed = 20f;
    public float currentRollSpeed = 5f;
  
    public float rollSpeedMultiplier =5f;
    public float rollSpeedMinimum = 10f;
    Vector3 lastMoveDir;
    public float staminaPerRollCost = 2;
    public bool isRolling = false;
    Knockback knockback;
    bool isParrying;

    //dành cho việc đổi màu sprite nhân vật
    List<Color> colors = new List<Color>() { Color.white, Color.yellow, Color.green, Color.blue, Color.black };

    public bool isMovementLocked = false;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if(instance == null )
        {
            instance = this;
        }

        input = new CustomInput();
        knockback = GetComponent<Knockback>();

        state = State.Normal;

        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCanceled;
        input.Player.Roll.performed += Roll_performed;
        input.Player.Parry.performed += Parry_performed;
        input.Player.Parry.canceled += Parry_canceled;
    }



    private void Parry_canceled(InputAction.CallbackContext obj)
    {
        // tránh việc thực hiện đỡ đòn khi không cầm vũ khí trên tay
        if (ActiveWeapon.Instance.isHoldingWeapon)
        {
            Debug.Log("no parry");
            parryCollider?.gameObject.SetActive(false);
            ActiveWeapon.Instance.CurrentActiveWeapon.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            isParrying = false;
        }
           
    }

    private void Parry_performed(InputAction.CallbackContext obj)
    {
        // tránh việc thực hiện đỡ đòn khi không cầm vũ khí trên tay
        if (ActiveWeapon.Instance.isHoldingWeapon)
        {
            if (PlayerShieldManager.instance.currentShield >= 2.5f)
            {
                AudioManager.instance.PlaySFX("player_block", transform);

                isParrying = true;
                Debug.Log("parrying");
                parryCollider?.gameObject.SetActive(true);
                ActiveWeapon.Instance.CurrentActiveWeapon.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
            else
            {
                isParrying = false;
                ActiveWeapon.Instance.CurrentActiveWeapon.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                parryCollider?.gameObject.SetActive(false);

            }
        }
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public void EndRolling()
    {
        state = State.Normal;
        animator.SetBool("isRoll", false);
        animator.SetBool("isLeftRoll", false);
        isRolling = false;
    }

    private void Roll_performed(InputAction.CallbackContext obj)
    {
        if (state == State.Flat) return;

        if (PlayerStaminaManager.instance.currentStamina < staminaPerRollCost || isRolling)
        {
            return;
        }

        PlayerStaminaManager.instance.StaminaConsume(staminaPerRollCost);
        rollDir = lastMoveDir;
        currentRollSpeed = initialRollSpeed;
        state = State.Rolling;
        isRolling = true;
        AudioManager.instance.PlaySFX("roll_action", transform);

    }

    public void EnterFlatState()
    {
        state = State.Flat;
        //bật flat layer
        animator.SetLayerWeight(flatLayerIndex, 1f);

        // Cấm hành động
        isRolling = false;
        parryCollider?.gameObject.SetActive(false);
        isParrying = false;
    }

    public void ExitFlatState()
    {
        animator.SetLayerWeight(flatLayerIndex, 0f);
        state = State.Normal;
    }

    void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }

    void OnMovementCanceled(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;
    }
    int flatLayerIndex;
    private void Start()
    {
     

       
            flatLayerIndex = animator.GetLayerIndex("Flat");
            animator.SetLayerWeight(flatLayerIndex, 0f); // ⭐ QUAN TRỌNG
        

    }
    public void ChangePlayerColor()
    {
        if(PlayerXPLevelUpManager.instance.level <= colors.Count)
            trnsPlayer.GetComponent<SpriteRenderer>().color = colors[PlayerXPLevelUpManager.instance.level-1];
        else
        {
            trnsPlayer.GetComponent<SpriteRenderer>().color = colors[colors.Count - 1];
        }
    }

    void Update()
    {
        // ChangePlayerColor();
        if (isParrying && PlayerShieldManager.instance.currentShield < 2.5f)
        {
            ActiveWeapon.Instance.CurrentActiveWeapon.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            parryCollider?.gameObject.SetActive(false);
            isParrying = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(DoPeeAnimation());
        }

    }

    IEnumerator DoPeeAnimation()
    {
        animator.SetBool("isPee", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        animator.SetBool("isPee", false);

    }

    private void FixedUpdate()
    {

        switch (state)
        {
            case State.Normal:
                Animation();
                FlipSprites();
                Move();
               
                break;
            case State.Rolling:
                rb.velocity = rollDir * currentRollSpeed;
                currentRollSpeed -= currentRollSpeed * rollSpeedMultiplier * Time.fixedDeltaTime;
                if (rollDir.x != 0 || rollDir.y != 0)
                {
                    isRolling = true;
                    if (rollDir.x > 0)
                    {
                        
                        animator.SetBool("isRoll", true);
                    }
                    else
                    {
                        animator.SetBool("isLeftRoll", true);
                    }
                }

                if (currentRollSpeed < rollSpeedMinimum)
                {
                    state = State.Normal;
                    isRolling = false;

                }
            break;

            case State.Flat:
                // ⭐ vẫn được chạy & idle
                Animation();   // dùng isRun → RunFlat / IdleFlat
                FlipSprites();
                Move();
                break;
        }

    }

    // lấy collder của vũ khí cận chiến
    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }

    // hàm di chuyển
    void Move()
    {
        if (isMovementLocked)  // ⭐ KHÓA DI CHUYỂN
        {
            rb.velocity = Vector2.zero;
            return;
        }
        // ✅ thêm kiểm tra null an toàn
        if ((knockback != null && knockback.gettingKnockback) || PlayerHealth.Instance.isDead)
        {
            rb.isKinematic = true;
            return;
        }

        rb.isKinematic = false;
        rb.MovePosition(rb.position + moveVector * movementSpeed * Time.fixedDeltaTime);

        if (moveVector.x != 0 || moveVector.y != 0)
            lastMoveDir = moveVector;
    }

    // animation khi di chuyển
    void Animation()
    {
        if (moveVector != Vector2.zero)
        {

            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
    }
    // để sprite của nhân vật hướng theo 2 chiều di chuyển - horizontal
    void FlipSprites()
    {
        if (moveVector.x < 0)
        {
            trnsPlayer.localScale = new Vector3(-1, 1, 1);
            
        }
        else
        {
            trnsPlayer.localScale = new Vector3(1, 1, 1);
           
        }
    }

    bool DialogueManagerIsActive()
    {
        return DialogueManager.instance.isDialogueActive;
    }

    public void LockPlayerMovement()
    {
        isMovementLocked = true;
    }
    public void UnlockPlayerMovement()
    {
        isMovementLocked = false;
    }

}
