using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntoNoticeFrame : MonoBehaviour//进场玩家通知
{
    public Image PlayerHeadshot;//玩家头像
    public TextMeshProUGUI playerName;//玩家名字
    public TextMeshProUGUI IntoTeam;//玩家加入的队伍

    //不同排名头像框
    public GameObject IntoNoticeFrame1_3;
    public GameObject IntoNoticeFrame4_10;
    public GameObject IntoNoticeFrame11_30;
    public GameObject IntoNoticeFrame31_50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //通知特效关闭自己
    public void CloseActive()
    {
        IntoNoticeManager.Instance.Is_IntoNoticePlaying = false;
        gameObject.SetActive(false);
        IntoNoticeFrame1_3.SetActive(false);
        IntoNoticeFrame4_10.SetActive(false);
        IntoNoticeFrame11_30.SetActive(false);
        IntoNoticeFrame31_50.SetActive(false);
    }
}
