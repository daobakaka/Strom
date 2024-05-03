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


    //���������ͼƬ�͸�����ӵ�Card�����
    void AddRandomCard(in CardManager CardMager)
    {
        for (int i = 0; i < CardMager.randomCard.Count; ++i)
        {
            var randcard = CardMager.randomCard[i];

            var card = _UITool.FindChildGameObject($"Card_{i + 1}");//("Card_" + i.ToString());
            if (card == null)
            {
                Debug.Log("û��Card");
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
                Debug.Log($"û��{randcard.m_ImagePath}��ͼƬ");

            var cardData = card.GetComponent<CardData>();
            if (cardData)
            {
                cardData.txt.text = "���� X " + randcard.m_Num.ToString();
                cardData.Nametxt.text = randcard.m_Name;
            }

        }
    }

    //ѡ��ͼƬ
    Sprite SelectCardImage(string sbName)
    {
        return Resources.Load<Sprite>(("UI/CardImge/" + sbName.ToString()));
    }
}
