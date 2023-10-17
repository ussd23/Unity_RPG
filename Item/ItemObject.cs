using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : Mover
{
    public Item m_Item;
    public int m_Amount;
    public float m_Torque;
    public Transform m_ItemModel;
    protected float m_Height = 1f;
    private float m_Timer = 30f;

    protected void Start()
    {
        m_ItemModel = transform.GetChild(0);
        m_ItemModel.GetComponent<MeshRenderer>().material.mainTexture = m_Item.Sprite.texture;
        m_JumpSpeed = m_Height;
        m_FallingTime = -0.25f * m_JumpSpeed;
    }

    private void Update()
    {
        MoveUpdate();
        ModelUpdate();
        TimeUpdate();
    }

    private void MoveUpdate()
    {
        m_MovementSpeed = Mathf.Abs(m_Torque);
        if (m_Jumpable)
        {
            m_JumpSpeed *= 0.7f;
        }

        BasisUpdate(m_JumpSpeed >= 0.1f ? true : false, false, m_Torque < 0 && m_JumpSpeed >= 0.1f ? true : false, m_Torque > 0 && m_JumpSpeed >= 0.1f ? true : false);
    }

    private void ModelUpdate()
    {
        Vector3 rot = m_ItemModel.rotation.eulerAngles;
        rot.y += Time.deltaTime * 180;
        if (rot.y > 180) rot.y -= 360;
        m_ItemModel.rotation = Quaternion.Euler(rot);
    }

    private void TimeUpdate()
    {
        m_Timer -= Time.deltaTime;
        if (m_Timer < 0) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            ItemManager.GetInstance.GiveItem(m_Item, m_Amount);
            Destroy(gameObject);
        }
    }
}
