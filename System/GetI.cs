using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GetI<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance = null;

    public static T GetInstance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<T>();

                if (m_Instance == null)
                {
                    string componentName = typeof(T).ToString();
                    GameObject findObject = GameObject.Find(componentName);

                    if (findObject != null)
                    {
                        m_Instance = findObject.AddComponent<T>();
                    }
                }
            }

            return m_Instance;
        }
    }
}
