using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gift_NotificationData : MonoBehaviour
{

    [Tooltip("送礼头像")] public Image Headshot;//送礼头像
    [Tooltip("送礼玩家名字")] public TextMeshProUGUI PlayerName;
    [Tooltip("礼物名字")] public TextMeshProUGUI GiftName;
    [Tooltip("还差多少升级")] public TextMeshProUGUI LevelUp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
