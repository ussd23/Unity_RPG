using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearAttack : Attack
{
    // 편집 변수
    [SerializeField]
    protected GameObject HitParticle = null;

    // 이펙트
    protected void Effect()
    {
        GameObject obj = GameObject.Instantiate(HitParticle, transform.position, transform.rotation);
        Destroy(obj, 2f);
    }
}
