using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillInfo : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
{
    public int m_SkillNo;
    public string m_SkillName = "";
    public string m_SkillText = "";
    public bool m_StatInfo = true;
    public Skill m_SkillComponent;
    public int m_RequirePlayerLevel = 0;
    public int m_RequireSP = 1;
    public int m_SkillLevel = 0;
    public int m_MinSkillLevel = 0;
    public int m_MaxSkillLevel = 1;
    public UI_SkillInfo m_RequireSkill;
    public int m_RequireSkillLevel = 1;
    [SerializeField]
    private Image m_RequireGauge;

    private UI_Manager m_Manager;
    private UI_SkillTree m_SkillTree;
    private Info m_PlayerInfo;
    private Image m_LevelImage;
    private Text m_LevelText;
    private Image m_UpButton;
    private Image m_DownButton;
    private UI_SkillInfo m_NextSkill;
    private float m_UIReqireLevel = 0f;
    private float m_LerpT = 5f;
    private bool m_GuideText = false;

    public void Awake()
    {
        m_Manager = UI_Manager.GetInstance;
        m_SkillTree = transform.GetComponentInParent<UI_SkillTree>();
        m_LevelImage = transform.GetChild(1)?.GetComponent<Image>();
        m_LevelText = m_LevelImage?.transform.GetComponentInChildren<Text>();
        m_UpButton = transform.GetChild(2)?.GetComponent<Image>();
        m_DownButton = transform.GetChild(3)?.GetComponent<Image>();

        if (m_RequireSkill != null) m_RequireSkill.SetNext(this);
    }

    public void Start()
    {
        m_PlayerInfo = Info.PlayerInfo;
    }

    public void Update()
    {
        UIUpdate();
    }

    public void SetSkillComponent(Skill p_com)
    {
        m_SkillComponent = p_com;
        if (m_SkillComponent != null) m_SkillComponent.SkillLevelUpdate(m_SkillLevel);
    }

    public void SetNext(UI_SkillInfo p_com)
    {
        m_NextSkill = p_com;
    }

    public void LevelUp(int p_val)
    {
        if (m_SkillLevel + p_val > m_MaxSkillLevel || m_SkillLevel + p_val < m_MinSkillLevel)
        {
            return;
        }
        if (m_PlayerInfo.m_Level < m_RequirePlayerLevel)
        {
            m_Manager.OpenWindow_OK($"요구 레벨을 만족하지 않습니다.\n({m_RequirePlayerLevel} 레벨 이상)", true);
            return;
        }
        if (p_val > 0 && m_RequireSkill != null && m_RequireSkill.m_SkillLevel < m_RequireSkillLevel)
        {
            m_Manager.OpenWindow_OK($"선행 스킬 조건을 만족하지 않습니다.\n({m_RequireSkill.m_SkillName} {m_RequireSkillLevel} 레벨 이상)", true);
            return;
        }
        if (p_val < 0 && m_NextSkill != null &&
            m_NextSkill.m_RequireSkillLevel >= m_SkillLevel && m_NextSkill.m_SkillLevel != 0)
        {
            m_Manager.OpenWindow_OK($"{m_NextSkill.m_SkillName}에 SP가 투자되어 있어 레벨을 낮출 수 없습니다.", true);
            return;
        }
        if (p_val > 0 && !m_SkillTree.is_SPRemains(m_RequireSP))
        {
            m_Manager.OpenWindow_OK($"SP가 부족합니다.\n(요구 SP: {m_RequireSP})", true);
            return;
        }

        m_SkillLevel += p_val;
        m_SkillTree.SPUp(-m_RequireSP * p_val);

        SkillUpdate();
        m_SkillTree.TooltipUpdate(m_SkillNo);
        m_SkillTree.Save();
    }

    public void UIUpdate()
    {
        bool level = m_PlayerInfo.m_Level < m_RequirePlayerLevel;
        if (m_LevelImage != null)
        {
            if (level)
            {
                m_LevelImage.fillAmount = 1;
                m_LevelText.text = $"{m_RequirePlayerLevel} 이상";
            }
            else if (m_RequireSkill != null && m_RequireSkill.m_SkillLevel < m_RequireSkillLevel)
            {
                m_LevelImage.fillAmount = 1;
                m_LevelText.text = "선행";
            }
            else if (m_GuideText)
            {
                m_LevelImage.fillAmount = 0;
                m_LevelText.text = $"{m_SkillLevel} / {m_MaxSkillLevel}";
            }
            else
            {
                m_LevelImage.fillAmount = 0;
                m_LevelText.text = "";
            }
        }

        if (m_UpButton != null)
        {
            if (m_SkillLevel == m_MaxSkillLevel || level || !m_SkillTree.is_SPRemains(m_RequireSP) ||
                (m_RequireSkill != null && m_RequireSkill.m_SkillLevel < m_RequireSkillLevel))
                m_UpButton.color = Color.gray;
            else m_UpButton.color = Color.green;
        }

        if (m_DownButton != null)
        {
            if (m_SkillLevel == m_MinSkillLevel || level || (m_NextSkill != null &&
                m_NextSkill.m_RequireSkillLevel >= m_SkillLevel && m_NextSkill.m_SkillLevel != 0))
                m_DownButton.color = Color.gray;
            else m_DownButton.color = Color.red;
        }

        if (m_RequireGauge != null)
        {
            m_UIReqireLevel = Mathf.Lerp(m_UIReqireLevel, m_RequireSkill.m_SkillLevel, m_LerpT * Time.deltaTime);
            m_RequireGauge.fillAmount = m_UIReqireLevel / m_RequireSkillLevel;
        }
    }

    public void SkillUpdate()
    {
        if (m_SkillComponent != null) m_SkillComponent.SkillLevelUpdate(m_SkillLevel);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_GuideText = true;
        m_SkillTree.TooltipUpdate(m_SkillNo);
        m_Manager.Tooltip(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_GuideText = false;
        m_Manager.Tooltip(false);
    }
}
