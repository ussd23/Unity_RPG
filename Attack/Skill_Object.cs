using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Object : MonoBehaviour
{
    // 편집 변수
    public float m_AttackPower = 0f;
    public Skill_2 m_ParentAttack;
    [SerializeField]
    private float m_Duration = 5f;

    // 내부 변수
    private Vector3 m_Direction = new Vector3(0, 10f, 0);

    private void Update()
    {
        m_Duration -= Time.deltaTime;
        transform.Translate(m_Direction * Time.deltaTime);

        if (m_Duration < 0) Destroy(gameObject);
    }

    // 적이 
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Mob")
        {
            other.GetComponent<Info>()?.SetDamage(m_ParentAttack.GetAttackPower());
        }
    }
}
