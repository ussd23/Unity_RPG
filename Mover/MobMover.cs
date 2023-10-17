using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobMover : Mover
{
    // ���� ����
    [SerializeField]
    private float m_ApproachingDistance = 7f;

    // ���� ����
    private Transform m_Target;

    private bool m_Up = false;
    private bool m_Down = false;
    private bool m_Left = false;
    private bool m_Right = false;

    private float m_JumpCooldown = 2f;
    private float m_PassedTime = 2f;

    private void Start()
    {
        GameObject obj = Info.PlayerInfo.gameObject;

        if (obj == null) return;
        m_Target = obj.GetComponent<Transform>();
    }

    void Update()
    {
        TargetApproach();
        if (m_Movable && !m_MoveReq)
        {
            BasisUpdate(m_Up, m_Down, m_Left, m_Right);
        }
    }

    // �÷��̾ ����
    void TargetApproach()
    {
        if (m_Target == null) return;

        m_Up = false;
        m_Down = false;
        m_Left = false;
        m_Right = false;

        Vector3 pos = transform.position;
        Vector3 playerpos = m_Target.position;
        float distance = GetDistance();

        // �ν� ������ �ƴ� ��� ����
        if (distance < 2f || distance > m_ApproachingDistance) return;

        Vector3 rot = transform.rotation.eulerAngles;

        // ��ü�� �ٶ󺸴� ������ ���� �÷��̾ �����ϵ��� ���� ����
        if (rot.y > 45 && rot.y < 135)
        {
            if (pos.x < playerpos.x) m_Right = true;
            else m_Left = true;
        }
        else if (rot.y > 135 && rot.y < 225)
        {
            if (pos.z > playerpos.z) m_Right = true;
            else m_Left = true;
        }
        else if (rot.y > 225 && rot.y < 315)
        {
            if (pos.x > playerpos.x) m_Right = true;
            else m_Left = true;
        }
        else
        {
            if (pos.z < playerpos.z) m_Right = true;
            else m_Left = true;
        }

        if (m_PassedTime < m_JumpCooldown)
        {
            m_PassedTime += Time.deltaTime;
            return;
        }

        // �÷��̾�� ���� ���̰� �� ��� ���� �Ǵ� ���� ����
        if (pos.y + 1f < playerpos.y)
        {
            m_Up = true;
            m_PassedTime = 0f;
        }
        else if (pos.y - 1f > playerpos.y)
        {
            m_Down = true;
            m_PassedTime = 0f;
        }
    }

    public float GetDistance()
    {
        if (m_Target == null) return float.MaxValue;

        return Vector3.Distance(transform.position, m_Target.transform.position);
    }
}