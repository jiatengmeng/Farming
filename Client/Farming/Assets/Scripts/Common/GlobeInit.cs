using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeInit : MonoBehaviour
{
    public static GlobeInit Instance;

    /// <summary>
    /// 玩家昵称
    /// </summary>
    [HideInInspector]
    public string PlayerNickName;

    /// <summary>
    /// 当前玩家
    /// </summary>
    [HideInInspector]
    public RoleCtrl CurPlayer;

    /// <summary>
    /// 当前资源路径
    /// </summary>
    [HideInInspector]
    public string ObjPath;
    private void Awake()
    {
        Instance = this;
        ObjPath = Application.dataPath;
        DontDestroyOnLoad(gameObject);
    }

}
