using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : GetI<ItemManager>
{
    private UI_Manager m_UIManager;
    public List<Sprite> m_Sprites;
    public Dictionary<int, Item> m_ItemList;

    public ItemObject m_ItemObject;
    public CurrencyObject m_CurrencyObject;

    private void Start()
    {
        m_UIManager = UI_Manager.GetInstance;

        ItemInit();
    }

    // ������ �̸����� �˻�
    public Item GetItem(string p_Name)
    {
        foreach (KeyValuePair<int, Item> item in m_ItemList)
        {
            if (item.Value.Name == null) continue;

            if (item.Value.Name == p_Name)
            {
                return item.Value;
            }
        }
        return null;
    }

    // ������ ����Ʈ ����
    public void ItemInit()
    {
        m_ItemList = new Dictionary<int, Item>();

        // Json ������ �Ľ�
        JsonReader.ItemReader();

        JsonReader.SlotLoad();
    }

    // ������ ���޿�
    public void GiveItem(Item p_Item, int p_Amount)
    {
        int amount = p_Amount;
        foreach (UI_Slot slot in m_UIManager.m_InventorySlots)
        {
            // 0���� �Ǹ� ���� Ż��
            if (amount == 0) break;

            // �ش� �������� �ִ� ���Կ� ���
            if (slot.m_Item == p_Item)
            {
                // ��� ������ ���ڸ�ŭ ���
                int available = p_Item.MaxStack - slot.m_Stack;
                if (amount > available)
                {
                    slot.m_Stack += available;
                    amount -= available;
                }

                // �ش� ������ �����ο� ���
                else
                {
                    slot.m_Stack += amount;
                    amount = 0;
                }
            }

            slot.UIUpdate();
        }

        if (amount != 0)
        {
            foreach (UI_Slot slot in m_UIManager.m_InventorySlots)
            {
                // 0���� �Ǹ� ���� Ż��
                if (amount == 0) break;

                // �� �����̳� �ش� �������� �ִ� ���Կ� ���
                if (slot.m_Item == null)
                {
                    if (slot.m_Item == null) slot.m_Item = p_Item;

                    // ��� ������ ���ڸ�ŭ ���
                    int available = p_Item.MaxStack - slot.m_Stack;
                    if (amount > available)
                    {
                        slot.m_Stack += available;
                        amount -= available;
                    }

                    // �ش� ������ �����ο� ���
                    else
                    {
                        slot.m_Stack += amount;
                        amount = 0;
                    }
                }

                slot.UIUpdate();
            }
        }

        m_UIManager.Save();

        if (amount > 0)
        {
            m_UIManager.OpenWindow_OK($"�κ��丮 ������ �����Ͽ� {p_Item.Name} ������ {amount}���� ȹ������ ���߽��ϴ�.", false);
        }
    }
}
