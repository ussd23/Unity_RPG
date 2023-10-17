using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot : MonoBehaviour
    , IPointerUpHandler
    , IPointerDownHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    public Item m_Item;                         // 아이템 클래스
    public int m_Stack;                         // 해당 슬롯에 있는 아이템의 소지 개수
    public bool m_isEquipSlot = false;          // 장비 슬롯만 true
    public bool m_isInventory = false;          // 인벤토리만 true 
    public bool m_isHoldItem = false;           // 홀드 슬롯만 true
    public bool m_isQuickSlot = false;          // 퀵 슬롯만 true
    public Type m_Type = Type.None;
    public UI_Slot m_LastSlot;

    private bool m_isHighlighted = false;        // 해당 슬롯에 마우스 포인터가 있는 경우 true
    private ItemManager m_ItemManager;
    private UI_Manager m_Manager;
    private UI_Inventory m_Inventory;
    private Sprite m_DefaultSprite;              // 아이템이 null인 경우 표시할 스프라이트
    private Image m_Sprite;
    private Image m_Highlight;
    private Text m_Amount;

    private void Awake()
    {
        m_Sprite = transform.GetChild(0)?.GetComponent<Image>();
        m_Amount = transform.GetChild(1)?.GetComponent<Text>();
        if (!m_isHoldItem) m_Highlight = transform.GetChild(2)?.GetComponent<Image>();

        if (m_Amount != null) m_Amount.gameObject.SetActive(false);
        if (m_Highlight != null) m_Highlight.gameObject.SetActive(false);
    }

    private void Start()
    {
        m_Manager = UI_Manager.GetInstance;
        m_ItemManager = ItemManager.GetInstance;
        m_Inventory = m_Manager.m_InventoryUI;
        m_DefaultSprite = m_ItemManager.m_Sprites[0];

        UIUpdate();
    }

    private void Update()
    {
        if (!m_isHoldItem) return;

        HoldItemUpdate();
    }

    // UI가 켜질 때 UI 업데이트
    private void OnEnable()
    {
        UIUpdate();
    }

    // UI 업데이트
    public void UIUpdate()
    {
        if (gameObject.activeInHierarchy == false) return;

        if (m_Highlight != null) HighLightUpdate();

        // 빈 슬롯인 경우
        if (m_Item == null)
        {
            m_Sprite.sprite = m_DefaultSprite;
            if (m_Highlight != null) m_Highlight.gameObject.SetActive(false);
            if (m_Amount != null) m_Amount.gameObject.SetActive(false);
        }

        // 아이템이 있는 슬롯인 경우
        else
        {
            m_Sprite.sprite = m_Item.Sprite;

            if (m_Amount != null)
            {
                // 소지 개수가 1개 이상인 경우 개수 표시
                if (m_Stack > 1)
                {
                    m_Amount.gameObject.SetActive(true);
                    m_Amount.text = $"{m_Stack}";
                }
                else
                {
                    m_Amount.gameObject.SetActive(false);
                }
            }
        }
    }

    // 마우스 오버 시 표시되는 효과 관련
    public void HighLightUpdate()
    {
        if (m_isHighlighted)
        {
            m_Highlight.gameObject.SetActive(true);
        }
        else
        {
            m_Highlight.gameObject.SetActive(false);
        }
    }

    // 아이템 사용 가능 여부
    public bool UseItem(int p_amount)
    {
        if (m_Stack >= p_amount)
        {
            m_Stack -= p_amount;

            if (m_Stack <= 0)
            {
                m_Item = null;
            }
            UIUpdate();
            m_Manager.Save();

            return true;
        }

        return false;
    }

    // 홀드 슬롯의 오브젝트 표시 위치 관련 설정
    private void HoldItemUpdate()
    {
        transform.position = Input.mousePosition;
    }

    // 툴팁 내용 변경
    private void TooltipUpdate(Item p_Item)
    {
        string text = $"[{p_Item.Grade} {p_Item.Type}]";

        if (p_Item.Text != null)
        {
            text += $"\n\n{p_Item.Text}";
        }

        m_Manager.TooltipUpdate(p_Item.Name, text);
    }

    // 클릭 시 이벤트
    public void OnPointerDown(PointerEventData eventData)
    {
        // 좌클릭
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PointerFunction();
            m_Manager.Tooltip(false);

            if (m_isEquipSlot && m_Manager.m_HoldItem.m_Item != null)
            {
                m_Manager.SetEquipStats();
            }
        }
    }

    // 클릭 해제 시 이벤트
    public void OnPointerUp(PointerEventData eventData)
    {
        // 스왑되지 않은 경우 리턴
        if (m_Manager.m_HoldItem.m_Item == null) return;

        // 좌클릭
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UI_Slot target = eventData.pointerCurrentRaycast.gameObject?.GetComponent<UI_Slot>();
            bool trig = false;

            // 스왑 가능 여부 판정 후 스왑
            if (target != null && target != this &&
                (target.m_Type == m_Manager.m_HoldItem.m_Item.Type || target.m_Type == Type.None))
            {
                if ((m_isEquipSlot || target.m_isEquipSlot) ?
                    (target.m_Item == null || m_Type == target.m_Item.Type) : true)
                {
                    target.PointerFunction();

                    // 장비 슬롯에 변화가 있는 경우 해당 아이템의 스크립트 실행
                    if (target != null && target.m_isEquipSlot && target.m_Item != null)
                    {
                        m_Manager.SetEquipStats();
                    }
                }
            }

            PointerFunction();
            if (m_isEquipSlot && m_Item != null)
            {
                m_Manager.SetEquipStats();
            }

            // 툴팁 재표시
            if (target != null && target.m_Item != null)
            {
                TooltipUpdate(target.m_Item);
                m_Manager.Tooltip(true);
            }
        }
    }

    // 아이템 합치기, 슬롯 이동 등의 기능
    public void PointerFunction()
    {
        // 홀드 슬롯과 해당 슬롯이 비어있는 경우 
        if (m_Item == null && m_Manager.m_HoldItem.m_Item == null) return;

        // 같은 아이템인 경우 합침
        if (m_Item == m_Manager.m_HoldItem.m_Item)
        {
            // 최대 소지 개수에 관련된 처리
            if (m_Item.MaxStack >= m_Stack + m_Manager.m_HoldItem.m_Stack)
            {
                m_Stack += m_Manager.m_HoldItem.m_Stack;
                m_Manager.m_HoldItem.m_Item = null;
                m_Manager.m_HoldItem.m_Stack = 0;
            }
            else
            {
                int available = m_Item.MaxStack - m_Stack;
                m_Stack += available;
                m_Manager.m_HoldItem.m_Stack -= available;
            }
        }

        // 같은 아이템이 아닌 경우 스왑
        else
        {
            Item tempitem = m_Item;
            int tempstack = m_Stack;
            m_Item = m_Manager.m_HoldItem.m_Item;
            m_Stack = m_Manager.m_HoldItem.m_Stack;

            m_Manager.m_HoldItem.m_Item = tempitem;
            m_Manager.m_HoldItem.m_Stack = tempstack;

            if (m_Manager.m_HoldItem.m_Item != null)
            {
                m_Manager.m_HoldItem.m_LastSlot = this;
            }
            else
            {
                m_Manager.m_HoldItem.m_LastSlot = null;
            }
        }

        // 변화된 슬롯의 UI 업데이트
        m_Manager.m_HoldItem.UIUpdate();
        UIUpdate();

        m_Manager.Save();
    }

    // 마우스를 오버한 경우 (하이라이트 설정)
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_isHighlighted = true;
        HighLightUpdate();

        if (m_Item != null)
        {
            TooltipUpdate(m_Item);
            m_Manager.Tooltip(true);
        }
    }

    // 마우스가 밖으로 나간 경우 (하이라이트 해제)
    public void OnPointerExit(PointerEventData eventData)
    {
        m_isHighlighted = false;
        HighLightUpdate();

        if (m_Item != null)
        {
            m_Manager.Tooltip(false);
        }
    }
}
