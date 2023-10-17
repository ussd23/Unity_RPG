using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Passive1 : Skill
{
    // 편집 변수
    [SerializeField]
    private float m_HealthHeal = 1f;
    [SerializeField]
    private float m_EnergyHeal = 5f;
    [SerializeField]
    private Info m_PlayerInfo;

    private int m_PastLevel = 1;

    new void Awake()
    {
        m_PlayerInfo = GetComponent<Info>();
        BasicAwake();
    }

    private new void Update()
    {
        base.Update();
        if (m_Switch) SwitchUpdate();

        if (GetComponent<Info>().isDie()) return;
        if (m_PassedTime < m_AttackCooldown) return;

        m_Info.EnergyRegen(m_EnergyHeal);
        m_Info.HealthRegen(m_HealthHeal);

        Cooldown();
    }

    public void SwitchUpdate()
    {
        m_PlayerInfo.m_MaxHealth = 100 + (m_SkillLevel - 1) * 10;
        m_PlayerInfo.m_MaxEnergy = 100 + (m_SkillLevel - 1) * 10;
        m_PlayerInfo.HealthRegen((m_SkillLevel - m_PastLevel) * 10);
        m_PlayerInfo.EnergyRegen((m_SkillLevel - m_PastLevel) * 10);
        m_PastLevel = m_SkillLevel;
    }
}
