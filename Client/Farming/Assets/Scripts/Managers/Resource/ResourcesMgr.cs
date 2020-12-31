using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ResourcesMgr : SingleTon<ResourcesMgr>
{
    /// <summary>
    /// 资源类型
    /// </summary>
    public enum ResourceType
    {
        UIScence,
        Role,
    }

    private Dictionary<string,GameObject> m_PrefabDic;

    public ResourcesMgr()
    {
        m_PrefabDic = new Dictionary<string, GameObject>();
    }
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="type">资源类型</param>
    /// <param name="path">短路径</param>
    /// <param name="cache">是否放入缓存</param>
    /// <returns>资源物体</returns>
    public GameObject Load(ResourceType type,string path,bool cache = false)
    {
        
        StringBuilder pathstr = new StringBuilder();
        switch(type)
        {
            case ResourceType.UIScence:
                {
                    pathstr.Append(/*路径*/"");
                }
                break;
            case ResourceType.Role:
                {
                    pathstr.Append(/*路径*/"");
                }
                break;
            default:
                break;
        }
        pathstr.Append(path);
        if (m_PrefabDic.ContainsKey(pathstr.ToString()))
        {
            return m_PrefabDic[pathstr.ToString()];
        }
        else
        {
            GameObject obj = Resources.Load(pathstr.ToString()) as GameObject;
            if(cache)
            {
                m_PrefabDic[pathstr.ToString()] = obj;
            }
            return obj;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        m_PrefabDic.Clear();
        Resources.UnloadUnusedAssets();
    }
}
