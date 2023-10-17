using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    // 편집 변수 (플레이어 체력, 기력)
    public int m_Level = 1;
    public int m_EXP = 0;
    public float m_MaxHealth = 100f;
    [SerializeField]
    protected float m_Health;
    public float m_MaxEnergy = 100f;
    [SerializeField]
    protected float m_Energy;
    [SerializeField]
    protected Animator m_Model = null;
    [SerializeField]
    protected bool m_Invincible = false;
    public int m_Attack = 100;
    public int m_Defense = 10;
    public int m_Currency = 0;

    // 내부 변수
    public PlayerData m_EquipStats;
    public static Info PlayerInfo = null;
    protected Text m_LevelText = null;
    protected Image m_PortraitImage = null;
    protected Image m_HealthBar = null;
    protected Text m_HealthText = null;
    protected Image m_EnergyBar = null;
    protected Text m_EnergyText = null;
    protected Image m_EXPBar = null;
    protected Text m_EXPText = null;
    protected int m_RequireEXP = 0;
    private Color32 m_UIColor = new Color32(255, 255, 255, 255);
    protected GameObject m_DamageText;
    protected UI_Manager m_Manager;
    protected float m_UIHealth;
    protected float m_UIEnergy;
    protected float m_UIMaxHealth;
    protected float m_UIMaxEnergy;
    protected float m_UIEXP;
    protected float m_UIRequireEXP;
    protected float m_LerpT = 10f;
    protected bool m_isPlayer = false;

    private void Awake()
    {
        if (tag == "Player")
        {
            m_isPlayer = true;
            PlayerInfo = this;
        }

        m_Health = m_MaxHealth;
        m_Energy = m_MaxEnergy;
        m_UIHealth = m_Health;
        m_UIEnergy = m_Energy;
        m_UIMaxHealth = m_MaxHealth;
        m_UIMaxEnergy = m_MaxEnergy;
        m_EquipStats = new PlayerData();
    }

    private void Start()
    {
        m_Manager = UI_Manager.GetInstance;
        if (m_isPlayer)
        {
            Load();

            m_RequireEXP = ReqEXP();
        }

        if (m_Manager != null)
        {
            m_DamageText = m_Manager.m_DamageText;

            if (m_isPlayer)
            {
                m_LevelText = m_Manager.m_LevelText;
                m_PortraitImage = m_Manager.m_PortaitImage;
                m_HealthBar = m_Manager.m_HealthBar;
                m_HealthText = m_Manager.m_HealthText;
                m_EnergyBar = m_Manager.m_EnergyBar;
                m_EnergyText = m_Manager.m_EnergyText;
                m_EXPBar = m_Manager.m_EXPBar;
                m_EXPText = m_Manager.m_EXPText;
            }
        }
    }

    // UI 업데이트
    private void Update()
    {
        if (m_isPlayer)
        {
            if (m_EXP >= m_RequireEXP)
            {
                m_EXP = m_EXP - m_RequireEXP;
                if (m_EXP < 0) m_EXP = 0;

                if (LevelUp(1))
                {
                    UI_SkillTree tempcom = m_Manager.m_SkillTreeUI;
                    tempcom.SPUp(tempcom.m_SPPerLevel);
                }
            }
        }
        UIUpdate();
    }

    // 사망 확인
    public bool isDie()
    {
        if (m_Health <= 0)
        {
            m_Health = 0;
            return true;
        }
        else return false;
    }

    // 피해 설정
    public void SetDamage(float p_dmg)
    {
        if (m_Invincible) return;

        float calcDmg = CalculateDamage(p_dmg);

        m_Health -= calcDmg;

        if (m_PortraitImage != null) StartCoroutine(DamageEffect());

        if (enabled && m_DamageText != null) DamageText(calcDmg);

        if (isDie())
        {
            if (m_PortraitImage != null) m_UIColor = new Color32(127, 127, 127, 255);

            m_Model.SetTrigger("Dead");
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<Mover>().m_Movable = false;
            m_Invincible = true;
            Destroy(gameObject, 2f);

            if (!m_isPlayer)
            {
                PlayerInfo.AddEXP(m_EXP);
                GetComponent<DropTable>()?.ItemDrop();
                GetComponent<DropTable>()?.CurrencyDrop();
            }
        }
    }

    // 피해량 텍스트
    public void DamageText(float p_dmg)
    {
        GameObject obj = Instantiate(m_DamageText);
        obj.transform.SetParent(m_Manager.transform);
        obj.GetComponent<UI_Damage>().SetPosition(transform.position);
        obj.GetComponent<Text>().text = $"{(int)p_dmg}";
        obj.SetActive(true);
    }

    // 기력 회복
    public bool isUsable(float p_use)
    {
        if (m_Energy > p_use) return true;
        
        return false;
    }

    // 기력 사용
    public bool UseEnergy(float p_use)
    {
        if (isUsable(p_use))
        {
            m_Energy -= p_use;
            return true;
        }
        
        return false;
    }

    // 체력 회복
    public void HealthRegen(float p_reg)
    {
        m_Health += p_reg;

        if (m_Health > m_MaxHealth)
        {
            m_Health = m_MaxHealth;
        }
    }

    // 기력 회복
    public void EnergyRegen(float p_reg)
    {
        m_Energy += p_reg;

        if (m_Energy > m_MaxEnergy)
        {
            m_Energy = m_MaxEnergy;
        }
    }

    // UI 업데이트
    public void UIUpdate()
    {
        if (m_LevelText != null)
        {
            m_LevelText.text = $"{m_Level}";
        }

        if (m_HealthBar != null)
        {
            m_UIHealth = Mathf.Lerp(m_UIHealth, m_Health, m_LerpT * Time.deltaTime);
            m_UIMaxHealth = Mathf.Lerp(m_UIMaxHealth, m_MaxHealth, m_LerpT * Time.deltaTime);
            m_HealthBar.fillAmount = m_UIHealth / m_UIMaxHealth;
        }

        if (m_HealthText != null)
        {
            m_HealthText.text = $"{Mathf.Round(m_UIHealth)} / {Mathf.Round(m_UIMaxHealth)}";
        }

        if (m_EnergyBar != null)
        {
            m_UIEnergy = Mathf.Lerp(m_UIEnergy, m_Energy, m_LerpT * Time.deltaTime);
            m_UIMaxEnergy = Mathf.Lerp(m_UIMaxEnergy, m_MaxEnergy, m_LerpT * Time.deltaTime);
            m_EnergyBar.fillAmount = m_UIEnergy / m_UIMaxEnergy;
        }

        if (m_EnergyText != null)
        {
            m_EnergyText.text = $"{Mathf.Round(m_UIEnergy)} / {Mathf.Round(m_UIMaxEnergy)}";
        }

        if (m_EXPBar != null)
        {
            m_UIEXP = Mathf.Lerp(m_UIEXP, m_EXP, m_LerpT * Time.deltaTime);
            m_UIRequireEXP = Mathf.Lerp(m_UIRequireEXP, m_RequireEXP, m_LerpT * Time.deltaTime);
            m_EXPBar.fillAmount = m_UIEXP / m_UIRequireEXP;
        }

        if (m_EXPText != null)
        {
            m_EXPText.text = $"{Mathf.Round(m_UIEXP)} / {Mathf.Round(m_UIRequireEXP)}";
        }
    }

    private float CalculateDamage(float p_dmg)
    {
        float defense = m_Defense + m_EquipStats.Defense;
        return p_dmg > defense / 2 ? p_dmg - defense / 4 : p_dmg / 2;
    }

    // 인스턴트 게이지 세팅
    public void SetHealthBar(Image p_img)
    {
        m_HealthBar = p_img;
    }

    // 피격 효과
    IEnumerator DamageEffect()
    {
        for (int i = 0; i < 3; ++i)
        {
            m_PortraitImage.color = new Color32(255, 127, 127, 255);
            yield return new WaitForSeconds(0.15f);
            m_PortraitImage.color = new Color32(255, 127, 127, 191);
            yield return new WaitForSeconds(0.15f);
        }
        m_PortraitImage.color = m_UIColor;
    }

    public void Invincible(float p_val)
    {
        m_PortraitImage.color = new Color32(255, 255, 0, 255);
        StartCoroutine(InvincibleEffect(p_val));
    }

    // 무적 효과
    IEnumerator InvincibleEffect(float p_val)
    {
        yield return new WaitForSeconds(p_val);
        m_PortraitImage.color = m_UIColor;
    }

    public bool LevelUp(int p_val)
    {
        if (m_Level + p_val > 99 || m_Level + p_val < 1) return false;

        m_Level += p_val;
        m_RequireEXP = ReqEXP();
        m_Attack += p_val * 5;
        m_UIEXP = 0;
        Save();
        return true;
    }

    public void AddEXP(int p_val)
    {
        m_EXP += p_val;
        Save();
    }

    public void AddAttack(int p_val)
    {
        m_Attack += p_val;
        Save();
    }

    public void AddDefense(int p_val)
    {
        m_Defense += p_val;
        Save();
    }

    public bool CurrencyChange(int p_val)
    {
        if (m_Currency < -p_val) return false;

        if (int.MaxValue - m_Currency < p_val) m_Currency = int.MaxValue;
        else m_Currency += p_val;
        Save();
        UIUpdate();

        return true;
    }

    public void Save()
    {
        JsonReader.PlayerInfoSave();
    }

    public void Load()
    {
        PlayerData data = JsonReader.PlayerInfoLoad();

        if (data != null)
        {
            m_Level = data.Level;
            m_EXP = data.EXP;
            m_Currency = data.Currency;
            m_Attack = data.Attack;
            m_Defense = data.Defense;
        }
    }

    public int ReqEXP()
    {
        return (m_Level - 1) * (m_Level - 1) + (m_Level - 1) * 5 + 100;
    }

    public void BossHP(Image p_com)
    {
        m_HealthBar = p_com;
    }
}
