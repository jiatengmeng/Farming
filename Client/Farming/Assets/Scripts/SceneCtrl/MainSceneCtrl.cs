using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneCtrl : MonoBehaviour
{

    private void Awake()
    {
        //加载当前页面ui

        //监听移动
    }

    // Start is called before the first frame update
    void Start()
    {
        //加载玩家
        GameObject player = RoleMgr.Instance.LoadPlayer(""/*玩家预制体名称*/);
        //当前玩家控制器
        GlobeInit.Instance.CurPlayer = player.GetComponent<RoleCtrl>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        //销毁移动监听
    }
}
