using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    public static UIBase Instance;

    private void Awake()
    {
        Instance = this as UIBase;
        OnAwake();
    }

    void Start()
    {
        Button[] btnArr = GetComponentsInChildren<Button>(true);
        for (int i = 0; i < btnArr.Length; i++)
        {
            btnArr[i].onClick.AddListener(delegate ()
            {
                BtnClick(btnArr[i].gameObject);
            });
        }
        OnStart();
    }

    private void BtnClick(GameObject go)
    {
        OnBtnClick(go);
    }

    void OnDestroy()
    {
        BeforeOnDestroy();
    }


    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void BeforeOnDestroy(){}
    protected virtual void OnBtnClick(GameObject go) { }

}
