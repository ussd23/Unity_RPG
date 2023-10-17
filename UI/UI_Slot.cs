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
    public Item m_Item;                         // ������ Ŭ����
    public int m_Stack;                         // �ش� ���Կ� �ִ� �������� ���� ����
    public bool m_isEquipSlot = false;          // ��� ���Ը� true
    public bool m_isInventory = false;          // �κ��丮�� true 
    public bool m_isHoldItem = false;           // Ȧ�� ���Ը� true
    public bool m_isQuickSlot = false;          // �� ���Ը� true
    public Type m_Type = Type.None;
    public UI_Slot m_LastSlot;

    private bool m_isHighlighted = false;        // �ش� ���Կ� ���콺 �����Ͱ� �ִ� ��� true
    private ItemManager m_ItemManager;
    private UI_Manager m_Manager;
    private UI_Inventory m_Inventory;
    private Sprite m_DefaultSprite;              // �������� null�� ��� ǥ���� ��������Ʈ
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

    // UI�� ���� �� UI ������Ʈ
    private void OnEnable()
    {
        UIUpdate();
    }

    // UI ������Ʈ
    public void UIUpdate()
    {
        if (gameObject.activeInHierarchy == false) return;

        if (m_Highlight != null) HighLightUpdate();

        // �� ������ ���
        if (m_Item == null)
        {
            m_Sprite.sprite = m_DefaultSprite;
            if (m_Highlight != null) m_Highlight.gameObject.SetActive(false);
            if (m_Amount != null) m_Amount.gameObject.SetActive(false);
        }

        // �������� �ִ� ������ ���
        else
        {
            m_Sprite.sprite = m_Item.Sprite;

            if (m_Amount != null)
            {
                // ���� ������ 1�� �̻��� ��� ���� ǥ��
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

    // ���콺 ���� �� ǥ�õǴ� ȿ�� ����
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

    // ������ ��� ���� ����
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

    // Ȧ�� ������ ������Ʈ ǥ�� ��ġ ���� ����
    private void HoldItemUpdate()
    {
        transform.position = Input.mousePosition;
    }

    // ���� ���� ����
    private void TooltipUpdate(Item p_Item)
    {
        string text = $"[{p_Item.Grade} {p_Item.Type}]";

        if (p_Item.Text != null)
        {
            text += $"\n\n{p_Item.Text}";
        }

        m_Manager.TooltipUpdate(p_Item.Name, text);
    }

    // Ŭ�� �� �̺�Ʈ
    public void OnPointerDown(PointerEventData eventData)
    {
        // ��Ŭ��
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

    // Ŭ�� ���� �� �̺�Ʈ
    public void OnPointerUp(PointerEventData eventData)
    {
        // ���ҵ��� ���� ��� ����
        if (m_Manager.m_HoldItem.m_Item == null) return;

        // ��Ŭ��
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UI_Slot target = eventData.pointerCurrentRaycast.gameObject?.GetComponent<UI_Slot>();
            bool trig = false;

            // ���� ���� ���� ���� �� ����
            if (target != null && target != this &&
                (target.m_Type == m_Manager.m_HoldItem.m_Item.Type || target.m_Type == Type.None))
            {
                if ((m_isEquipSlot || target.m_isEquipSlot) ?
                    (target.m_Item == null || m_Type == target.m_Item.Type) : true)
                {
                    target.PointerFunction();

                    // ��� ���Կ� ��ȭ�� �ִ� ��� �ش� �������� ��ũ��Ʈ ����
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

            // ���� ��ǥ��
            if (target != null && target.m_Item != null)
            {
                TooltipUpdate(target.m_Item);
                m_Manager.Tooltip(true);
            }
        }
    }

    // ������ ��ġ��, ���� �̵� ���� ���
    public void PointerFunction()
    {
        // Ȧ�� ���԰� �ش� ������ ����ִ� ��� 
        if (m_Item == null && m_Manager.m_HoldItem.m_Item == null) return;

        // ���� �������� ��� ��ħ
        if (m_Item == m_Manager.m_HoldItem.m_Item)
        {
            // �ִ� ���� ������ ���õ� ó��
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

        // ���� �������� �ƴ� ��� ����
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

        // ��ȭ�� ������ UI ������Ʈ
        m_Manager.m_HoldItem.UIUpdate();
        UIUpdate();

        m_Manager.Save();
    }

    // ���콺�� ������ ��� (���̶���Ʈ ����)
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

    // ���콺�� ������ ���� ��� (���̶���Ʈ ����)
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
