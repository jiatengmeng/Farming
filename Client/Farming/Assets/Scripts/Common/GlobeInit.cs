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

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
