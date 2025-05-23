using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    internal enum CarDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum SpeedType
    {
        MPH,
        KPH
    }

    public class CarController : MonoBehaviour
    {
        [SerializeField] private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper;
        [Range(0, 1)] [SerializeField] private float m_TractionControl;
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private SpeedType m_SpeedType;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;

        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;

        public bool isBoosting = false;
        private float normalTopSpeed = 100f;
        private float boostedTopSpeed = 150f;

        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle { get { return m_SteerAngle; } }
        public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }
        public float MaxSpeed { get { return m_Topspeed; } }
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }

        private void Start()
        {
            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++)
            {
                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
            }

            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;
            m_MaxHandbrakeTorque = float.MaxValue;
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);

            m_Topspeed = normalTopSpeed;
        }

        public void Move(float steering, float accel, float footbrake, float handbrake)
        {
            HandleBoost();

            steering = Mathf.Clamp(steering, -1, 1);
            AccelInput = accel = Mathf.Clamp(accel, 0, 1);
            BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            SteerHelper();
            ApplyDrive(accel, footbrake);
            CapSpeed();

            if (handbrake > 0f)
            {
                float hbTorque = handbrake * m_MaxHandbrakeTorque;
                m_WheelColliders[2].brakeTorque = hbTorque;
                m_WheelColliders[3].brakeTorque = hbTorque;
            }

            CalculateRevs();
            GearChanging();

            AddDownForce();
            CheckForWheelSpin();
            TractionControl();
        }

        private void HandleBoost()
        {
            isBoosting = Input.GetKey(KeyCode.W);
            m_Topspeed = isBoosting ? boostedTopSpeed : normalTopSpeed;
        }

        private void GearChanging()
        {
            float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
            float upgearlimit = (1f / NoOfGears) * (m_GearNum + 1);
            float downgearlimit = (1f / NoOfGears) * m_GearNum;

            if (m_GearNum > 0 && f < downgearlimit)
                m_GearNum--;

            if (f > upgearlimit && m_GearNum < (NoOfGears - 1))
                m_GearNum++;
        }

        private static float CurveFactor(float factor) => 1 - (1 - factor) * (1 - factor);

        private static float ULerp(float from, float to, float value) => (1.0f - value) * from + value * to;

        private void CalculateGearFactor()
        {
            float f = 1f / NoOfGears;
            float targetGearFactor = Mathf.InverseLerp(f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
            m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
        }

        private void CalculateRevs()
        {
            CalculateGearFactor();
            float gearNumFactor = m_GearNum / (float)NoOfGears;
            float revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
            float revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
        }

        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_SpeedType)
            {
                case SpeedType.MPH:
                    speed *= 2.23693629f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
                    break;
                case SpeedType.KPH:
                    speed *= 3.6f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
                    break;
            }
        }
        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void ApplyDrive(float accel, float footbrake)
        {
            float thrustTorque = 0f;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 4f);
                    for (int i = 0; i < 4; i++) m_WheelColliders[i].motorTorque = thrustTorque;
                    break;
                case CarDriveType.FrontWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                    break;
                case CarDriveType.RearWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                    break;
            }

            for (int i = 0; i < 4; i++)
            {
                if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
                {
                    m_WheelColliders[i].brakeTorque = m_BrakeTorque * footbrake;
                }
                else if (footbrake > 0)
                {
                    m_WheelColliders[i].brakeTorque = 0f;
                    m_WheelColliders[i].motorTorque = -m_ReverseTorque * footbrake;
                }
            }
        }

        private void SteerHelper()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!m_WheelColliders[i].GetGroundHit(out var hit) || hit.normal == Vector3.zero)
                    return;
            }

            if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
            {
                float turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }

        private void AddDownForce()
        {
            m_Rigidbody.AddForce(-transform.up * m_Downforce * m_Rigidbody.velocity.magnitude);
        }

        private void CheckForWheelSpin()
        {
            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].GetGroundHit(out var wheelHit);
                if (Mathf.Abs(wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= m_SlipLimit)
                {
                    m_WheelEffects[i].EmitTyreSmoke();
                    if (!AnySkidSoundPlaying()) m_WheelEffects[i].PlayAudio();
                    continue;
                }

                if (m_WheelEffects[i].PlayingAudio) m_WheelEffects[i].StopAudio();
                m_WheelEffects[i].EndSkidTrail();
            }
        }

        private bool AnySkidSoundPlaying()
        {
            for (int i = 0; i < 4; i++)
                if (m_WheelEffects[i].PlayingAudio) return true;
            return false;
        }

        private void TractionControl()
        {
            WheelHit wheelHit;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].GetGroundHit(out wheelHit);
                        AdjustTorque(wheelHit.forwardSlip);
                    }
                    break;
                case CarDriveType.RearWheelDrive:
                    m_WheelColliders[2].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    m_WheelColliders[3].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;
                case CarDriveType.FrontWheelDrive:
                    m_WheelColliders[0].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    m_WheelColliders[1].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;
            }
        }

        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
            {
                m_CurrentTorque -= 10 * m_TractionControl;
            }
            else
            {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels)
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
            }
        }

        public float GetSpeed()
        {
            throw new NotImplementedException();
        }
    }
}
