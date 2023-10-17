using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : GetI<UI_Manager>
{
    public Text m_LevelText;
    public Image m_PortaitImage;
    public Image m_HealthBar;
    public Text m_HealthText;
    public Image m_EnergyBar;
    public Text m_EnergyText;
    public Image m_EXPBar;
    public Text m_EXPText;
    public Text m_UITextUp;
    public Text m_UITextDown;
    public Text m_NextText;
    public Text m_GameOver;
    public Text m_GameClear;
    public GameObject m_MobGuageBack;
    public GameObject m_BossGuageBack;
    public GameObject m_DamageText;
    public RectTransform m_Resolution;
    public UI_Character m_CharacterUI;
    public UI_Inventory m_InventoryUI;
    public UI_SkillTree m_SkillTreeUI;
    [SerializeField]
    private Window m_Window_OK;
    [SerializeField]
    private RectTransform m_TooltipObj;

    private Info m_PlayerInfo;
    private bool m_CharacterOpens = false;
    private bool m_InventoryOpens = false;
    private bool m_SkillTreeOpens = false;
    private bool m_Modal = false;
    private bool m_Tooltip = false;
    private Text m_TooltipTitle;
    private Text m_TooltipText;
    private RectTransform m_TooltipTextRectTransform;
    private TextGenerator textGenerator;

    public UI_Slot m_HoldItem;                      // 홀드 슬롯
    public List<UI_Slot> m_EquipmentSlots;          // 장비 슬롯
    public List<UI_Slot> m_InventorySlots;          // 인벤토리 슬롯
    public List<UI_Slot> m_QuickSlots;              // 퀵 슬롯

    private void Awake()
    {
        textGenerator = new TextGenerator();
        m_TooltipTitle = m_TooltipObj.GetChild(0).GetComponent<Text>();
        m_TooltipText = m_TooltipObj.GetChild(1).GetComponent<Text>();
        m_TooltipTextRectTransform = m_TooltipText.GetComponent<RectTransform>();
        m_Resolution = GetComponent<RectTransform>();

        SlotInit();
    }

    private void Start()
    {
        m_PlayerInfo = Info.PlayerInfo;
    }

    private void Update()
    {
        if (m_Modal) return;

        if (m_Tooltip)
        {
            TooltipUIUpdate();
        }

        // 키 입력에 따른 메뉴 오픈
        if (Input.GetKeyDown(KeyCode.U))
        {
            CharacterOpens();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryOpens();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SkillTreeOpens();
        }

        if (m_CharacterOpens || m_InventoryOpens ||
            (m_HoldItem.m_LastSlot != null && m_HoldItem.m_LastSlot.m_isQuickSlot)) m_HoldItem.gameObject.SetActive(true);
        else m_HoldItem.gameObject.SetActive(false);
    }

    public void CharacterOpens()
    {
        UIOpens(ref m_CharacterOpens, m_CharacterUI);
        SlotClose();
    }

    public void InventoryOpens()
    {
        UIOpens(ref m_InventoryOpens, m_InventoryUI);
        SlotClose();
    }

    public void SkillTreeOpens()
    {
        UIOpens(ref m_SkillTreeOpens, m_SkillTreeUI);
    }

    private void UIOpens(ref bool p_val, UI_Movable p_com)
    {
        p_val = !p_val;

        if (p_val)
        {
            p_com.gameObject.SetActive(true);
            SetFocus(p_com);
            m_TooltipObj.transform.SetAsLastSibling();
        }
        else
        {
            p_com.gameObject.SetActive(false);
            Tooltip(false);
        }
    }

    private void SlotClose()
    {
        if (m_HoldItem.m_Item != null &&
            m_HoldItem.m_LastSlot != null &&
            !m_HoldItem.m_LastSlot.m_isQuickSlot)
        {
            m_HoldItem.m_LastSlot.PointerFunction();
        }
    }

    public void LevelUp(int p_val)
    {
        if (p_val < 0)
        {
            if (!m_SkillTreeUI.SPCheck(m_PlayerInfo.m_Level + p_val))
            {
                OpenWindow_OK($"요구 레벨이 {m_PlayerInfo.m_Level}인 스킬에 SP가 투자되어 있어 레벨을 낮출 수 없습니다.", true);
                return;
            }
            if (m_SkillTreeUI.m_SP < m_SkillTreeUI.m_SPPerLevel * -p_val)
            {
                OpenWindow_OK($"보유 SP가 {m_SkillTreeUI.m_SPPerLevel} 미만인 경우 레벨을 낮출 수 없습니다.", true);
                return;
            }
        }

        if (m_PlayerInfo.LevelUp(p_val))
        {
            m_SkillTreeUI.SPUp(m_SkillTreeUI.m_SPPerLevel * p_val);
        }
        else
        {
            OpenWindow_OK("레벨 변경에 실패했습니다.", true);
        }
    }

    // 팝업창 생성
    public void OpenWindow_OK(string p_notice, bool p_modal)
    {
        GameObject obj = Instantiate(m_Window_OK.gameObject);
        obj.transform.SetParent(transform);

        obj.transform.position = transform.GetChild(0).position;
        obj.transform.localScale = Vector3.one;

        Window tempcom = obj.GetComponent<Window>();
        tempcom.m_Modal = p_modal;
        tempcom.m_Notice = p_notice;
        obj.SetActive(true);

        if (p_modal) m_Modal = true;
    }

    // Modal 해제
    public void UnModal()
    {
        m_Modal = false;
    }

    // 새로 연 창이나 팝업창의 포커스 설정 (화면 상 맨 앞 레이어에 위치하도록)
    public void SetFocus(UI_Movable p_com)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform temp = transform.GetChild(i);
            if (temp.GetComponent<UI_Movable>() == p_com)
            {
                temp.SetAsLastSibling();
                p_com.PosRevision();
            }
        }
        m_HoldItem.transform.SetAsLastSibling();
    }

    // 슬롯 생성 및 관련 정보 초기화
    public void SlotInit()
    {
        for (int i = 0; i < 5; ++i)
        {
            UI_Slot slot;
            if (i != 0)
            {
                slot = Instantiate(m_EquipmentSlots[0]);
                m_EquipmentSlots.Add(slot);

                Transform comp = slot.transform;

                comp.position = m_EquipmentSlots[0].transform.position;
                comp.SetParent(m_EquipmentSlots[0].transform.parent);
                Vector3 pos = comp.localPosition;
                pos.y -= 50 * i;
                comp.localPosition = pos;

                Vector3 scale = comp.localScale;
                scale.x = 1;
                scale.y = 1;
                comp.localScale = scale;
            }
            else
            {
                slot = m_EquipmentSlots[0];
            }
            slot.m_Type = Type.무기 + i;
        }

        for (int i = 0; i < 56; ++i)
        {
            UI_Slot slot;
            if (i != 0)
            {
                slot = Instantiate(m_InventorySlots[0]);
                m_InventorySlots.Add(slot);

                Transform comp = slot.transform;

                comp.position = m_InventorySlots[0].transform.position;
                comp.SetParent(m_InventorySlots[0].transform.parent);
                Vector3 pos = comp.localPosition;
                pos.x += 50 * (i % 8);
                pos.y -= 50 * (i / 8);
                comp.localPosition = pos;

                Vector3 scale = comp.localScale;
                scale.x = 1;
                scale.y = 1;
                comp.localScale = scale;
            }
        }
    }

    // 툴팁 On/Off
    public void Tooltip(bool p_val)
    {
        if (p_val)
        {
            m_Tooltip = true;
            m_TooltipObj.gameObject.SetActive(true);
            m_TooltipObj.transform.SetAsLastSibling();
        }
        else
        {
            m_Tooltip = false;
            m_TooltipObj.gameObject.SetActive(false);
        }
    }

    // 툴팁 내용 변경
    public void TooltipUpdate(string p_name, string p_text)
    {
        m_TooltipTitle.text = p_name;
        m_TooltipText.text = p_text;

        Vector2 size = m_TooltipObj.sizeDelta;
        size.y = 47 + textGenerator.GetPreferredHeight(m_TooltipText.text, m_TooltipText.GetGenerationSettings(m_TooltipTextRectTransform.rect.size)) / m_Resolution.localScale.y;
        m_TooltipObj.sizeDelta = size;

        TooltipUIUpdate();
    }

    // 툴팁이 마우스 포인터를 따라오도록 설정
    private void TooltipUIUpdate()
    {
        m_TooltipObj.position = Input.mousePosition;

        Vector2 screen = GetScreenSize();
        Vector2 pos = m_TooltipObj.anchoredPosition;

        // 툴팁이 화면 밖으로 벗어나지 않도록 pivot 조정
        Vector2 pivot = m_TooltipObj.pivot;
        if (pos.x > screen.x / 2 - m_TooltipObj.rect.width * 1.1f) pivot.x = 1.1f;
        else pivot.x = -0.1f;
        pivot.y = (pos.y + screen.y / 2) / screen.y;
        m_TooltipObj.pivot = pivot;
    }

    public void SetEquipStats()
    {
        Info info = Info.PlayerInfo;

        info.m_EquipStats = new PlayerData();

        foreach (UI_Slot slot in m_EquipmentSlots)
        {
            if (slot.m_Item == null) continue;
            slot.m_Item.RunScript(info);
        }
    }

    // 현재 해상도가 아닌 기본 해상도(1280x720)를 기준으로 위치 변환
    public Vector2 GetLocalPoint(Vector2 p_val)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Resolution, p_val, null, out localPoint);
        return localPoint;
    }

    // 화면 비율 계산
    public Vector2 GetScreenSize()
    {
        Vector2 screen = new Vector2();
        screen.x = (int)m_Resolution.rect.width;
        screen.y = (int)m_Resolution.rect.height;
        return screen;
    }

    // 인벤토리 저장
    public void Save()
    {
        JsonReader.SlotSave();
    }
}
