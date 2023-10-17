using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Movable : MonoBehaviour
    , IPointerUpHandler
    , IPointerDownHandler
    , IPointerMoveHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    protected UI_Manager m_Manager;
    protected bool m_isMove = false;
    protected RectTransform m_RectTransform;
    protected Vector2 m_Rect;
    protected Vector2 m_Difference;
    protected Image m_Image;
    protected Color m_DefaultColor;

    protected void Start()
    {
        m_Manager = UI_Manager.GetInstance;
        m_RectTransform = GetComponent<RectTransform>();
        m_Rect = m_RectTransform.sizeDelta / 2;
        m_Image = GetComponent<Image>();
    }

    // â �̵��� ���õ� ����
    public void OnPointerDown(PointerEventData eventData)
    {
        m_isMove = true;
        m_Difference = m_RectTransform.anchoredPosition - m_Manager.GetLocalPoint(eventData.position);
        m_Manager.SetFocus(this);
    }

    // â �̵�
    public void OnPointerMove(PointerEventData eventData)
    {
        if (!m_isMove) return;
        m_RectTransform.anchoredPosition = m_Manager.GetLocalPoint(eventData.position) + m_Difference;

        PosRevision();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isMove = false;
    }

    // UI �׷��� ������ ���̶���Ʈ ó���Ͽ� ������ �� �� �ֵ��� ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_Image.sprite != null) return;

        m_DefaultColor = m_Image.color;
        Color color = m_DefaultColor;
        color.a = 0.5f;
        m_Image.color = color;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_Image.sprite != null) return;

        m_Image.color = m_DefaultColor;
    }

    // ȭ�� �����δ� �̵���ų �� ������ ���� (ȭ�� ������ ����� ��� �ش� UI�� ��Ŀ���� ��쿡�� ȣ��)
    public void PosRevision()
    {
        if (m_RectTransform == null) return;

        Vector2 screen = m_Manager.GetScreenSize() / 2;
        Vector2 pos = m_RectTransform.localPosition;

        if (pos.x < -screen.x + m_Rect.x) pos.x = -screen.x + m_Rect.x;
        else if (pos.x > screen.x - m_Rect.x) pos.x = screen.x - m_Rect.x;

        if (pos.y < -screen.y + m_Rect.y) pos.y = -screen.y + m_Rect.y;
        else if (pos.y > screen.y - m_Rect.y) pos.y = screen.y - m_Rect.y;

        m_RectTransform.localPosition = pos;
    }
}
