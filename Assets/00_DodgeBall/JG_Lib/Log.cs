using UnityEngine;

public class Log : MonoBehaviour
{
    public static Log Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<Log>();
            }
            if(instance == null)
            {
                GameObject g = Resources.Load<GameObject>("Log");
                if (g)
                {
                    instance = Instantiate(g).GetComponent<Log>();
                }
            }
            return instance;
        }
    }
    static Log instance = null;

    public bool CanLogMessage => CanLog(LogLevel.All);
    public bool CanLogWarnings => CanLog(LogLevel.Warnings);
    public bool CanLogErrors => CanLog(LogLevel.Errors);
    public bool CanLogL0 => CanLog(LogLevel.L0);

    public enum LogLevel { L0, All, Warnings, Errors }

    [SerializeField] LogLevel logLevel = LogLevel.All;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void OnDestroy()
    {
        if(instance != null && instance == this)
        {
            instance = null;
        }
    }

    public static void LogL0(object o, UnityEngine.Object ctx = null)
    {
#if UNITY_EDITOR
        if (Instance && !Instance.CanLogL0)
            return;
#endif
        Debug.Log(o, ctx);
    }
    public static void Message(object o,UnityEngine.Object ctx = null)
    {
#if UNITY_EDITOR
        if (Instance && !Instance.CanLogMessage)
            return;
#endif
        Debug.Log(o, ctx);
    }
    public static void Write(object o,UnityEngine.Object ctx = null)
    {
        Message(o,ctx);
    }
    public static void LogLog(object o, UnityEngine.Object ctx = null)
    {
        Message(o, ctx);
    }
    public static void Warning(object o, UnityEngine.Object ctx = null)
    {
#if UNITY_EDITOR
        if (Instance && !Instance.CanLogWarnings)
            return;
#endif
        Debug.LogWarning(o, ctx);
    }
    public static void Error(object o, UnityEngine.Object ctx = null)
    {
        Debug.LogError(o, ctx);
    }
    public bool CanLog(LogLevel logLevel)
    {
        return (int)logLevel >= (int)this.logLevel;
    }
}