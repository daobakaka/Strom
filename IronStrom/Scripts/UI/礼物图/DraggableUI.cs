using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // ʹ���¼�ϵͳ

public class DraggableUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private bool isDragging = false;
    private Vector2 originalPosition;
    // Start is called before the first frame update
    //void Start()
    //{
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //}
    public void OnPointerDown(PointerEventData eventData)
    {
        // ����Ƿ�Ϊ������
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;
            originalPosition = transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            transform.position = eventData.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // ����Ƿ�Ϊ����ɿ�
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false;
        }
    }
}
