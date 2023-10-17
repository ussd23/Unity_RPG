using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window : UI_Movable
{
    public bool m_Modal = false;
    public string m_Notice = "";

    private Image m_ModalImage;
    private Text m_Text;

    // Start is called before the first frame update
    void Awake()
    {
        m_Text = transform.GetChild(1).GetComponent<Text>();
        if (m_Modal)
        {
            m_ModalImage = transform.GetChild(0).GetComponent<Image>();
            m_ModalImage.transform.SetParent(transform.parent);
        }
        transform.SetAsLastSibling();
    }

    // Update is called once per frame
    void OnEnable()
    {
        if (m_Modal) m_ModalImage.gameObject.SetActive(true);
        m_Text.text = m_Notice;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseWindow();
        }
    }

    public void CloseWindow()
    {
        if (m_Modal)
        {
            GetComponentInParent<UI_Manager>()?.UnModal();
            Destroy(m_ModalImage.gameObject);
        }
        Destroy(gameObject);
    }
}
