using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 缓存池模块
/// </summary>
public class PoolMgr : SingleTon<PoolMgr>
{
    //缓存池容器
    public Dictionary<string, List<GameObject>> poolDic = new Dictionary<string, List<GameObject>>();

    /// <summary>
    /// 获取缓存池中的物品
    /// </summary>
    /// <param name="name">物品的路径</param>
    /// <returns></returns>
    public GameObject GetObj(string name)
    {
        GameObject obj = null;
        if(poolDic.ContainsKey(name)&&poolDic[name].Count>0)
        {
            obj = poolDic[name][0];
            poolDic[name].RemoveAt(0);
        }
        else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;
        }
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 将物品放入缓存池
    /// </summary>
    /// <param name="name">物品路径</param>
    /// <param name="obj">需要缓存的物体</param>
    public void PushObj(string name,GameObject obj)
    {
        obj.SetActive(false);
        if(poolDic.ContainsKey(name))
        {
            poolDic[name].Add(obj);
        }
        else
        {
            poolDic.Add(name, new List<GameObject>() { obj });
        }
    }
}
