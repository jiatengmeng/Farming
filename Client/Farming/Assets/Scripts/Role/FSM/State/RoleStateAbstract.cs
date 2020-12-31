using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色状态的抽象基类
/// </summary>
public abstract class RoleStateAbstract
{
    /// <summary>
    /// 当前角色有限状态机管理器
    /// </summary>
    public RoleFSMMgr CurRoleFSMMgr { get; private set; }

    /// <summary>
    /// 当前动画状态信息
    /// </summary>
    public AnimatorStateInfo CurRoleAnimatorStateInfo { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleFSMMgr">有限状态机管理器</param>
    public RoleStateAbstract(RoleFSMMgr roleFSMMgr)
    {
        CurRoleFSMMgr = roleFSMMgr;
    }
    /// <summary>
    /// 进入状态
    /// </summary>
    public virtual void OnEnter() { }

    /// <summary>
    /// 执行状态
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// 离开状态
    /// </summary>
    public virtual void OnExit() { }
}
