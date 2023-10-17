using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobGauge : MonoBehaviour
{
    // ���� ����
    [SerializeField]
    private GameObject m_OriginalGuage;

    // ���� ����
    private Camera m_Camera;
    private Info m_Info;
    private MobMover m_Mover;
    private GameObject m_Gauge;
    private Vector3 m_Position = Vector3.down * 0.5f;

    private void Awake()
    {
        m_OriginalGuage = UI_Manager.GetInstance.m_MobGuageBack;
    }

    // �ν���Ʈ ������ ����
    private void OnEnable()
    {
        m_Camera = Camera.main;
        m_Info = GetComponentInParent<Info>();
        m_Mover = GetComponent<MobMover>();
        m_Gauge = Instantiate(m_OriginalGuage);

        m_Info.SetHealthBar(m_Gauge.transform.GetChild(0).GetComponent<Image>());
        m_Gauge.transform.SetParent(UI_Manager.GetInstance.transform.GetChild(0));
        m_Gauge.transform.localScale = Vector3.one;
    }

    // ������ ��ġ ����
    private void Update()
    {
        if (m_Mover.GetDistance() < 15) m_Gauge.SetActive(true);
        else m_Gauge.SetActive(false);

        m_Gauge.transform.position = m_Camera.WorldToScreenPoint(transform.position + m_Position);
    }

    // ������ ����
    private void OnDestroy()
    {
        Destroy(m_Gauge);
    }
}
