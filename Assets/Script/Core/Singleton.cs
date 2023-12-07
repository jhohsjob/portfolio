using UnityEngine;

public interface ISingleton
{
    void DestroySingleton();
}

public class Singleton<T> : ISingleton where T : Singleton<T>, new()
{
    protected static T _instance = null;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Create Singleton: " + typeof(T).Name);
                _instance = new T();
            }
            return _instance;
        }
    }

    public static T GetInstance()
    {
        return instance;
    }

    public static bool HasInstance()
    {
        return (_instance != null);
    }

    public Singleton()
    {
    }

    public void DestroySingleton()
    {
        Debug.Log("Destroy Singleton: " + typeof(T).Name);
        _instance?.OnDestroy();
        _instance = null;
    }

    protected virtual void OnDestroy()
    {
    }
}

public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
{
    protected static T _instance = null;
    protected bool _destroyed = false;

    public static T instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            _instance = FindObjectOfType<T>();
            if (_instance == null)
            {
                GameObject go = new GameObject();
                go.name = typeof(T).Name;
                _instance = go.AddComponent<T>();
            }

            return _instance;
        }
    }

    public static T GetInstance()
    {
        return instance;
    }

    public static bool HasInstance()
    {
        return (_instance != null) && (_instance._destroyed == false);
    }

    public virtual void Awake()
    {
        Debug.Log("Create Singleton: " + typeof(T).Name);

        // 에디터에서 여러개를 배치한 경우, 에러 로그가 표시되록 한다.
        T[] objs = FindObjectsOfType<T>();
        Debug.AssertFormat(objs.Length == 1, "Singleton {0} instance count: {1}", typeof(T).Name, objs.Length);

        DontDestroyOnLoad(gameObject);
        _instance = (T)this;

        OnAwake();
    }

    protected abstract void OnAwake();

    public void DestroySingleton()
    {
        if (HasInstance())
        {
            Debug.Log("Destroy Singleton: " + typeof(T).Name);
            _instance._destroyed = true;
            Destroy(_instance.gameObject);
            _instance.OnCallDestroy();
            _instance = null;
        }
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    protected virtual void OnCallDestroy()
    {
    }
}