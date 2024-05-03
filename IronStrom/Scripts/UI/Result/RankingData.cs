using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingData : MonoBehaviour
{
    public TopData Top1;
    public TopData Top2;
    public TopData Top3;
    public GameObject Item;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //载入玩家数据，把玩家数据根据不同榜单进行排序
    public void LoadPlayeData(in List<PlayerData> playerList)
    {
        for (int i = 0; i < playerList.Count; ++i)
        {
            int Score = 0;
            if (gameObject.name == "日榜")
                Score = playerList[i].m_Score_total_day;
            else if (gameObject.name == "周榜")
                Score = playerList[i].m_Last_day_rank;
            else if (gameObject.name == "月榜")
                Score = playerList[i].m_Score_total_month;
            else if (gameObject.name == "本局排行榜")
                Score = playerList[i].m_GiftScore + (int)playerList[i].m_ATScore;

            if (i == 0)
            {
                Top1.HandImage.texture = Resources.Load<Sprite>(playerList[i].m_Avatar).texture;
                Top1.Name.text = playerList[i].m_Nick;
                Top1.Score.text = Score.ToString();
                Top1.Rank.text = $"世界排名{playerList[i].m_Rank}";
            }
            else if(i == 1)
            {
                Top2.HandImage.texture = Resources.Load<Sprite>(playerList[i].m_Avatar).texture;
                Top2.Name.text = playerList[i].m_Nick;
                Top2.Score.text = Score.ToString();
                Top2.Rank.text = $"世界排名{playerList[i].m_Rank}";
            }
            else if(i == 2)
            {
                Top3.HandImage.texture = Resources.Load<Sprite>(playerList[i].m_Avatar).texture;
                Top3.Name.text = playerList[i].m_Nick;
                Top3.Score.text = Score.ToString();
                Top3.Rank.text = $"世界排名{playerList[i].m_Rank}";
            }
            else if(i == 3)
            {
                var topData = Item.GetComponent<TopData>();
                topData.HandImage.texture = Resources.Load<Sprite>(playerList[i].m_Avatar).texture;
                topData.Name.text = playerList[i].m_Nick;
                topData.Score.text = Score.ToString();
                topData.Rank.text = playerList[i].m_Rank.ToString();
                if (topData.Index != null)
                    topData.Index.text = i.ToString();
            }
            else
            {
                var item = Instantiate(Item, Item.transform.parent);
                var itemTopData = item.GetComponent<TopData>();
                itemTopData.HandImage.texture = Resources.Load<Sprite>(playerList[i].m_Avatar).texture;
                itemTopData.Name.text = playerList[i].m_Nick;
                itemTopData.Score.text = Score.ToString();
                itemTopData.Rank.text = playerList[i].m_Rank.ToString();
                if (itemTopData.Index != null)
                    itemTopData.Index.text = i.ToString();
            }
        }


    }

}
