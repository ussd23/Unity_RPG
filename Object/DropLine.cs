using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLine : MonoBehaviour
{
    public bool m_isDropLine = true;
    public Transform m_RevPosition = null;
    public float m_Damage = 20f;

    private void Awake()
    {
        if (m_RevPosition == null) m_RevPosition = transform.GetChild(0).transform;
        transform.GetComponent<LineRenderer>().startColor = Color.gray;
        transform.GetComponent<LineRenderer>().endColor = Color.gray;
    }


}
