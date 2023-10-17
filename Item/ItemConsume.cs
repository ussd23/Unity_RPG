using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemConsume : MonoBehaviour
{
    public KeyCode m_Key;
    public float m_AttackCooldown = 10f;
    private float m_PassedTime = 0f;
    private Info m_Target;
    private UI_Slot m_Slot;
    private Transform m_Cooldown;
    private Image m_UIImage;
    private Text m_UIText;

    private void Awake()
    {
        m_Slot = GetComponent<UI_Slot>();
        m_Cooldown = transform.GetChild(3);
        m_UIImage = m_Cooldown.GetComponent<Image>();
        m_UIText = m_Cooldown.GetChild(0).GetComponent<Text>();
        m_PassedTime = m_AttackCooldown;
    }

    private void Start()
    {
        m_Target = Info.PlayerInfo;
    }

    private void Update()
    {
        UIUpdate();
        if (m_PassedTime < m_AttackCooldown)
        {
            m_PassedTime += Time.deltaTime;
            return;
        }

        if (m_Slot.m_Item == null)
        {
            return;
        }

        if (m_Target.isDie()) return;

        if (Input.GetKey(m_Key)) Pressed();
    }

    private void UIUpdate()
    {
        float remain = m_AttackCooldown - m_PassedTime;

        // 사용 가능한 경우
        if (remain <= 0)
        {
            m_UIImage.fillAmount = 0;
            m_UIText.text = "";
        }

        // 쿨타임인 경우
        else
        {
            m_UIImage.fillAmount = remain / m_AttackCooldown;
            m_UIText.text = $"{(int)remain + 1}";
        }
    }

    private void Pressed()
    {
        Cooldown();
        Item item = m_Slot.m_Item;

        // 아이템을 사용할 수 있는 지의 여부 확인 후 사용
        if (m_Slot.UseItem(1))
        {
            item.RunScript(m_Target);
        }
    }

    private void Cooldown()
    {
        m_PassedTime = 0f;
    }
}
