using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character : UI_Movable
{
    private List<Text> m_Texts;
    private Info m_PlayerInfo;

    private new void Start()
    {
        base.Start();
        m_Texts = new List<Text>();

        Transform tempcom = transform.GetChild(3);

        for (int i = 0; i < tempcom.childCount; ++i)
        {
            m_Texts.Add(tempcom.GetChild(i).GetComponent<Text>());
        }

        m_PlayerInfo = Info.PlayerInfo;
    }

    private void Update()
    {
        UIUpdate();
    }

    // 공격력, 방어력 정보 표시
    private void UIUpdate()
    {
        m_Texts[0].text = $"{m_PlayerInfo.m_Attack + m_PlayerInfo.m_EquipStats.Attack}";
        m_Texts[1].text = $"{m_PlayerInfo.m_Defense + m_PlayerInfo.m_EquipStats.Defense}";
    }
}
