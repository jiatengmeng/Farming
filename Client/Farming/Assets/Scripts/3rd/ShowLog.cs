using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLog : MonoBehaviour
{
    private List<string> mLogEntity = new List<string>();
    private void Awake()
    {
        Application.logMessageReceived += (condtion, tracky, logType) =>
        {
            mLogEntity.Add(string.Format("{0}\n{1}", condtion, tracky));
        };
    }
    //测试用打Log
    private void Start()
    {
        curTime = Time.realtimeSinceStartup;
        mShowLog = false;
    }
    float curTime = 0;
    private void Update()
    {
        //if (Time.realtimeSinceStartup - curTime > 1f)
        //{
        //    curTime = Time.realtimeSinceStartup;
        //    Debug.Log(Time.realtimeSinceStartup);
        //    Debug.LogError(Time.realtimeSinceStartup);
        //}

    }
    //在中上角
    private Rect mLogWindow = new Rect(0, 0, Screen.width, Screen.height);
    private bool mShowLog = false;
    private Vector2 mScrollView = Vector2.zero;
    private void OnGUI()
    {
        if (GUI.Button(new Rect((Screen.width / 2)-50, 0, 100, 50), "显示日志"))
        {
            mShowLog = true;
        }
        if (mShowLog)
            mLogWindow = GUILayout.Window(0, mLogWindow, GUILogWindow, "输出的日志");
    }
    void GUILogWindow(int windowId)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("清空日志"))
        {
            mLogEntity.Clear();
        }
        if (GUILayout.Button("关闭窗口"))
        {
            mShowLog = false;
        }
        GUILayout.EndHorizontal();
        //滚动条日志：
        mScrollView = GUILayout.BeginScrollView(mScrollView);
        foreach (string log in mLogEntity)
        {
            GUILayout.TextArea(log, GUILayout.Height(70));
        }
        GUILayout.EndScrollView();
    }
}
