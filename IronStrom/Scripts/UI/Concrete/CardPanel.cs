using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : BasePanel
{
    static readonly string path = "UI/Prefabs/CardPanel";

    public CardPanel() : base(new UIType(path)) { }

    public override void OnEnter()
    {
        var CardMager = CardManager.Instance;


        AddRandomCard(in CardMager);


    }


    //将随机卡牌图片和个数添加到Card面板上
    void AddRandomCard(in CardManager CardMager)
    {
        for (int i = 0; i < CardMager.randomCard.Count; ++i)
        {
            var randcard = CardMager.randomCard[i];

            var card = _UITool.FindChildGameObject($"Card_{i + 1}");//("Card_" + i.ToString());
            if (card == null)
            {
                Debug.Log("没有Card");
                continue;
            }
            var sprite = SelectCardImage(randcard.m_ImagePath);
            if (sprite != null)
            {
                var image = card.GetComponent<Image>();
                if (image)
                    image.sprite = sprite;
            }
            else
                Debug.Log($"没有{randcard.m_ImagePath}的图片");

            var cardData = card.GetComponent<CardData>();
            if (cardData)
            {
                cardData.txt.text = "数量 X " + randcard.m_Num.ToString();
                cardData.Nametxt.text = randcard.m_Name;
            }

        }
    }

    //选择图片
    Sprite SelectCardImage(string sbName)
    {
        return Resources.Load<Sprite>(("UI/CardImge/" + sbName.ToString()));
    }
}
