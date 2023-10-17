using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    // ���� ����
    public bool m_Movable = true;
    [SerializeField, Range(3f, 15f)]
    protected float m_MovementSpeed = 5f;
    [SerializeField, Range(1f, 3f), Tooltip("��� ���¿��� Value�踸ŭ �̵� �ӵ��� �����Ѵ�.")]
    protected float m_DashSpeed = 1.5f;
    [SerializeField, Range(0.5f, 2f)]
    protected float m_JumpSpeed = 1f;
    [SerializeField]
    protected Animator m_Model = null;
    [SerializeField]
    protected Transform m_ModelTransform = null;
    [SerializeField]
    protected bool m_MoveReq = false;

    // ���� ����
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

    // ��/���� ����, ���� ���� ������Ʈ
    void JumpUpdate(bool up, bool down)
    {
        if (m_MoveReq)
        {
            m_FallingTime = 0;
            return;
        }

        m_TempMove.Set(0, 0, 0);

        // ���� ������ ������ ��� ���� ����
        if (up && m_Jumpable)
        {
            m_FallingTime = -0.25f * m_JumpSpeed;
            m_Jumpable = false;
        }

        // ���� ������ ������ ��� ���� ����
        if (down && m_Jumpable)
        {
            // ������ �� �ִ� �ٴ��� �ִ� �� ����
            if (Physics.Raycast(transform.position - 0.1f * Vector3.up,
                -Vector3.up, Mathf.Infinity, LayerMask.GetMask("LevelLine")))
            {
                m_TempMove.y = -0.1f;
                m_Jumpable = false;
                transform.Translate(m_TempMove);
                return;
            }
        }

        // ���� ���� ó�� (���� �� �Ǵ� ���� ��)
        if (Physics.Raycast(transform.position,
            -Vector3.up, out m_hitinfo, Mathf.Infinity, LayerMask.GetMask("LevelLine")))
        {
            LineUpdate();

            // ���� �ӵ� ����
            if (m_HeightFromLine > -1f)
            {
                // ���� �ӵ� ����
                if (m_FallingTime < m_MaxFallingSpeed)
                {
                    m_FallingTime += Time.deltaTime;
                }
                // �ִ� ���� �ӵ��� �Ǹ� �� �̻� �������� ����
                else
                {
                    m_FallingTime = m_MaxFallingSpeed;
                }
                m_TempMove.y = -m_FallingTime * (Time.deltaTime * 100);
            }

            // �ٴڿ� ����� ���
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

        // ��ġ �̵�
        transform.Translate(m_TempMove);

        m_isLineExists = false;
        if (m_hitinfo.transform == null) m_isLineExists = true;
    }

    // ��/���� �̵� ���� ������Ʈ
    void MoveUpdate(bool left, bool right)
    {
        bool pressed = false;
        int direction = 0;
        m_TempMove.Set(0, 0, 0);

        // ��/�� �̵�
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

        // �̵��� �� ���� ��� ����
        if (!pressed) return;

        // �ִϸ��̼� ����
        m_isWalking = true;

        // ���� ��ġ ���
        Vector3 temppos = transform.position;

        // y�� ����
        if (m_Jumpable)
        {
            // ������ ����
            float angle = m_DependedLine.rotation.eulerAngles.x;
            if (angle > 180) angle -= 360;

            // �ö󰡴� ������ ��� y�� ����
            if (direction * angle < 0) m_TempMove.y = Mathf.Abs(m_TempMove.z) * Mathf.Sin(Mathf.Abs(angle * 0.03f));

            // �̼��� ���� ����
            if (m_TempMove.y < 0.005f) m_TempMove.y = 0.005f;
        }

        // �̵�
        transform.Translate(m_TempMove);

        // ��/�� �̵��� ���� �ٴ��� �ִ� ���� �� ����
        if (Physics.Raycast(transform.position,
            -Vector3.up, out m_hitinfo, Mathf.Infinity, LayerMask.GetMask("LevelLine")))
        {
            LineUpdate();
        }

        // �̵� �Ұ����� ��� ���� ��ġ�� ����
        else
        {
            transform.position = temppos;
        }
    }

    // ���� ���� ������Ʈ
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

    // �ִϸ��̼� ������Ʈ
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

    // �ٴ� ������Ʈ
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
            // ȸ�� ������ x ����
            temprot.x = 0;
            transform.rotation = Quaternion.Euler(temprot);
        }

    }

    // ���� ��ġ�ϵ��� ��ġ ����
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

    // �̵��� �� ���� ��ġ�� �ִ� ��� ����� �ٴ����� �̵�
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