using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 跑步
/// </summary>
public class RoleStateRun : RoleStateAbstract
{
    /// <summary>
    /// 转身速度
    /// </summary>
    private float m_RotationSpeed = 0.2f;

    /// <summary>
    /// 转身的目标方向
    /// </summary>
    private Quaternion m_TargetQuaternion;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleFSMMgr">角色有限状态机管理器</param>
    public RoleStateRun(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {

    }
    /// <summary>
    /// 进入动画
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        CurRoleFSMMgr.CurRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToRun.ToString(), true);
        CurRoleFSMMgr.CurRoleCtrl.Animator.SetFloat(ToAnimatorCondition.Speed.ToString(), CurRoleFSMMgr.CurRoleCtrl.RunSpeed);
    }

    /// <summary>
    /// 动画进行中
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
        CurRoleAnimatorStateInfo = CurRoleFSMMgr.CurRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
        //修改当前动画状态
        if (CurRoleAnimatorStateInfo.IsName(RoleAnimatorName.Run.ToString()))
        {
            CurRoleFSMMgr.CurRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurState.ToString(), (int)RoleState.Run);
        }
        else
        {
            CurRoleFSMMgr.CurRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurState.ToString(), 0);
        }

        if (CurRoleFSMMgr.CurRoleCtrl.CurDirection != Vector3.zero)
        {
            Vector3 direction = CurRoleFSMMgr.CurRoleCtrl.CurDirection * CurRoleFSMMgr.CurRoleCtrl.RunSpeed * Time.deltaTime;
            direction.y = 0;

            //让角色缓慢转身
            if (m_RotationSpeed <= 1)
            {
                m_RotationSpeed += 10f * Time.deltaTime;
                m_TargetQuaternion = Quaternion.LookRotation(direction);
                CurRoleFSMMgr.CurRoleCtrl.transform.rotation = Quaternion.Lerp(CurRoleFSMMgr.CurRoleCtrl.transform.rotation, m_TargetQuaternion, m_RotationSpeed);

                if (Quaternion.Angle(CurRoleFSMMgr.CurRoleCtrl.transform.rotation, m_TargetQuaternion) < 1)
                {
                    m_RotationSpeed = 0;
                }
            }

            CurRoleFSMMgr.CurRoleCtrl.CurCharacterController.Move(direction);
        }
        else
        {
            CurRoleFSMMgr.CurRoleCtrl.ToIdle();
        }
    }

    /// <summary>
    /// 离开动画
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        CurRoleFSMMgr.CurRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToRun.ToString(), false);
        CurRoleFSMMgr.CurRoleCtrl.Animator.SetFloat(ToAnimatorCondition.Speed.ToString(), 0);
    }
}
