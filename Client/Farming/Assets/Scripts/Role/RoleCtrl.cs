using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleCtrl : MonoBehaviour
{
    #region 测试用
    private Vector3 addCubePos;
    public GameObject cube;
    public GameObject wheelPb;
    private Dictionary<Vector3,GameObject> builtObj = new Dictionary<Vector3, GameObject>();
    #endregion

    /// <summary>
    /// 走的速度
    /// </summary>
    public float WalkSpeed = 4.0f;

    /// <summary>
    /// 跑速度
    /// </summary>
    public float RunSpeed = 8.0f;

    /// <summary>
    /// 移动方向
    /// </summary>
    public Vector3 CurDirection { get; set; }

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

    /// <summary>
    /// 角色控制器
    /// </summary>
    public CharacterController CurCharacterController { get; internal set; }

    // Start is called before the first frame update
    void Start()
    {
        CurCharacterController = this.GetComponent<CharacterController>();
        CurDirection = Vector3.zero;
        CurRoleFSMMgr = new RoleFSMMgr(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Rect rt = new Rect(0,0,Screen.width,Screen.height);
            CameraMgr.Instance.CaptureCamera(Camera.main, rt);
        }
        CurDirection = (Input.GetAxis("Horizontal") * Vector3.right + Input.GetAxis("Vertical") * Vector3.forward).normalized;
        if (CurDirection != Vector3.zero)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                ToRun();
            }
            else
            {
                ToWalk();
            }
        }
        CameraAutoFollow();
        //BuildObject();
        //if (CurRoleAI == null) return;
        //CurRoleAI.DoAI();
        if (CurRoleFSMMgr!=null)
        {
            CurRoleFSMMgr.OnUpdate();
        }

        if(CurRoleType == RoleType.MainPlayer)
        {

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

    private void CameraAutoFollow()
    {
        if (CameraMgr.Instance == null) return;
        CameraMgr.Instance.transform.position = gameObject.transform.position;
        CameraMgr.Instance.AutoLookAt(gameObject.transform.position);
    }

    private void BuildObject()
    {
        if (GetMouseRayPoint(out addCubePos))
        {
            cube.transform.position = addCubePos;
            if (Input.GetMouseButtonDown(0))
            {
                GameObject obj = Instantiate(wheelPb, addCubePos, wheelPb.transform.rotation);
                builtObj.Add(addCubePos, obj);
            }
        }
    }

    bool GetMouseRayPoint(out Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo,Mathf.Infinity,LayerMask.GetMask("Floor")))
        {
            pos = FromWorldPositionToCubePosition(hitInfo.point);
            if (builtObj.ContainsKey(addCubePos))
            {
                pos = Vector3.zero;
                Debug.Log("位置被占用");
                return false;
            }
            return true;
        }
        pos = Vector3.zero;
        return false;
    }
    public static Vector3 FromWorldPositionToCubePosition(Vector3 position)
    {
        Vector3 resut = Vector3.zero;
        resut.x = (int)(position.x - 0.5f)+0.5f;
        resut.y = (int)(position.y - 0.5f)+0.5f;
        resut.z = (int)(position.z - 0.5f)+0.5f;
        Debug.Log(resut);
        return resut;
    }
}
