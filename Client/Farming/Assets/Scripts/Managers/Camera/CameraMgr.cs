using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMgr : MonoBehaviour
{
    public static CameraMgr Instance;

    /// <summary>
    /// 相机上下控制
    /// </summary>
    [SerializeField]
    private Transform CameraUpAndDown;

    /// <summary>
    /// 相机缩放父物体
    /// </summary>
    [SerializeField]
    private Transform CameraZoomContainer;

    /// <summary>
    /// 相机父物体
    /// </summary>
    [SerializeField]
    private Transform CameraContainer;

    /// <summary>
    /// 相机的左右旋转，上下旋转，以及缩放速度
    /// x=上下，y=左右，z=缩放
    /// </summary>
    [SerializeField]
    private Vector3 CameraSpeed;
    private void Awake()
    {
        if(Instance==null)
        {
            Instance = new CameraMgr();
        }
    }
    /// <summary>
    /// 相机左右旋转
    /// </summary>
    /// <param name="type">type ? 左 : 右 </param>
    private void setCameraRotate(bool type)
    {
        this.transform.Rotate(Vector3.up * CameraSpeed.y * (type ? 1 : -1) * Time.deltaTime);
    }
    /// <summary>
    /// 相机上下
    /// </summary>
    /// <param name="type">type ? 上 : 下</param>
    private void setCameraUpAndDown(bool type)
    {
        CameraUpAndDown.Rotate(Vector3.right * CameraSpeed.x * (type ? 1 : -1) * Time.deltaTime);
        CameraUpAndDown.eulerAngles = Vector3.right * (Mathf.Clamp(CameraUpAndDown.eulerAngles.z, 30, 85));
    }

    private void setCameraZoom(bool type)
    {
        CameraContainer.Translate(Vector3.forward * CameraSpeed.z * (type ? 1 : -1) * Time.deltaTime);
        CameraUpAndDown.position = Vector3.forward * (Mathf.Clamp(CameraUpAndDown.eulerAngles.z, CameraUpAndDown.eulerAngles.z - 5, CameraUpAndDown.eulerAngles.z + 5));
    }
}
