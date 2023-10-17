using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : Attack
{
    // ���� ����
    public float m_AddPowerPerLevel = 0;
    [SerializeField]
    protected Image m_UIImage = null;
    [SerializeField]
    protected Text m_UIText = null;
    [SerializeField]
    protected UI_SkillInfo m_SkillInfo = null;
    protected int m_SkillLevel = 0;
    protected bool m_Switch = false;

    // ���� ����
    protected Color32 m_CooldownColor = new Color32(0, 0, 0, 191);
    protected Color32 m_EnergyColor = new Color32(255, 0, 0, 191);

    public void SkillLevelUpdate(int p_val)
    {
        m_SkillLevel = p_val;
        m_Switch = true;
    }

    // UI ������Ʈ
    protected void UIUpdate()
    {
        float remain = m_AttackCooldown - m_PassedTime;

        if (m_SkillLevel <= 0)
        {
            m_UIImage.fillAmount = 1;
            m_UIImage.color = m_CooldownColor;
            m_UIText.text = "�̽���";
            return;
        }

        // ��� ������ ���
        if (remain <= 0)
        {
            m_UIImage.fillAmount = 0;
            m_UIText.text = "";
        }

        // ��Ÿ���� ���
        else
        {
            m_UIImage.fillAmount = remain / m_AttackCooldown;
            m_UIText.text = $"{(int)remain + 1}";
        }

        // ����� ������ ���
        if (!m_Info.isUsable(m_EnergyUsage))
        {
            m_UIImage.fillAmount = 1;
            m_UIImage.color = m_EnergyColor;
        }
        else
        {
            m_UIImage.color = m_CooldownColor;
        }
    }
    public float GetLevelPower()
    {
        int level = m_SkillLevel - 1;
        if (m_SkillLevel == 0) level = 0;
        return m_AttackPower + m_AddPowerPerLevel * level;
    }

    public new float GetAttackPower()
    {
        return GetLevelPower() * GetPowerRange();
    }
}
