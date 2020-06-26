using Photon.Pun;

public class N_Singleton<T> : MonoBehaviourPunCallbacks where T : N_Singleton<T>
{
    public static T instance { private set; get; }
    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }
    protected virtual void OnDestroy()
    {
        if (instance != null && instance == this)
            instance = null;
    }
}
