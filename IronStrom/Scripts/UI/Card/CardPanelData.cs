using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPanelData : MonoBehaviour
{
    public Dictionary<ShiBingName, Sprite> CardSprite;
    //public TextMeshProUGUI 


    private void Awake()
    {
        CardSprite = new Dictionary<ShiBingName, Sprite>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
