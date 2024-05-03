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

        scoreText1.text = "����GPU���ٵ�Entity��λ��" + teamManager.OnGpuEntityNum.ToString();
        scoreText2.text = "û��GPU���ٵ�Entity��λ��" + teamManager.NoGpuEntityNum.ToString();
        scoreText3.text = "��ͳ������Entity��λ��" + teamManager.UnityAniEntityNum.ToString() +
                    "           Entityʿ���ܹ���" + teamManager.TotalEntityNum.ToString();
        scoreText4.text = "�ӵ�������" + teamManager.BulletNum.ToString() +
                    "\n�������Ӹ�����" + teamManager.AttackBoxNum.ToString() +
                    "\n����1���ޱ��Ŷ�����"+ teamManager.Tema1_LikeSoldierQueueNum +
                    "\n����2���ޱ��Ŷ�����"+ teamManager.Tema2_LikeSoldierQueueNum +
                    "\n\nÿ��" + teamManager.DetectionNum + "֡���һ��" +
                    "\n�Զ�������� " + teamManager.SelectedGiftNum;

        var SelectedPlayer = teamManager.SelectedPlayer;
        if (SelectedPlayer != null)
        {
            scoreText5.text = $"��ǰѡ�еĽ�ɫ��: {SelectedPlayer.m_Team} �� {SelectedPlayer.m_Nick}";
        }
        

    }
    void DetectionFrameRateCtrl(ref TeamManager teamManager)//���֡�ʿ���
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            teamManager.DetectionNum += 1;
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            teamManager.DetectionNum -= 1;
        if (teamManager.DetectionNum < 0)
            teamManager.DetectionNum = 0;
    }
    void MaxAllLikeShiBingCtrl(ref TeamManager teamManager)//ͬ�����ʿ����������
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
