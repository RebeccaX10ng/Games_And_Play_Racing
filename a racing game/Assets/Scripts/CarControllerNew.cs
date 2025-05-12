using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarControllerNew : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxMotorTorque = 2000f;  // 最大电机扭矩
    [SerializeField] private float maxSpeed = 180f;       // 最高时速(km/h)
    [SerializeField] private float reverseSpeed = 60f;    // 倒车最高时速
    [SerializeField] private float brakeTorque = 3000f;   // 刹车力度
    
    [Header("Steering Settings")]
    [SerializeField] private float maxSteerAngle = 30f;   // 最大转向角度
    
    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftCollider;
    [SerializeField] private WheelCollider frontRightCollider;
    [SerializeField] private WheelCollider rearLeftCollider;
    [SerializeField] private WheelCollider rearRightCollider;

    private Rigidbody rb;
    private float currentMotorTorque;
    private float currentSteerAngle;
    private float currentBrakeTorque;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // 降低重心增加稳定性
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        ApplyMotorTorque();
        ApplySteering();
        ApplyBrakes();
    }

    private void HandleInput()
    {
        // 获取输入 (W/S或上/下箭头控制加速刹车，A/D或左/右箭头控制转向)
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        
        // 计算当前速度(km/h)
        float currentSpeed = rb.velocity.magnitude * 3.6f;
        
        // 前进加速
        if (verticalInput > 0)
        {
            // 限制不超过最高速度
            if (currentSpeed < maxSpeed)
            {
                currentMotorTorque = maxMotorTorque * verticalInput;
            }
            else
            {
                currentMotorTorque = 0;
            }
            currentBrakeTorque = 0;
        }
        // 倒车
        else if (verticalInput < 0)
        {
            // 限制不超过倒车最高速度
            if (Mathf.Abs(currentSpeed) < reverseSpeed)
            {
                currentMotorTorque = maxMotorTorque * verticalInput * 0.5f; // 倒车力度减半
            }
            else
            {
                currentMotorTorque = 0;
            }
            currentBrakeTorque = 0;
        }
        // 刹车
        else if (Input.GetKey(KeyCode.Space))
        {
            currentMotorTorque = 0;
            currentBrakeTorque = brakeTorque;
        }
        // 无输入时自然减速
        else
        {
            currentMotorTorque = 0;
            currentBrakeTorque = 0;
        }
        
        // 转向
        currentSteerAngle = maxSteerAngle * horizontalInput;
    }

    private void ApplyMotorTorque()
    {
        // 应用动力到后轮(后驱)
        rearLeftCollider.motorTorque = currentMotorTorque;
        rearRightCollider.motorTorque = currentMotorTorque;
    }

    private void ApplySteering()
    {
        // 前轮转向
        frontLeftCollider.steerAngle = currentSteerAngle;
        frontRightCollider.steerAngle = currentSteerAngle;
    }

    private void ApplyBrakes()
    {
        
        // 四轮刹车
        frontLeftCollider.brakeTorque = currentBrakeTorque;
        frontRightCollider.brakeTorque = currentBrakeTorque;
        rearLeftCollider.brakeTorque = currentBrakeTorque;
        rearRightCollider.brakeTorque = currentBrakeTorque;
    }

    // 提供给UI脚本获取当前速度
    public float GetSpeed()
    {
        return rb.velocity.magnitude * 3.6f; // m/s转km/h
    }
    
    
}