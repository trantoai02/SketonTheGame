using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Skills : MonoBehaviour
{
    public Canvas skillOneTargetCanvas;
    public Image skillOneRangeIndicator;
    public float maxAbility2Distance = 3;


    SkillSummoner skillSummoner;

    private CustomInput input = null;
    public Vector2 aimDir;

    public Transform aimTransform;

    public GameObject MobileCanvas;

    // player's skill
    public SkillSO lightningStrikeSkill;

    private void Awake()
    {
        skillSummoner = GetComponent<SkillSummoner>();

        // khởi tạo new custom input
        input = new CustomInput();
        input.Player.Aim.performed += Aim_performed;
        input.Player.SkillAttack.performed += SkillAttack_performed;
        input.Player.Attack.performed += Attack_performed;

        if(MobileCanvas.activeSelf)
        {
            input.Player.SkillAttack.canceled += SkillAttack_canceled;
        }
        else
        {
            return;
        }
    }

  

    //thực thi skill LightningStrike
    private void Attack_performed(InputAction.CallbackContext obj)
    {
        if (skillOneTargetCanvas.enabled)
        {
            if(!MobileCanvas.activeSelf)
            {
                if (PlayerStaminaManager.instance.currentStamina >= lightningStrikeSkill.staminaAmount)
                {
                    // tiêu hao stamina
                    PlayerStaminaManager.instance.StaminaConsume(lightningStrikeSkill.staminaAmount);

                    //gọi tia sét tại vị trí X
                    skillSummoner.SummonSkill(lightningStrikeSkill.skillPrefab, skillOneTargetCanvas.transform.position);
                }
                skillOneTargetCanvas.enabled = false;
                skillOneRangeIndicator.enabled = false;
                Cursor.visible = true;
            }
        }
    }

    //mở thanh ngắm skill LightningStrike
    private void SkillAttack_performed(InputAction.CallbackContext obj)
    {
        if (!MobileCanvas.activeSelf){
            skillOneTargetCanvas.enabled = !skillOneTargetCanvas.enabled;
            skillOneRangeIndicator.enabled = !skillOneRangeIndicator.enabled;
        }
        else
        {
            if (!skillOneTargetCanvas.enabled)
            {
                skillOneTargetCanvas.enabled = true;
                skillOneRangeIndicator.enabled = true;
            }
           
        }
        Cursor.visible = false;
    }

    private void SkillAttack_canceled(InputAction.CallbackContext obj)
    {
        if (PlayerStaminaManager.instance.currentStamina >= lightningStrikeSkill.staminaAmount)
        {
            // tiêu hao stamina
            PlayerStaminaManager.instance.StaminaConsume(lightningStrikeSkill.staminaAmount);

            //gọi tia sét tại vị trí X
            skillSummoner.SummonSkill(lightningStrikeSkill.skillPrefab, skillOneTargetCanvas.transform.position);
        }

        skillOneTargetCanvas.enabled = false;
        skillOneRangeIndicator.enabled = false;
        Cursor.visible = true;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Aim_performed(InputAction.CallbackContext obj)
    {
        aimDir = obj.ReadValue<Vector2>();
    }

    void Start()
    {
        skillOneTargetCanvas.enabled = false;
        skillOneRangeIndicator.enabled = false;

    }

    void Update()
    {
       // skillOneInput();
        skillOneCanvas();
    }

    //private void skillOneInput()
    //{
    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        skillOneTargetCanvas.enabled = true;
    //        skillOneRangeIndicator.enabled = true;

    //        Cursor.visible = false;
    //    }

    //    if (skillOneTargetCanvas.enabled && Input.GetMouseButtonDown(0))
    //    {
           
    //            skillSummoner.SummonLightningStrike(skillOneTargetCanvas.transform.position);

    //        skillOneTargetCanvas.enabled = false;
    //        skillOneRangeIndicator.enabled = false;

    //        Cursor.visible = true;
    //    }
    //}

    private void skillOneCanvas()
    {
        var gamepadDevices = Gamepad.all;
        if (gamepadDevices.Count > 0 || MobileCanvas.gameObject.activeSelf)
        {
            float horizontal = aimDir.x;
            float vertical = aimDir.y;

            // tạo một vector từ giá trị đầu vào
            Vector3 inputDirection = new Vector3(horizontal, vertical, 0);

            // nếu không có đầu vào thì return
            if (inputDirection.magnitude < 0.1f)
            {
                return;
            }

            // tính toán vị trí tâm ngắm 
            Vector3 newHitPos = aimTransform.position + inputDirection * maxAbility2Distance;

            // đặt vị trí mới cho tâm ngắm
            skillOneTargetCanvas.transform.position = newHitPos;
        }
        else
        {

            // lấy vị trí con trỏ chuột trên màn hình và chuyển sang tọa độ thế giới game
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //lấy vị trí chiếu trên mặt phẳng 2d
            mousePosition.z = 0;

            // tính toán khoảng cách từ aimTransform đến con trỏ chuột
            Vector3 mouseAim = mousePosition - aimTransform.position;

            Vector3 newHitPos = Vector3.zero;

            // nếu khoảng cách lớn hơn maxAbility2Distance, xoay theo chiều con trỏ chuột, nhưng vị trí tâm ngắm sẽ ở rìa đường biên
            if (mouseAim.magnitude > maxAbility2Distance)
            {
                newHitPos = aimTransform.position + mouseAim.normalized * maxAbility2Distance;
            }
            // nếu khoảng cách nhỏ hơn hoặc bằng maxAbility2Distance, đặt vị trí tâm ngắm ở vị trí con trỏ chuột
            else
            {
                newHitPos = mousePosition;
            }

            // đảm bảo vị trí tâm ở trong mặt phẳng 2D
            newHitPos.z = 0;

            // gán vị trí tâm đã tính toán cho GO tâm ngắm
            skillOneTargetCanvas.transform.position = newHitPos;

        }

    }
}
