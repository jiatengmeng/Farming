using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色有限状态机管理器
/// </summary>
public class RoleFSMMgr
{
    /// <summary>
    /// 当前角色控制器
    /// </summary>
    public RoleCtrl CurRoleCtrl { get; private set; }

    /// <summary>
    /// 当前角色状态枚举
    /// </summary>
    public RoleState CurRoleState { get; private set; }

    /// <summary>
    /// 当前角色状态类
    /// </summary>
    private RoleStateAbstract m_CurRoleStateAbstract = null;

    private Dictionary<RoleState, RoleStateAbstract> m_RoleStateDic;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="curRoleCtrl">角色控制器</param>
    public RoleFSMMgr(RoleCtrl curRoleCtrl)
    {
        CurRoleCtrl = curRoleCtrl;
        m_RoleStateDic = new Dictionary<RoleState, RoleStateAbstract>();
        m_RoleStateDic[RoleState.Idle] = new RoleStateIdle(this);
        m_RoleStateDic[RoleState.Walk] = new RoleStateWalk(this);
        m_RoleStateDic[RoleState.Run] = new RoleStateRun(this);
        m_RoleStateDic[RoleState.Attack] = new RoleStateAttack(this);
        m_RoleStateDic[RoleState.Hurt] = new RoleStateHurt(this);
        m_RoleStateDic[RoleState.Die] = new RoleStateDie(this);

        if(m_RoleStateDic.ContainsKey(CurRoleState))
        {
            m_CurRoleStateAbstract = m_RoleStateDic[CurRoleState];
        }
    }

    #region OnUpDate 每帧执行
    /// <summary>
    /// 每帧执行
    /// </summary>
    public void OnUpdate()
    {
        if(m_CurRoleStateAbstract!=null)
        {
            m_CurRoleStateAbstract.OnUpdate();
        }
    }
    #endregion

    /// <summary>
    /// 状态变更
    /// </summary>
    /// <param name="newState">新状态</param>
    public void ChangeState(RoleState newState)
    {
        if (CurRoleState == newState) return;
        if (!m_RoleStateDic.ContainsKey(CurRoleState)) return;

        //退出原状态
        if (m_CurRoleStateAbstract != null)
        {
            m_CurRoleStateAbstract.OnExit();
        }

        //进入新状态
        CurRoleState = newState;
        m_CurRoleStateAbstract = m_RoleStateDic[CurRoleState];
        m_CurRoleStateAbstract.OnEnter();

    }

}
