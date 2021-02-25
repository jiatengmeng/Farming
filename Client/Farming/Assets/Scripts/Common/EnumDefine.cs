//这里放所有的enum

/// <summary>
/// 角色类型
/// </summary>
public enum RoleType
{
   None = -1,
   MainPlayer = 0,
   Monster = 1,
   Npc = 2,
}

/// <summary>
/// 角色状态
/// </summary>
public enum RoleState
{
    /// <summary>
    /// 无
    /// </summary>
    None = -1,
    /// <summary>
    /// 待机
    /// </summary>
    Idle = 0,
    /// <summary>
    /// 走路
    /// </summary>
    Walk = 1,
    /// <summary>
    /// 跑步
    /// </summary>
    Run = 2,
    /// <summary>
    /// 攻击
    /// </summary>
    Attack = 3,
    /// <summary>
    /// 受击
    /// </summary>
    Hurt = 4,
    /// <summary>
    /// 死亡
    /// </summary>
    Die = 5,
}

/// <summary>
/// 角色动画名称
/// </summary>
public enum RoleAnimatorName
{
    Idle,
    FishIdle,
    Walk,
    Run,
    Hurt,
    DeathForward,
    Attack,
}


/// <summary>
/// 
/// </summary>
public enum ToAnimatorCondition
{
    ToIdle,
    ToFishIdle,
    ToWalk,
    ToRun,
    ToHurt,
    ToDead,
    ToAttack,
    CurState,
    Speed,
}

/// <summary>
/// 游戏物体层级
/// </summary>
public enum LayerType
{
    Floor = 9,
}