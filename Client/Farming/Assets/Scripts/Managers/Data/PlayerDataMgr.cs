using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家数据管理
/// </summary>
public class PlayerDataMgr : SingleTon<PlayerDataMgr>
{
    public int NickName { get; set; }
    public float WalkSpeed = 4.0f;
    public float RunSpeed = 8.0f;
    public int HpLimit = 100;
    public int HpNow { get; set; }
    public int EnergyLimit = 200;
    public int EnergyNow { get; set; }

    private void ReaderDataByBinary(string fileName)
    {

    }
}
