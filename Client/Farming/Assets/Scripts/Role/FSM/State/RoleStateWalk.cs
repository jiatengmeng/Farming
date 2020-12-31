using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 走路
/// </summary>
public class RoleStateWalk : RoleStateAbstract
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleFSMMgr">角色有限状态机管理器</param>
    public RoleStateWalk(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {

    }
    /// <summary>
    /// 进入动画
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        CurRoleFSMMgr.CurRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToWalk.ToString(), true);
    }

    /// <summary>
    /// 动画进行中
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
        CurRoleAnimatorStateInfo = CurRoleFSMMgr.CurRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
        if (CurRoleAnimatorStateInfo.IsName(RoleAnimatorName.Walk.ToString()))
        {
            CurRoleFSMMgr.CurRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurState.ToString(), (int)RoleState.Walk);
        }
    }

    /// <summary>
    /// 离开动画
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        CurRoleFSMMgr.CurRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToWalk.ToString(), false);
    }
}
