using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    // 편집 변수
    public bool m_Movable = true;
    [SerializeField, Range(3f, 15f)]
    protected float m_MovementSpeed = 5f;
    [SerializeField, Range(1f, 3f), Tooltip("대시 상태에서 Value배만큼 이동 속도가 증가한다.")]
    protected float m_DashSpeed = 1.5f;
    [SerializeField, Range(0.5f, 2f)]
    protected float m_JumpSpeed = 1f;
    [SerializeField]
    protected Animator m_Model = null;
    [SerializeField]
    protected Transform m_ModelTransform = null;
    [SerializeField]
    protected bool m_MoveReq = false;

    // 내부 변수
    protected Vector3 m_TempMove = new Vector3();
    protected float m_MaxFallingSpeed = 0.25f;
    protected float m_FallingTime = 0f;
    protected bool m_Jumpable = false;
    protected bool m_isWalking = false;
    protected bool m_isDashing = false;

    public Transform m_DependedLine = null;
    protected Vector3 m_DependedLinePoint = new Vector3();
    protected float m_HeightFromLine = 0f;
    protected RaycastHit m_hitinfo;
    protected bool m_isLineExists = false;
    protected bool m_LineUpdate = true;

    protected void BasisUpdate(bool up, bool down, bool left, bool right)
    {
        if (!m_Movable && !m_MoveReq)
        {
            up = false;
            down = false;
            left = false;
            right = false;
        }
        JumpUpdate(up, down);
        MoveUpdate(left, right);
        RotationUpdate();
        AnimationUpdate();

        PosRevision();

        //Debug.DrawLine(m_DependedLine.position, m_DependedLine.position + Vector3.up, Color.green, 0.1f);
        //Debug.DrawLine(transform.position, transform.position - Vector3.up, Color.red, 0.1f);
    }

    // 상/하향 점프, 높이 관련 업데이트
    void JumpUpdate(bool up, bool down)
    {
        if (m_MoveReq)
        {
            m_FallingTime = 0;
            return;
        }

        m_TempMove.Set(0, 0, 0);

        // 점프 가능한 상태일 경우 상향 점프
        if (up && m_Jumpable)
        {
            m_FallingTime = -0.25f * m_JumpSpeed;
            m_Jumpable = false;
        }

        // 점프 가능한 상태일 경우 하향 점프
        if (down && m_Jumpable)
        {
            // 내려갈 수 있는 바닥이 있는 지 판정
            if (Physics.Raycast(transform.position - 0.1f * Vector3.up,
                -Vector3.up, Mathf.Infinity, LayerMask.GetMask("LevelLine")))
            {
                m_TempMove.y = -0.1f;
                m_Jumpable = false;
                transform.Translate(m_TempMove);
                return;
            }
        }

        // 높이 관련 처리 (점프 중 또는 낙하 중)
        if (Physics.Raycast(transform.position,
            -Vector3.up, out m_hitinfo, Mathf.Infinity, LayerMask.GetMask("LevelLine")))
        {
            LineUpdate();

            // 낙하 속도 관련
            if (m_HeightFromLine > -1f)
            {
                // 낙하 속도 증가
                if (m_FallingTime < m_MaxFallingSpeed)
                {
                    m_FallingTime += Time.deltaTime;
                }
                // 최대 낙하 속도가 되면 더 이상 가속하지 않음
                else
                {
                    m_FallingTime = m_MaxFallingSpeed;
                }
                m_TempMove.y = -m_FallingTime * (Time.deltaTime * 100);
            }

            // 바닥에 닿았을 경우
            if (m_HeightFromLine < -m_TempMove.y)
            {
                if (m_DependedLine.GetComponent<DropLine>()?.m_isDropLine == true && m_Movable)
                {
                    m_Movable = false;
                    StartCoroutine(RePosition());
                }

                m_TempMove.y = 0;
                m_FallingTime = 0;
                transform.Translate(m_TempMove);
                m_Jumpable = true;

                return;
            }
        }

        // 위치 이동
        transform.Translate(m_TempMove);

        m_isLineExists = false;
        if (m_hitinfo.transform == null) m_isLineExists = true;
    }

    // 좌/우향 이동 관련 업데이트
    void MoveUpdate(bool left, bool right)
    {
        bool pressed = false;
        int direction = 0;
        m_TempMove.Set(0, 0, 0);

        // 좌/우 이동
        if (left && right)
        {
            left = false;
            right = false;
        }
        else if (left)
        {
            direction = -1;
            pressed = true;
        }
        else if (right)
        {
            direction = 1;
            pressed = true;
        }
        m_TempMove.z = direction * m_MovementSpeed * Time.deltaTime;
        if (m_isDashing) m_TempMove.z *= m_DashSpeed;

        // 이동할 수 없는 경우 리턴
        if (!pressed) return;

        // 애니메이션 관련
        m_isWalking = true;

        // 현재 위치 기억
        Vector3 temppos = transform.position;

        // y값 보정
        if (m_Jumpable)
        {
            // 지형의 기울기
            float angle = m_DependedLine.rotation.eulerAngles.x;
            if (angle > 180) angle -= 360;

            // 올라가는 방향인 경우 y값 보정
            if (direction * angle < 0) m_TempMove.y = Mathf.Abs(m_TempMove.z) * Mathf.Sin(Mathf.Abs(angle * 0.03f));

            // 미세한 오차 방지
            if (m_TempMove.y < 0.005f) m_TempMove.y = 0.005f;
        }

        // 이동
        transform.Translate(m_TempMove);

        // 좌/우 이동할 곳이 바닥이 있는 곳인 지 판정
        if (Physics.Raycast(transform.position,
            -Vector3.up, out m_hitinfo, Mathf.Infinity, LayerMask.GetMask("LevelLine")))
        {
            LineUpdate();
        }

        // 이동 불가능한 경우 원래 위치로 변경
        else
        {
            transform.position = temppos;
        }
    }

    // 보는 방향 업데이트
    void RotationUpdate()
    {
        if (m_ModelTransform == null) return;

        Vector3 tempmodel = m_ModelTransform.localRotation.eulerAngles;

        if (m_TempMove.z > 0)
        {
            tempmodel.y = 0;
        }
        else if (m_TempMove.z < 0)
        {
            tempmodel.y = 180f;
        }

        m_ModelTransform.localRotation = Quaternion.Euler(tempmodel);
    }

    // 애니메이션 업데이트
    void AnimationUpdate()
    {
        if (m_Model == null) return;

        if (m_isWalking && m_Movable)
        {
            m_Model.SetBool("IsWalking", true);
        }
        else
        {
            m_Model.SetBool("IsWalking", false);
        }

        m_isWalking = false;
    }

    // 바닥 업데이트
    void LineUpdate()
    {
        if (m_DependedLine != m_hitinfo.transform)
        {
            m_Jumpable = false;
        }

        if (m_hitinfo.transform == null) return;

        m_HeightFromLine = m_hitinfo.distance;

        if (!m_LineUpdate) return;

        if (m_DependedLine != m_hitinfo.transform ||
            m_DependedLine.rotation.eulerAngles != transform.rotation.eulerAngles)
        {
            m_DependedLine = m_hitinfo.transform;
            m_DependedLinePoint = m_DependedLine.transform.position;
            Vector3 temprot = m_DependedLine.rotation.eulerAngles;
            // 회전 값에서 x 무시
            temprot.x = 0;
            transform.rotation = Quaternion.Euler(temprot);
        }

    }

    // 선상에 위치하도록 위치 보정
    void PosRevision()
    {
        if (m_DependedLine == null) {
            SetClosestLine();
            return;
        }

        m_LineUpdate = true;
        Vector3 pastpos = transform.position;
        Vector3 temppos = m_DependedLine.position;
        Vector3 temprot = m_DependedLine.rotation.eulerAngles;
        float templen = m_DependedLine.lossyScale.z;
        float additionlen = 0f;
        Elevator tempcomp = m_DependedLine.GetComponent<Elevator>();
        if (tempcomp != null)
        {
            if (temppos != m_DependedLinePoint && m_Jumpable)
            {
                if (tempcomp.m_Direction.x == 0 &&
                    tempcomp.m_Direction.y == 0)
                {
                    additionlen = Vector3.Distance(temppos, m_DependedLinePoint);
                    if (Vector3.Distance(m_DependedLinePoint +
                        Quaternion.Euler(temprot) * Vector3.forward * additionlen,
                        temppos) > additionlen)
                    {
                        additionlen *= -1;
                    }
                }
            }
        }
        m_DependedLinePoint = temppos;

        Vector3 maxpos = temppos + Quaternion.Euler(temprot) * Vector3.forward * templen;
        maxpos.y = pastpos.y;
        templen = Vector3.Distance(temppos, maxpos);

        float tempdis = Vector3.Distance(temppos, pastpos) + additionlen;
        if (tempdis > templen) tempdis = templen;

        if (pastpos == temppos) return;

        Vector3 resultrot = Quaternion.LookRotation(pastpos - temppos).eulerAngles;
        if (m_Jumpable && m_isLineExists) resultrot.x = temprot.x;
        resultrot.y = temprot.y;
        resultrot.z = 0;
        temppos = temppos + Quaternion.Euler(resultrot) * Vector3.forward * tempdis;
        temppos.y += 0.001f;
        transform.position = temppos;

    }

    // 이동할 수 없는 위치에 있는 경우 가까운 바닥으로 이동
    void SetClosestLine()
    {
        m_DependedLine = GameObject.FindWithTag("LevelLine").transform;
        transform.position = m_DependedLine.position + Vector3.up * 0.1f + m_DependedLine.rotation * Vector3.forward * 0.1f;
    }

    IEnumerator RePosition()
    {
        DropLine tempcomp = m_DependedLine.GetComponent<DropLine>();
        yield return new WaitForSeconds(3f);
        m_Movable = true;
        transform.GetComponent<Info>().SetDamage(tempcomp.m_Damage);
        transform.position = tempcomp.m_RevPosition.position;
    }
}