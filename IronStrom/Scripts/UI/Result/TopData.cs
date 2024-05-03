using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TopData : MonoBehaviour
{
    [Tooltip("前三名头像")] public RawImage HandImage;
    [Tooltip("前三名名字")] public TextMeshProUGUI Name;
    [Tooltip("前三名积分")] public TextMeshProUGUI Score;
    [Tooltip("前三名排名")] public TextMeshProUGUI Rank;
    [Tooltip("排名Index")] public TextMeshProUGUI Index;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
