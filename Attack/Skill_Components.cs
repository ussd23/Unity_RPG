using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Components : MonoBehaviour
{
    private List<Skill> m_SkillComponentList;
    private UI_SkillTree m_SkillTree;

    private void Awake()
    {
        m_SkillTree = UI_Manager.GetInstance.m_SkillTreeUI;
        if (m_SkillTree == null) return;

        m_SkillComponentList = new List<Skill>
        {
            GetComponent<Skill_Passive1>(),
            null, //m_SkillComponentList.Add(GetComponent<Skill_Passive2>());
            null, //m_SkillComponentList.Add(GetComponent<Skill_Passive3>());
            null, //m_SkillComponentList.Add(GetComponent<Skill_Passive4>());
            null, //m_SkillComponentList.Add(GetComponent<Skill_Passive5>());
            GetComponent<Skill_Dash>(),
            null, // Near Attack
            GetComponent<Skill_1>(),
            GetComponent<Skill_2>(),
            null, //m_SkillComponentList.Add(GetComponent<Skill_3>());
            null, //m_SkillComponentList.Add(GetComponent<Skill_4>());
            null, //m_SkillComponentList.Add(GetComponent<Skill_5>());
            null, //m_SkillComponentList.Add(GetComponent<Skill_6>());
            null //m_SkillComponentList.Add(GetComponent<Skill_7>());
        };

        m_SkillTree.InitSkillTree(this);
    }

    public Skill GetComponent(int i)
    {
        return m_SkillComponentList[i];
    }
}
