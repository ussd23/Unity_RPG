using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // 편집 변수
    [SerializeField]
    private Stage[] m_Stages;
    [SerializeField]
    private Text m_UIText;
    [SerializeField]
    private Text m_GameOver;
    [SerializeField]
    private Text m_GameClear;

    // 내부 변수
    private int m_CurrentStage = 0;
    private GameObject m_Player = null;
    private bool m_Quit = false;

    private void Awake()
    {
        UI_Manager tempcom = UI_Manager.GetInstance;
        m_UIText = tempcom.m_UITextUp;
        m_GameOver = tempcom.m_GameOver;
        m_GameClear = tempcom.m_GameClear;

        m_UIText.text = "Stage 1";
    }

    private void Start()
    {
        m_Player = Info.PlayerInfo.gameObject;
    }

    // 스테이지와 관련된 업데이트
    void Update()
    {
        if (m_Quit) return;

        if (m_Player == null)
        {
            m_Quit = true;
            m_GameOver.gameObject.SetActive(true);
            StartCoroutine(GameQuit());
        }

        if (m_Stages[m_CurrentStage] == null)
        {
            // 마지막 스테이지가 아닌 경우 다음 레벨로 업데이트
            if (m_Stages.Length - 1 != m_CurrentStage)
            {
                ++m_CurrentStage;
                m_Stages[m_CurrentStage].gameObject.SetActive(true);

                if (m_Stages.Length - 1 == m_CurrentStage) m_UIText.text = "Boss Stage";
                else m_UIText.text = $"Stage {m_CurrentStage + 1}";
            }

            // 마지막 스테이지인 경우
            else
            {
                m_Quit = true;
                m_GameClear.gameObject.SetActive(true);
                StartCoroutine(GameQuit());
            }

        }
    }

    // 게임 종료
    IEnumerator GameQuit()
    {
        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("Title");
        //Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }
}
