using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��������
public class PanelManager
{
    private static readonly Lazy<PanelManager> lazy = new Lazy<PanelManager>(() => new PanelManager());
    public static PanelManager instance { get { return lazy.Value; } }

    //�洢UI���
    private Stack<BasePanel> PanelStack;

    //UI������
    public UIManager _UIManager;
    private BasePanel panel;

    public PanelManager()
    {
        PanelStack = new Stack<BasePanel>();
        _UIManager = new UIManager();
    }

    //UI����ջ����,�˲�������ʾһ��UI
    public void Push(BasePanel nextPanel)
    {
        if (PanelStack.Count > 0)
        {
            panel = PanelStack.Peek();//��ȡջ������壬
            panel.OnPause();//Ȼ����ͣ��
        }
        PanelStack.Push(nextPanel);//���������
        GameObject panelGo = _UIManager.GetSingleUI(nextPanel._UIType);//ʵ������
        nextPanel.Initialize(new UITool(panelGo));
        nextPanel.Initialize(instance);
        nextPanel.Initialize(_UIManager);
        nextPanel.OnEnter();
    }

    //����ջ�����
    public void Pop()
    {
        if (PanelStack.Count > 0)
            PanelStack.Pop().OnExit();
        if (PanelStack.Count > 0)
            PanelStack.Peek().OnResume();
    }
    //ִ����������OnExit()
    public void PopAll()
    {
        while (PanelStack.Count > 0)
            PanelStack.Pop().OnExit();
    }

}
