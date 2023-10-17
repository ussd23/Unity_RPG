using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobNearAttack : NearAttack
{
    protected new void Update()
    {
        base.Update();
    }

    // ��ó�� �÷��̾ ���� ��� ����
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (m_PassedTime < m_AttackCooldown ||
                !m_Info.enabled) return;

            Damage(other.transform.gameObject);
            Effect();
            Cooldown();
        }
    }
}
