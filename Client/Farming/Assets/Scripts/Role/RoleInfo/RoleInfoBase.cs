using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色信息基类
/// </summary>
public class RoleInfoBase
{
    /// <summary>
    /// 角色服务器ID
    /// </summary>
    public int RoleServerID;
    /// <summary>
    /// 角色编号
    /// </summary>
    public int RoleID;
    /// <summary>
    /// 昵称
    /// </summary>
    public string NickName;

    /// <summary>
    /// 最大血量
    /// </summary>
    public int MaxHP;

    /// <summary>
    /// 当前血量
    /// </summary>
    public int CurHP;
}
