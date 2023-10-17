using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkill_1 : Attack
{
    // 편집 변수
    [SerializeField]
    private GameObject[] m_Mobs;

    private new void Update()
    {
        base.Update();

        if (m_PassedTime < m_AttackCooldown) return;

        StartCoroutine(Pressed());
    }

    // 몬스터 소환
    IEnumerator Pressed()
    {
        Cooldown();
        GetComponent<MobMover>().m_Movable = false;

        // 소환하는 동안 움직일 수 없도록 설정
        yield return new WaitForSeconds(1f);

        int Count = Random.Range(1, 5);

        for (int i = 0; i < Count; ++i)
        {
            GameObject obj = Instantiate(m_Mobs[i % m_Mobs.Length]);
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.transform.Translate(new Vector3(0, 5, (i + 1) * Mathf.Pow(-1, i + 1)));
            obj.SetActive(true);
        }

        yield return new WaitForSeconds(1f);
        GetComponent<MobMover>().m_Movable = true;
    }
}
