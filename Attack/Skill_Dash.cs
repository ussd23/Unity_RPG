using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill_Dash : Skill
{
    // 내부 변수
    private PlayerMover m_PlayerMover;

    private new void Awake()
    {
        BasicAwake();
        m_PlayerMover = GetComponent<PlayerMover>();

        Transform tempcom = GameObject.FindWithTag("Skill_Dash").transform;
        m_UIImage = tempcom.GetChild(1).GetComponent<Image>();
        m_UIText = m_UIImage.GetComponentInChildren<Text>();
    }

    private new void Update()
    {
        base.Update();
        UIUpdate();

        if (m_PassedTime < m_AttackCooldown) return;

        if (m_SkillLevel > 0 && Input.GetKey(KeyCode.LeftShift) && m_Info.UseEnergy(m_EnergyUsage)) Pressed();
    }

    // 버튼을 누른 경우
    void Pressed()
    {
        m_PlayerMover.DashSkillUse();
        Cooldown();
    }
}
