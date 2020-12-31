using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleCtrl : MonoBehaviour
{
    /// <summary>
    /// 动画
    /// </summary>
    public Animator Animator;

    /// <summary>
    /// 角色类型
    /// </summary>
    public RoleType CurRoleType = RoleType.None;

    /// <summary>
    /// 角色信息
    /// </summary>
    public RoleInfoBase CurRoleInfo = null;

    /// <summary>
    /// 角色AI
    /// </summary>
    public IRoleAI CurRoleAI = null;

    /// <summary>
    /// 当前角色有限状态机管理器
    /// </summary>
    public RoleFSMMgr CurRoleFSMMgr = null;

    // Start is called before the first frame update
    void Start()
    {
        CurRoleFSMMgr = new RoleFSMMgr(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (CurRoleAI == null) return;
        CurRoleAI.DoAI();
        if(CurRoleFSMMgr!=null)
        {
            CurRoleFSMMgr.OnUpdate();
        }
    }
    /// <summary>
    /// 角色初始化
    /// </summary>
    /// <param name="roleType">类型</param>
    /// <param name="roleInfo">信息</param>
    /// <param name="ai">AI</param>
    public void Init(RoleType roleType,RoleInfoBase roleInfo,IRoleAI ai)
    {
        CurRoleType = roleType;
        CurRoleInfo = roleInfo;
        CurRoleAI = ai;
    }

    #region 角色动画控制方法

    public void ToIdle()
    {
        CurRoleFSMMgr.ChangeState(RoleState.Idle);
    }

    public void ToWalk()
    {
        CurRoleFSMMgr.ChangeState(RoleState.Walk);
    }
    public void ToRun()
    {
        CurRoleFSMMgr.ChangeState(RoleState.Run);
    }
    public void ToAttack()
    {
        CurRoleFSMMgr.ChangeState(RoleState.Attack);
    }

    public void ToHurt()
    {
        CurRoleFSMMgr.ChangeState(RoleState.Hurt);
    }
    public void ToDie()
    {
        CurRoleFSMMgr.ChangeState(RoleState.Die);
    }

    #endregion
}
