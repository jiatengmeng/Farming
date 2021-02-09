using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : MonoBehaviour
{
    private Button OKbtn;
    private Text TipText;
    private ScrollRect TipScrollView;

    float tiptextheight = 0.0f;
    //float testtime = 1.0f;
    //float testtiming = 0.0f;
    //int x = 0;
    private void Awake()
    {
        OKbtn = this.transform.Find("TipBackground").Find("OK").GetComponent<Button>();
        TipScrollView = this.transform.Find("TipBackground").Find("TipScrollView").GetComponent<ScrollRect>();
        TipText = TipScrollView.content.transform.Find("TipText").GetComponent<Text>();

        OKbtn.onClick.AddListener(OnOKButtonClick);
    }

    private void Update()
    {
        //testtiming += Time.deltaTime;
        //if (testtiming >= testtime)
        //{
        //    TipText.text += x + "\n";
        //    x++;
        //    testtiming = 0.0f;
        //}
        if (tiptextheight != TipText.gameObject.GetComponent<RectTransform>().rect.height)
        {
            tiptextheight = TipText.gameObject.GetComponent<RectTransform>().rect.height;
            TipText.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(TipText.gameObject.GetComponent<RectTransform>().rect.width/2, -tiptextheight / 2, 0);
            TipScrollView.content.sizeDelta = new Vector2(TipScrollView.content.sizeDelta.x, tiptextheight);
        }
    }

    private void OnOKButtonClick()
    {
        this.gameObject.SetActive(false);
    }
}
