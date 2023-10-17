using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UI_Movable
{
    [SerializeField]
    private Text m_Text;
    private Info m_PlayerInfo;

    private new void Start()
    {
        m_PlayerInfo = Info.PlayerInfo;

        base.Start();
        UIUpdate();
    }

    private void Update()
    {
        UIUpdate();
    }

    public void UIUpdate()
    {
        m_Text.text = $"{m_PlayerInfo.m_Currency}";
    }
}