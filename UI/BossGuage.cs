using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossGuage : MonoBehaviour
{
    // 편집 변수
    [SerializeField]
    private GameObject m_Guage;

    private void Awake()
    {
        m_Guage = UI_Manager.GetInstance?.m_BossGuageBack;
        GetComponent<Info>().BossHP(m_Guage.transform.GetChild(0).GetComponent<Image>());
    }

    private void OnEnable()
    {
        m_Guage.SetActive(true);
    }
}