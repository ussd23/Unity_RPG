using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ItemScript
{
    Info m_Info;

    // 메소드 이름을 string으로 받아와 실행하도록 함
    public void Perform(string p_FunctionName, Info p_Info, object[] p_val)
    {
        m_Info = p_Info;

        MethodInfo methodInfo = typeof(ItemScript).GetMethod(p_FunctionName);

        if (methodInfo != null) methodInfo.Invoke(this, p_val);
    }

    // 포션
    public void C001(int p_val)
    {
        m_Info.HealthRegen(p_val);
    }

    // 장비 스탯 적용
    public void E001(int p_val1, int p_val2)
    {
        m_Info.m_EquipStats.Attack += p_val1;
        m_Info.m_EquipStats.Defense += p_val2;
    }
}
