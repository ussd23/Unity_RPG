using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // 편집 변수
    [SerializeField]
    private Transform m_Target = null;
    [SerializeField]
    private float m_LimitTop = float.MaxValue;
    [SerializeField]
    private float m_LimitBottom = 1f;
    [SerializeField]
    private float m_LerpT = 5f;

    private Transform m_DefaultTarget = null;

    void Awake()
    {
        if (m_Target == null) m_DefaultTarget = GameObject.FindWithTag("PlayerView").transform;
        else m_DefaultTarget = m_Target;
        DefaultTarget();
    }

    // 플레이어를 타겟으로 이동
    void Update()
    {
        if (m_Target == null) return;

        Vector3 temppos = m_Target.position;
        if (temppos.y < m_LimitBottom) temppos.y = m_LimitBottom;
        if (temppos.y > m_LimitTop) temppos.y = m_LimitTop;
        transform.position = Vector3.Lerp(transform.position, temppos, m_LerpT * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(transform.rotation, m_Target.rotation, m_LerpT * Time.deltaTime);
    }

    public void TargetChange(Transform p_target)
    {
        m_Target = p_target;
    }

    public void DefaultTarget()
    {
        m_Target = m_DefaultTarget;
    }
}
