using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraZone : MonoBehaviour
{
    [SerializeField]
    protected bool m_IsLook = false;
    [SerializeField]
    protected bool m_IsStaticCamera = false;
    [SerializeField]
    protected Transform m_ViewPosition;

    protected Collider m_Collider;
    protected Transform m_Player;

    private void Awake()
    {
        if (m_ViewPosition == null) m_ViewPosition = transform.GetChild(0).transform;
        if (!m_IsStaticCamera) m_ViewPosition.rotation = Quaternion.LookRotation(transform.position - m_ViewPosition.position);
    }

    private void Start()
    {
        m_Player = Info.PlayerInfo.transform;
    }

    private void Update()
    {
        if (!m_IsLook || m_Player == null) return;

        m_ViewPosition.rotation = Quaternion.LookRotation(m_Player.position - m_ViewPosition.position);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "PlayerModel")
        {
            Camera.main.GetComponent<CameraMove>().TargetChange(m_ViewPosition);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "PlayerModel")
        {
            Camera.main.GetComponent<CameraMove>().DefaultTarget();
        }
    }
}
