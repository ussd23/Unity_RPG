using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Damage : MonoBehaviour
{
    private float m_PassedTime = 0f;
    private Text m_UIText;
    private Camera m_Camera;
    private Vector3 m_Position;

    // Start is called before the first frame update
    void Awake()
    {
        m_Camera = Camera.main;
        m_UIText = GetComponent<Text>();
        StartCoroutine(Rising());
    }

    // Update is called once per frame
    void Update()
    {
        m_PassedTime += Time.deltaTime;
    }

    public void SetPosition(Vector3 p_pos)
    {
        m_Position = p_pos + Vector3.up * 1.5f;
        transform.localScale = Vector3.one;
    }

    IEnumerator Rising()
    {
        while (true)
        {
            transform.position = m_Camera.WorldToScreenPoint(m_Position);
            yield return new WaitForSeconds(0.01f);

            m_Position.y += Time.deltaTime * 10f;

            Color tempcolor = m_UIText.color;
            tempcolor.a -= Time.deltaTime * 4f;
            m_UIText.color = tempcolor;

            if (m_PassedTime >= 0.5f) break;
        }
        Destroy(gameObject);
    }
}
