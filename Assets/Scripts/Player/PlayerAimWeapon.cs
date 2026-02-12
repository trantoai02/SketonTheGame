
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimWeapon : MonoBehaviour
{
    private Transform aimTransform;

    private CustomInput input = null;

    public static PlayerAimWeapon Instance;
    //Aiming
    public Vector2 aimDir;

    public GameObject MobileCanvas;
  
    private void Awake()
    {
        Instance = this;
        aimTransform = transform.Find("Aim");

        // khởi tạo new custom input
        input = new CustomInput();
        input.Player.Aim.performed += Aim_performed;

    }

    private void Start()
    {
        ActiveInventory.Instance.ChangeActiveWeapon();
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

    private void Update()
    {

        HandleAiming();
        
    }
    void HandleAiming()
    {
        //tính toán góc tọa độ từ input
        float angle;
 
        // nếu có cắm tay cầm - hoặc không
        var gamepadDevices = Gamepad.all;
        if (gamepadDevices.Count > 0 || MobileCanvas.gameObject.activeSelf)
        {
            angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        }
        else
        {

            Vector3 mousePosition = GetMouseWorldPosition();
            Vector3 aimDirection = (mousePosition - transform.position).normalized;

            angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        }

        //áp dụng góc tọa độ vào transform của aimTransform
        aimTransform.eulerAngles = new Vector3(0, 0, angle);


        Vector3 localScale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = +1f;
        }
        aimTransform.localScale = localScale;

    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }
    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}
