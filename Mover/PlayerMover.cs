using System.Collections;
using UnityEngine;

public class PlayerMover : Mover
{
    [SerializeField]
    protected bool m_DynamicCameraMove = false;

    // 내부 변수
    private Transform m_CameraView = null;
    private float m_DashSec = 0;
    private bool m_WalkKeyWait = false;
    private bool m_DashKeyWait = false;
    private bool m_LastInput = false;

    private bool m_ReqUp = false;
    private bool m_ReqDown = false;
    private bool m_ReqLeft = false;
    private bool m_ReqRight = false;

    public void DashSkillUse()
    {
        StartCoroutine(DashSkill(m_MovementSpeed, m_DashSpeed));
        m_MovementSpeed = 75f;
        m_DashSpeed = 1f;
        if (m_LastInput) m_ReqLeft = true;
        else m_ReqRight = true;
        m_MoveReq = true;
        m_Movable = false;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Info>().Invincible(0.7f);
    }

    IEnumerator DashSkill(float p_MovementSpeed, float p_DashSpeed)
    {
        yield return new WaitForSeconds(0.1f);
        m_MovementSpeed = 0;
        m_DashSpeed = 0;
        yield return new WaitForSeconds(0.2f);
        m_MovementSpeed = p_MovementSpeed;
        m_DashSpeed = p_DashSpeed;
        m_MoveReq = false;
        m_Movable = true;
        m_ReqLeft = false;
        m_ReqRight = false;
        yield return new WaitForSeconds(0.4f);
        GetComponent<CapsuleCollider>().enabled = true;
    }

    private void Awake()
    {
        m_CameraView = GameObject.FindWithTag("PlayerView").transform;
        m_Model = GameObject.FindWithTag("PlayerModel").GetComponent<Animator>();
        m_ModelTransform = m_Model.GetComponent<Transform>();
    }

    void Update()
    {
        bool up = Input.GetKey(KeyCode.UpArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);

        if (left) m_LastInput = true;
        else if (right) m_LastInput = false;

        DashUpdate(left, right);
        if (m_Movable && !m_MoveReq)
        {
            BasisUpdate(up, down, left, right);
        }
        else if (m_MoveReq)
        {
            BasisUpdate(
                m_ReqUp,
                m_ReqDown,
                m_ReqLeft,
                m_ReqRight);
        }
        CameraUpdate();
        if (m_DynamicCameraMove) DynamicCameraMove();
    }

    // 보는 방향에 따라 카메라 뷰 전환
    void CameraUpdate()
    {
        Vector3 tempmodel = m_ModelTransform.localRotation.eulerAngles;

        if (m_TempMove.z > 0)
        {
            tempmodel.y = 0;
        }
        else if (m_TempMove.z < 0)
        {
            tempmodel.y = 180f;
        }
        else return;

        m_ModelTransform.localRotation = Quaternion.Euler(tempmodel);
    }

    void DynamicCameraMove()
    {
        Vector3 tempcamera = m_CameraView.localRotation.eulerAngles;
        Vector3 temppos = m_CameraView.localPosition;

        if (m_TempMove.z > 0)
        {
            tempcamera.y = -85f;
            temppos.z = 3f;
        }
        else if (m_TempMove.z < 0)
        {
            tempcamera.y = -95f;
            temppos.z = -3f;
        }
        else
        {
            tempcamera.y = -90f;
            temppos.z = 0f;
        }

        m_CameraView.localRotation = Quaternion.Euler(tempcamera);
        m_CameraView.localPosition = temppos;
    }

    void DashUpdate(bool left, bool right)
    {
        if (m_DashSec > 0) m_DashSec -= Time.deltaTime;
        else
        {
            if (m_WalkKeyWait) m_WalkKeyWait = false;
            if (m_DashKeyWait) m_DashKeyWait = false;
        }

        if (left || right)
        {
            if (m_isDashing) return;
            if (m_DashKeyWait && m_LastInput == left && m_DashSec > 0)
            {
                m_isDashing = true;
                m_WalkKeyWait = false;
                m_DashSec = 0.2f;
            }
            else
            {
                m_DashKeyWait = false;
                m_WalkKeyWait = true;
                m_DashSec = 0.2f;
            }
        }
        else
        {
            m_isDashing = false;
            if (m_WalkKeyWait && !m_DashKeyWait && m_DashSec > 0)
            {
                m_DashSec = 0.2f;
                m_DashKeyWait = true;
            }
        }
    }
}