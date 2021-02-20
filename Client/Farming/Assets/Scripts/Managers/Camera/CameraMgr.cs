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
    /// 初始化
    /// </summary>
    public void Init()
    {
        CameraUpAndDown.transform.localEulerAngles = new Vector3(0, 0, Mathf.Clamp(CameraUpAndDown.transform.localEulerAngles.z, 30f, 85f));
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

    /// <summary>
    /// 相机前后（缩放）
    /// </summary>
    /// <param name="type"></param>
    private void setCameraZoom(bool type)
    {
        CameraContainer.Translate(Vector3.forward * CameraSpeed.z * (type ? 1 : -1) * Time.deltaTime);
        CameraUpAndDown.position = Vector3.forward * (Mathf.Clamp(CameraUpAndDown.eulerAngles.z, CameraUpAndDown.eulerAngles.z - 5, CameraUpAndDown.eulerAngles.z + 5));
    }

    public void AutoLookAt(Vector3 pos)
    {
        CameraContainer.LookAt(pos);
    }

    public Texture2D CaptureCamera(Camera camera,Rect rect)
    {
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        camera.targetTexture = rt;
        camera.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        camera.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        string fileName = Application.dataPath + "/GameData/Picture/Screenshot.png";
        System.IO.File.WriteAllBytes(fileName, bytes);
        Debug.Log(string.Format("截屏成功: {0}", fileName));
        return screenShot;
    }
}
