using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityNum : MonoBehaviour
{
    public TextMeshProUGUI scoreText1;
    public TextMeshProUGUI scoreText2;
    public TextMeshProUGUI scoreText3;
    public TextMeshProUGUI scoreText4;
    public TextMeshProUGUI scoreText5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var teamManager = TeamManager.teamManager;
        if (teamManager == null)
            return;
        DetectionFrameRateCtrl(ref teamManager);
        UpdateEntityNum(ref teamManager);
        MaxAllLikeShiBingCtrl(ref teamManager);
    }


    private void UpdateEntityNum(ref TeamManager teamManager)
    {

        scoreText1.text = "开启GPU加速的Entity单位：" + teamManager.OnGpuEntityNum.ToString();
        scoreText2.text = "没有GPU加速的Entity单位：" + teamManager.NoGpuEntityNum.ToString();
        scoreText3.text = "传统动画的Entity单位：" + teamManager.UnityAniEntityNum.ToString() +
                    "           Entity士兵总共：" + teamManager.TotalEntityNum.ToString();
        scoreText4.text = "子弹个数：" + teamManager.BulletNum.ToString() +
                    "\n攻击盒子个数：" + teamManager.AttackBoxNum.ToString() +
                    "\n队伍1点赞兵排队数："+ teamManager.Tema1_LikeSoldierQueueNum +
                    "\n队伍2点赞兵排队数："+ teamManager.Tema2_LikeSoldierQueueNum +
                    "\n\n每隔" + teamManager.DetectionNum + "帧检测一次" +
                    "\n自动送礼个数 " + teamManager.SelectedGiftNum;

        var SelectedPlayer = teamManager.SelectedPlayer;
        if (SelectedPlayer != null)
        {
            scoreText5.text = $"当前选中的角色是: {SelectedPlayer.m_Team} 的 {SelectedPlayer.m_Nick}";
        }
        

    }
    void DetectionFrameRateCtrl(ref TeamManager teamManager)//检测帧率控制
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            teamManager.DetectionNum += 1;
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            teamManager.DetectionNum -= 1;
        if (teamManager.DetectionNum < 0)
            teamManager.DetectionNum = 0;
    }
    void MaxAllLikeShiBingCtrl(ref TeamManager teamManager)//同屏最大士兵个数限制
    {
        if(Input.GetKey(KeyCode.F1)&&Input.GetKey(KeyCode.KeypadPlus))
        {
            teamManager.Tema1_LikeSoldierMaxNum += 10;
        }
        else if (Input.GetKey(KeyCode.F1) && Input.GetKey(KeyCode.KeypadMinus))
        {
            if (teamManager.Tema1_LikeSoldierMaxNum > 100)
                teamManager.Tema1_LikeSoldierMaxNum -= 10;
        }
        if (Input.GetKey(KeyCode.F2) && Input.GetKey(KeyCode.KeypadPlus))
        {
            teamManager.Tema2_LikeSoldierMaxNum += 10;
        }
        else if (Input.GetKey(KeyCode.F2) && Input.GetKey(KeyCode.KeypadMinus))
        {
            if (teamManager.Tema2_LikeSoldierMaxNum > 100)
                teamManager.Tema2_LikeSoldierMaxNum -= 10;
        }
    }
}
