using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템의 종류
public enum Type
{
    None,

    // Equipments
    무기 = 100,
    상의,
    하의,
    장갑,
    신발,

    소비 = 200,

    재료 = 300,

    특수 = 400,
}

// 아이템의 등급
public enum Grade
{
    일반,
    레어,
    엘리트,
    유니크,
    레전드,
}

// 아이템 정보
public class Item
{
    public int Code;
    public string Name;
    public Sprite Sprite;
    public string Text;
    public Type Type;
    public Grade Grade;
    public int MaxStack;
    public string Function;
    public ItemScript Script;
    public object[] Parameters;

    // 생성자
    public Item(int p_Code, string p_Name, Sprite p_Sprite, string p_Text = null, Type p_Type = Type.재료, Grade p_Grade = Grade.일반, int p_MaxStack = 100, string p_Function = null, object[] p_Parameters = null)
    {
        Code = p_Code;
        Name = p_Name;
        Sprite = p_Sprite;
        Text = p_Text;
        Type = p_Type;
        Grade = p_Grade;

        if (Type >= Type.무기 && Type < Type.소비)
        {
            MaxStack = 1;
        }
        else
        {
            MaxStack = p_MaxStack;
        }

        if (p_Function != null)
        {
            Script = new ItemScript();
            Function = p_Function;
        }

        if (p_Parameters != null)
        {
            Parameters = p_Parameters;
        }
    }

    // 해당 아이템과 관련된 스크립트가 있는 경우 실행하는 내용
    public void RunScript(Info p_Info)
    {
        if (Script == null) return;

        Script.Perform(Function, p_Info, Parameters);
    }
}
