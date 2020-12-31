using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private Dictionary<string, string> testDic = new Dictionary<string, string>();
    // Start is called before the first frame update
    void Start()
    {
        testDic["hsc"] = "黄世琛";
        Debug.Log(testDic["hsc"]);
        testDic["hsc"] = "huangshichen";
        Debug.Log(testDic["hsc"]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
