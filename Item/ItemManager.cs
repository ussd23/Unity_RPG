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

    // 아이템 이름으로 검색
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

    // 아이템 리스트 생성
    public void ItemInit()
    {
        m_ItemList = new Dictionary<int, Item>();

        // Json 데이터 파싱
        JsonReader.ItemReader();

        JsonReader.SlotLoad();
    }

    // 아이템 지급용
    public void GiveItem(Item p_Item, int p_Amount)
    {
        int amount = p_Amount;
        foreach (UI_Slot slot in m_UIManager.m_InventorySlots)
        {
            // 0개가 되면 루프 탈출
            if (amount == 0) break;

            // 해당 아이템이 있는 슬롯에 등록
            if (slot.m_Item == p_Item)
            {
                // 등록 가능한 숫자만큼 등록
                int available = p_Item.MaxStack - slot.m_Stack;
                if (amount > available)
                {
                    slot.m_Stack += available;
                    amount -= available;
                }

                // 해당 슬롯이 여유로운 경우
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
                // 0개가 되면 루프 탈출
                if (amount == 0) break;

                // 빈 슬롯이나 해당 아이템이 있는 슬롯에 등록
                if (slot.m_Item == null)
                {
                    if (slot.m_Item == null) slot.m_Item = p_Item;

                    // 등록 가능한 숫자만큼 등록
                    int available = p_Item.MaxStack - slot.m_Stack;
                    if (amount > available)
                    {
                        slot.m_Stack += available;
                        amount -= available;
                    }

                    // 해당 슬롯이 여유로운 경우
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
            m_UIManager.OpenWindow_OK($"인벤토리 공간이 부족하여 {p_Item.Name} 아이템 {amount}개를 획득하지 못했습니다.", false);
        }
    }
}
