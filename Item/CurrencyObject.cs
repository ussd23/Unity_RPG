using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyObject : ItemObject
{
    public new Color[] m_Item;

    private new void Start()
    {
        m_Item = new Color[3];
        ColorUtility.TryParseHtmlString("#FFC90E", out m_Item[0]);
        ColorUtility.TryParseHtmlString("#C3C3C3", out m_Item[1]);
        ColorUtility.TryParseHtmlString("#784315", out m_Item[2]);

        m_ItemModel = transform.GetChild(0);
        m_ItemModel.GetComponent<MeshRenderer>().material.color =
            m_Amount >= 200 ? m_Item[0] : m_Amount >= 100 ? m_Item[1] : m_Item[2];
        m_JumpSpeed = m_Height;
        m_FallingTime = -0.15f * m_JumpSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Info.PlayerInfo.CurrencyChange(m_Amount);
            Destroy(gameObject);
        }
    }
}
