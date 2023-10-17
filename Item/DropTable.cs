using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTable : MonoBehaviour
{
    // 드랍되는 아이템 정보
    [System.Serializable]
    public struct DropItem
    {
        public int index;
        public string name;
        public int amount;
        [Range(0, 100)]
        public float probability;
    };

    [SerializeField]
    private List<DropItem> m_DropItems;
    private Info m_Info;
    private ItemManager m_ItemManager;

    private void Start()
    {
        m_ItemManager = ItemManager.GetInstance;
        m_Info = GetComponent<Info>();
    }

    // DropItems에 있는 모든 아이템을 각 확률에 따라 드랍 (몬스터의 사망 처리 시점에 호출됨)
    public void ItemDrop()
    {
        Vector3 pos = transform.position;
        pos.y += 0.5f;

        int torque = 0;
        foreach (DropItem item in m_DropItems)
        {
            if (Random.Range(0, 100) > item.probability) continue;

            Item comp;
            if (m_ItemManager.m_ItemList[item.index] != null)
            {
                comp = m_ItemManager.m_ItemList[item.index];
            }
            else if (m_ItemManager.GetItem(item.name) != null)
            {
                comp = m_ItemManager.GetItem(item.name);
            }
            else continue;

            ItemObject obj = Instantiate(m_ItemManager.m_ItemObject);
            obj.transform.position = pos;
            obj.m_Item = comp;
            obj.m_Amount = item.amount;
            obj.m_Torque = torque % 2 == 0 ? torque / -2 : (torque - 1) / 2 + 1;    // 아이템을 흩뿌리는 방향
            obj.gameObject.SetActive(true);

            ++torque;
        }
    }

    public void CurrencyDrop()
    {
        Vector3 pos = transform.position;
        pos.y += 0.5f;

        int currency = (int)(m_Info.m_Currency * Random.Range(0.8f, 1.2f));
        int amount = currency / 100;
        int torque = 0;
        int count = Random.Range(2, amount < 4 ? 4 : amount > 10 ? 10 : amount);
        currency /= count;
        for (int i = 0; i < count; ++i)
        {
            CurrencyObject obj = Instantiate(m_ItemManager.m_CurrencyObject);
            obj.transform.position = pos;
            obj.m_Amount = currency;
            obj.m_Torque = torque % 2 == 0 ? (torque * 0.6f) / -2 : (torque * 0.6f - 0.6f) / 2 + 0.6f;    // 아이템을 흩뿌리는 방향
            obj.gameObject.SetActive(true);

            ++torque;
        }
    }
}
