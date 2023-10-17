using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // ���� ����
    public float m_EnergyUsage = 0f;
    public float m_AttackCooldown = 3f;
    [Tooltip("���� �� ���ط�: ���ݷ� * ��ġ(%)")]
    public float m_AttackPower = 10f;
    [Range(0, 0.5f)]
    public float m_AttackPowerRange = 0.2f;
    
    // ���� ����
    protected Info m_Info;
    protected float m_PassedTime = 0f;

    protected void Awake()
    {
        BasicAwake();
    }

    protected void BasicAwake()
    {
        m_Info = GetComponentInParent<Info>();

        m_PassedTime = m_AttackCooldown;
    }

    // ��Ÿ�� ���� ������Ʈ
    protected void Update()
    {
        if (m_PassedTime < m_AttackCooldown) m_PassedTime += Time.deltaTime;
    }

    // ���� ����
    protected void Damage(GameObject gameObject)
    {
        if (m_Info.isDie()) return;
        gameObject.GetComponent<Info>()?.SetDamage(GetAttackPower());
    }

    protected void Cooldown()
    {
        m_PassedTime = 0f;
    }

    public float GetAttackPower()
    {
        float attack = m_Info.m_Attack + m_Info.m_EquipStats.Attack;
        return attack * (m_AttackPower / 100) * GetPowerRange();
    }

    public float GetPowerRange()
    {
        return Random.Range(1 - m_AttackPowerRange, 1 + m_AttackPowerRange);
    }
}
