using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearAttack : Attack
{
    // ���� ����
    [SerializeField]
    protected GameObject HitParticle = null;

    // ����Ʈ
    protected void Effect()
    {
        GameObject obj = GameObject.Instantiate(HitParticle, transform.position, transform.rotation);
        Destroy(obj, 2f);
    }
}
