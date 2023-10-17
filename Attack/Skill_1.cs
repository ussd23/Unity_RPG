using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill_1 : Skill
{
    // 편집 변수
    [SerializeField]
    protected Transform GunEndPoint = null;
    [SerializeField]
    protected LineRenderer LinkLineCom = null;
    [SerializeField]
    protected GameObject HitParticle = null;
    [SerializeField]
    protected GameObject GunParticle = null;

    // 내부 변수
    protected float LineDelay = 0.4f;
    protected float CurrLineSec = 0f;
    protected RaycastHit m_hitinfo;

    private new void Awake()
    {
        BasicAwake();

        Transform tempcom = GameObject.FindWithTag("Skill_1").transform;
        m_UIImage = tempcom.GetChild(1).GetComponent<Image>();
        m_UIText = m_UIImage.GetComponentInChildren<Text>();
    }

    private new void Update()
    {
        base.Update();
        UIUpdate();

        if (CurrLineSec < LineDelay) UpdateLine();

        if (m_PassedTime < m_AttackCooldown) return;

        if (m_SkillLevel > 0 && Input.GetKey(KeyCode.X) && m_Info.UseEnergy(m_EnergyUsage)) Pressed();
    }

    // 궤적 관련 업데이트
    void UpdateLine()
    {
        CurrLineSec += Time.deltaTime;

        if (CurrLineSec > LineDelay)
        {
            LinkLineCom.gameObject.SetActive(false);
        }
    }

    // 스킬 이펙트
    protected void Effect(Vector3 endpos)
    {
        GameObject obj = GameObject.Instantiate(GunParticle, GunEndPoint.position, Quaternion.identity);
        GameObject obj2 = GameObject.Instantiate(HitParticle, endpos, Quaternion.identity);
        obj2.transform.LookAt(obj.transform.position + m_hitinfo.normal, Vector3.up);
        Destroy(obj, 2f);
        Destroy(obj2, 2f);

        CurrLineSec = 0f;
        LinkLineCom.gameObject.SetActive(true);

        LinkLineCom.SetPosition(0, GunEndPoint.position);
        LinkLineCom.SetPosition(1, endpos);
    }

    // 버튼을 누른 경우
    void Pressed()
    {
        Vector3 endpos = GunEndPoint.position + GunEndPoint.forward * 20f;

        // 전방에 적 탐색
        if (Physics.Raycast(GunEndPoint.position, GunEndPoint.forward, out m_hitinfo, 20, LayerMask.GetMask("Mob")))
        {
            if ("Mob" == m_hitinfo.transform.tag)
            {
                m_hitinfo.transform.GetComponent<Info>().SetDamage(GetAttackPower());
                endpos = m_hitinfo.point;
            }
        }

        Effect(endpos);
        Cooldown();
    }
}
