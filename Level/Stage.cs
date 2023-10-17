using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    // 편집 변수
    [SerializeField]
    private List<GameObject> m_Mobs;
    [SerializeField]
    private GameObject m_NextWall;
    [SerializeField]
    private GameObject m_CurrentWall;
    [SerializeField]
    private GameObject m_PrevWall;
    [SerializeField]
    private Transform m_LevelLine;
    [SerializeField]
    private Text m_UIText;
    [SerializeField]
    private Text m_NextText;

    // 내부 변수
    private PlayerMover m_Mover;
    private bool m_Clear = false;
    private Coroutine m_Cor;

    // 플레이어의 이동 컴포넌트를 불러옴
    private void Awake()
    {
        UI_Manager tempcom = UI_Manager.GetInstance;
        m_UIText = tempcom.m_UITextDown;
        m_NextText = tempcom.m_NextText;
    }

    private void Start()
    {
        m_Mover = Info.PlayerInfo.GetComponent<PlayerMover>();
    }

    // 해당 스테이지가 시작된 경우 Init
    private void OnEnable()
    {
        foreach (GameObject v in m_Mobs)
        {
            if (v != null) v.SetActive(true);
        }
        if (m_NextWall != null) m_NextWall.SetActive(true);
    }

    private void Update()
    {
        if (m_Clear) return;

        // 죽은 몹 삭제
        for (int i = 0; i < m_Mobs.Count;)
        {
            if (m_Mobs[i] == null)
            {
                m_Mobs.RemoveAt(i);
            }
            else ++i;
        }

        UIUpdate();

        // 다음 스테이지로 이동할 수 있는 발판 위에 있는 경우 다음 스테이지 진행
        if (m_Mobs.Count == 0)
        {
            if (m_NextText != null && m_Cor == null) m_Cor = StartCoroutine(NextStage());

            if (m_Mover.m_DependedLine == m_LevelLine || m_LevelLine == null)
            {
                if (m_NextText != null)
                {
                    StopCoroutine(m_Cor);
                    m_NextText.gameObject.SetActive(false);
                }

                if (m_NextWall != null) StartCoroutine(DestroyScale(m_NextWall));
                if (m_PrevWall != null) StartCoroutine(DestroyScale(m_PrevWall));
                if (m_CurrentWall != null) StartCoroutine(CreateScale(m_CurrentWall));
                Destroy(gameObject, 0.5f);
                m_Clear = true;
            }
        }
    }

    private void OnDestroy()
    {
        Destroy(m_NextWall);
        Destroy(m_PrevWall);
    }

    // UI 업데이트
    private void UIUpdate()
    {
        if (m_Mobs.Count == 0)
        {
            m_UIText.text = $"CLEAR";
        }
        else
        {
            m_UIText.text = $"★ {m_Mobs.Count}";
        }
    }

    // 벽 생성 효과
    IEnumerator CreateScale(GameObject obj)
    {
        obj.SetActive(true);

        while (true)
        {
            yield return new WaitForSeconds(0.01f);

            Vector3 temppos = obj.transform.localScale;
            temppos.x += 0.1f;
            temppos.y += 0.1f;
            obj.transform.localScale = temppos;

            if (temppos.y >= 3f) break;
        }
    }

    // 벽 파괴 효과
    IEnumerator DestroyScale(GameObject obj)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);

            Vector3 temppos = obj.transform.localScale;
            temppos.x -= 0.1f;
            temppos.y -= 0.1f;
            obj.transform.localScale = temppos;

            if (temppos.y <= 0.1f) break;
        }
    }

    // 다음 스테이지 강조 효과
    IEnumerator NextStage()
    {
        while (true)
        {
            m_NextText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            m_NextText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
