using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{

    [SerializeField]
    private bool m_isMove = true;
    [SerializeField, Tooltip("수평 이동과 수직 이동 중 하나만 사용하시오.")]
    public Vector3 m_Direction = new Vector3(0, 0, 0);
    [SerializeField]
    private float m_MoveDuration = 1f;
    [SerializeField]
    private bool m_isRepeat = false;
    [SerializeField]
    private bool m_isRest = false;
    private float m_PassedTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (!m_isMove) return;
        m_PassedTime += Time.deltaTime;

        if (m_isRepeat)
        {
            int tempval = (int)(m_PassedTime / m_MoveDuration);
            if (m_isRest)
            {
                if (tempval % 2 == 0)
                {
                    if (tempval % 4 == 0) transform.Translate(m_Direction * Time.deltaTime);
                    else transform.Translate(-m_Direction * Time.deltaTime);
                }
            }
            else
            {
                if (tempval % 2 == 0) transform.Translate(m_Direction * Time.deltaTime);
                else transform.Translate(-m_Direction * Time.deltaTime);
            }
        }
        else
        {
            if (m_PassedTime >= m_MoveDuration)
            {
                m_isMove = false;
                m_PassedTime = 0;
                return;
            }
            transform.Translate(m_Direction * Time.deltaTime);
        }
    }

    [ContextMenu("Move On")]
    public void SetMove()
    {
        m_isMove = true;
    }
}
