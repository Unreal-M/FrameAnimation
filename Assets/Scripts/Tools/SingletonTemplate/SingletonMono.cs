/********************************************************************
	作  者：	        Unreal-M
	文件说明：	单例类 针对Mono
*********************************************************************/
using UnityEngine;
using System.Collections;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T instance
    {
        get { return _instance; }
    }

    /// <summary>
    /// 第一 初始化
    /// </summary>
    private void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = GetComponent<T>();
        if (!transform.parent)
        {
            DontDestroyOnLoad(gameObject);
        }

        OnSingletonInit();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void OnSingletonInit() { }
}
