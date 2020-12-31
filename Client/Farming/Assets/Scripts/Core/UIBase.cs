using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public static UIBase Instance;

    private void Awake()
    {
        Instance = this as UIBase;
        OnAwake();
    }

    protected virtual void OnAwake()
    {

    }

    void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {

    }

}
