using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill_2 : Skill
{
    // 편집 변수
    [SerializeField]
    private GameObject m_Object = null;
    [SerializeField]
    private GameObject m_Point;

    private new void Awake()
    {
        BasicAwake();

        Transform tempcom = GameObject.FindWithTag("Skill_2").transform;
        m_UIImage = tempcom.GetChild(1).GetComponent<Image>();
        m_UIText = m_UIImage.GetComponentInChildren<Text>();
    }

    private new void Update()
    {
        base.Update();
        UIUpdate();

        if (m_PassedTime < m_AttackCooldown) return;

        if (m_SkillLevel > 0 && Input.GetKey(KeyCode.C) && m_Info.UseEnergy(m_EnergyUsage)) Pressed();
    }

    // 버튼을 누른 경우
    void Pressed()
    {
        GameObject obj = Instantiate(m_Object);
        obj.transform.position = m_Point.transform.position;
        Vector3 temprot = m_Point.transform.rotation.eulerAngles;
        temprot.x += 90;
        obj.transform.rotation = Quaternion.Euler(temprot);
        obj.GetComponent<Skill_Object>().m_ParentAttack = this;
        obj.SetActive(true);

        Cooldown();
    }
}
