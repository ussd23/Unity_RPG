using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �������� ����
public enum Type
{
    None,

    // Equipments
    ���� = 100,
    ����,
    ����,
    �尩,
    �Ź�,

    �Һ� = 200,

    ��� = 300,

    Ư�� = 400,
}

// �������� ���
public enum Grade
{
    �Ϲ�,
    ����,
    ����Ʈ,
    ����ũ,
    ������,
}

// ������ ����
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

    // ������
    public Item(int p_Code, string p_Name, Sprite p_Sprite, string p_Text = null, Type p_Type = Type.���, Grade p_Grade = Grade.�Ϲ�, int p_MaxStack = 100, string p_Function = null, object[] p_Parameters = null)
    {
        Code = p_Code;
        Name = p_Name;
        Sprite = p_Sprite;
        Text = p_Text;
        Type = p_Type;
        Grade = p_Grade;

        if (Type >= Type.���� && Type < Type.�Һ�)
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

    // �ش� �����۰� ���õ� ��ũ��Ʈ�� �ִ� ��� �����ϴ� ����
    public void RunScript(Info p_Info)
    {
        if (Script == null) return;

        Script.Perform(Function, p_Info, Parameters);
    }
}
