using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//UI��常��
public class BasePanel
{
    //UI��Ϣ
    public UIType _UIType { get; private set; }
    //UI������
    public UITool _UITool { get; private set; }
    //��������
    public PanelManager _PanelManager { get; private set; }
    //UI������
    public UIManager _UIManager { get; private set; }

    public BasePanel(UIType uiType)
    {
        _UIType = uiType;
    }

    //��ʼ��UITool
    public void Initialize(UITool tool)
    {
        _UITool = tool;
    }
    //��ʼ����������
    public void Initialize(PanelManager panelManager)
    {
        _PanelManager = panelManager;
    }
    //��ʼ��UI������
    public void Initialize(UIManager uiManager)
    {
        _UIManager = uiManager;
    }

    //UI����ʱִ�еĲ�����ֻ��ִ��һ��
    public virtual void OnEnter() { }

    //UI��ͣʱִ�еĲ���
    public virtual void OnPause()
    {
        //�����岻�����߼��
        _UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = false;
    }

    //UI����ʱִ�еĲ���
    public virtual void OnResume()
    {
        //������ָ����߼��
        _UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = true;
    }

    //UI�˳�ʱִ�еĲ���
    public virtual void OnExit()
    {
        _UIManager.DestroyUI(_UIType);
    }

    //��ʾһ�����
    public void Push(BasePanel panel) => _PanelManager?.Push(panel);
    //�ر�һ�����
    public void Pop() => _PanelManager?.Pop();
}
