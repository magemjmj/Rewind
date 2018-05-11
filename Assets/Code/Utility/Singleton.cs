using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> 
{
	private static T m_Instance = null;
    private static object m_Syncobj = new object();
    private static bool m_IsClosing = false;

    public static bool isInstanced
	{
		get { return m_Instance != null; }
	}

    public static T Instance
    {
        get { return GetManager(); }
    }

	public static T GetManager()
    {
        if (m_IsClosing) return null;

        lock (m_Syncobj)
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

                // Object not found, we create a temporary one
                if (m_Instance == null)
                {
                    // Debug.Log ("No instance of " + typeof(T).ToString () + ", a temporary one is created.");
                    m_Instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();

                    // Problem during the creation, this should not happen
                    if (m_Instance == null)
                    {
                        Debug.Log("Problem during the creation of " + typeof(T).ToString());
                    }

                }
            }
            return m_Instance;
        }
	}

	// If no other monobehaviour request the instance in an awake function
	// executing before this one, no need to search the object.
	private void Awake()
	{
        if (m_Instance == null)
        {
            //if not, set instance to this
            m_Instance = this as T;
        } else

        //If instance already exists and it's not this:
        if (m_Instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        Object.DontDestroyOnLoad(gameObject);
    }

    public static void Destroy()
    {
        if (m_Instance)
        {
            GameObject.Destroy(m_Instance.gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        // release reference on exit
        m_IsClosing = true;
    }
}
