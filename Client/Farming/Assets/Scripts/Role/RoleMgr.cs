using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleMgr : SingleTon<RoleMgr>
{
    /// <summary>
    /// 角色镜像字典
    /// </summary>
    private Dictionary<string, GameObject> m_RoleDic = new Dictionary<string, GameObject>();

    /// <summary>
    /// 克隆角色
    /// </summary>
    /// <param name="name">角色名称</param>
    /// <returns></returns>
    public GameObject LoadPlayer(string name)
    {
        GameObject obj = null;
        if(m_RoleDic.ContainsKey(name))
        {
            obj = m_RoleDic[name];
        }
        else
        {
            obj = ResourcesMgr.Instance.Load(ResourcesMgr.ResourceType.Role, string.Format("Player/{0}", name), cache: true);
            m_RoleDic.Add(name, obj);
        }
        return GameObject.Instantiate(obj);
    }

    public override void Dispose()
    {
        base.Dispose();
        m_RoleDic.Clear();
    }
}
