using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNearAttack : NearAttack
{
    // ���� ����
    [SerializeField]
    private Image m_UIImage = null;
    [SerializeField]
    private Text m_UIText = null;

    // ���� ����
    private List<GameObject> m_Targets = new List<GameObject>();
    private Color32 m_CooldownColor = new Color32(0, 0, 0, 191);
    private Color32 m_EnergyColor = new Color32(255, 0, 0, 191);

    private new void Update()
    {
        base.Update();
        UIUpdate();

        if (m_PassedTime < m_AttackCooldown) return;

        for (int i = m_Targets.Count - 1; i >= 0; --i)
        {
            GameObject target = m_Targets[i];
            if (target == null || target.GetComponent<Info>()?.isDie() == true) m_Targets.Remove(target);
        }

        if (Input.GetKey(KeyCode.Z) && m_Info.UseEnergy(m_EnergyUsage)) Pressed();
    }

    // ��ư�� ���� ���
    void Pressed()
    {
        Effect();
        Cooldown();

        foreach (GameObject target in m_Targets) Damage(target);
    }

    // UI ������Ʈ
    void UIUpdate()
    {
        float remain = m_AttackCooldown - m_PassedTime;

        if (m_UIImage == null && m_UIText == null) return;

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

    // ���� ���� ���� �� ������Ʈ
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Mob")
        {
            m_Targets.Add(other.transform.gameObject);
        }
    }

    // ���� �������� ���� ���� ��� null
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Mob")
        {
            m_Targets.Remove(other.transform.gameObject);
        }
    }
}
