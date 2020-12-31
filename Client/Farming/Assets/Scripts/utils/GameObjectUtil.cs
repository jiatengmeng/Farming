using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameObjectUtil
{
    /// <summary>
    /// 获取或创建组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="obj">组件所在物体</param>
    /// <returns>组件</returns>
    public static T GetOrCreateComponent<T>(this GameObject obj) where T:MonoBehaviour
    {
        T t = obj.GetComponent<T>();
        if(t==null)
        {
            t = obj.AddComponent<T>();
        }
        return t;
    }
}
