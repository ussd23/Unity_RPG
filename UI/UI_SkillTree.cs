using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillTree : UI_Movable
{
    public Info m_PlayerInfo;
    public int m_SPPerLevel = 2;
    public int m_SP = 0;
    [SerializeField]
    private Text m_SPText;
    [SerializeField]
    public List<UI_SkillInfo> m_SkillInfos;

    private new void Start()
    {
        base.Start();
        m_PlayerInfo = Info.PlayerInfo;
    }

    public void InitSkillTree(Skill_Components p_com)
    {
        Transform skills = transform.GetChild(1);
        for (int i = 0; i < skills.childCount; ++i)
        {
            UI_SkillInfo skillcom = skills.GetChild(i).GetComponent<UI_SkillInfo>();
            skillcom.m_SkillNo = i;
            skillcom.SetSkillComponent(p_com.GetComponent(i));
            m_SkillInfos.Add(skillcom);
        }

        m_SP = 2;
        Load();
    }

    public void Update()
    {
        UIUpdate();
    }

    public void UIUpdate()
    {
        if (m_SPText != null)
        {
            m_SPText.text = $"{m_SP}";
        }
    }

    public bool is_SPRemains(int p_val)
    {
        if (m_SP >= p_val) return true;
        return false;
    }

    public void SPUp(int p_val)
    {
        m_SP += p_val;
        Save();
    }

    public bool SPCheck(int p_val)
    {
        foreach (UI_SkillInfo v in m_SkillInfos)
        {
            if (v.m_SkillLevel > 0 && v.m_RequirePlayerLevel > p_val)
            {
                return false;
            }
        }

        return true;
    }

    public void TooltipUpdate(int p_val)
    {
        UI_SkillInfo info = m_SkillInfos[p_val];
        string text = $"\n{info.m_SkillText}\n";

        if (info.m_SkillComponent != null && info.m_StatInfo)
        {
            float temp1 = info.m_SkillComponent.GetLevelPower();
            float temp2 = temp1 * info.m_SkillComponent.m_AttackPowerRange;
            if (temp1 != 0) text += $"\n공격력의 {temp1 - temp2}% ~ {temp1 + temp2}%의 피해\n";
            if (info.m_SkillComponent.m_EnergyUsage != 0) text += $"\n기력 {info.m_SkillComponent.m_EnergyUsage} 소모";
            if (info.m_SkillComponent.m_AttackCooldown != 0) text += $"\n{info.m_SkillComponent.m_AttackCooldown}초 후 재사용 가능\n";
        }

        text += $"\n요구 레벨: {info.m_RequirePlayerLevel}";

        if (info.m_SkillLevel == info.m_MaxSkillLevel)
        {
            text += $"\n\n최고 레벨";
        }
        else
        {
            text += $"\n\n스킬 레벨: {info.m_SkillLevel} / {info.m_MaxSkillLevel} (필요 SP: {info.m_RequireSP})";
        }

        if (info.m_RequireSkill != null)
        {
            text += $"\n\n선행 스킬: {info.m_RequireSkill.m_SkillName} {info.m_RequireSkillLevel} 레벨 이상";
        }

        m_Manager.TooltipUpdate(info.m_SkillName, text);
    }

    public void Save()
    {
        JsonReader.SkillTreeSave();
    }

    public void Load()
    {
        JsonReader.SkillTreeLoad();

        foreach (UI_SkillInfo info in m_SkillInfos)
        {
            info.SkillUpdate();
        }
    }
}
