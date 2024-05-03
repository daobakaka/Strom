using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;

public class SettingPanel : BasePanel
{
    static readonly string path = "UI/Prefabs/SettingPanel";
    public SettingPanel() : base(new UIType(path)) { }


    Slider MusicSlider;//������������
    Slider EffectSlider;//��Ч��������
    public override void OnEnter()
    {
        //�������ý���ı��水ť
        _UITool.GetOrAddComponentInChildren<Button>("Save_Button").onClick.AddListener(()=>
        {
            Pop();
        });
        //��ʼ��������ЧSlider
        InitMusicEffectSlider();
        //��ʼ��������Ч��ʾ��ť
        InitGiftEffectsToggle();
        //��ʼ������ͼ
        InitGiftImage();
    }


    public override void OnExit()
    {
        //�ر����������ð�ť
        _UIManager.DestroyUI(_UIType);
        CloseMainPanelSettingToggle();
    }
    //��ʼ��������ЧSlider
    void InitMusicEffectSlider()
    {
        //����
        var audioMager = AudioManager.Instance;
        if (audioMager == null) return;
        MusicSlider = _UITool.GetOrAddComponentInChildren<Slider>("Scrollbar_Volume");
        MusicSlider.maxValue = 1;
        MusicSlider.minValue = 0;
        MusicSlider.value = audioMager.MusicSource.volume;
        MusicSlider.onValueChanged.AddListener(MusicHandleSliderValueChanged);
        ////��Ч
        //EffectSlider = _UITool.GetOrAddComponentInChildren<Slider>("Scrollbar_SoundEffects");
        //EffectSlider.maxValue = 1;
        //EffectSlider.minValue = 0;
        //EffectSlider.value = audioMager.EffectVolume;
        //EffectSlider.onValueChanged.AddListener(EffectHandleSliderValueChanged);
    }
    // ���ֻ���ֵ�仯ʱ���ô˷���
    void MusicHandleSliderValueChanged(float value)
    {
        // ������ƵԴ������Ϊ�����ֵ
        var audioMager = AudioManager.Instance;
        audioMager.MusicSource.volume = value;
    }
    // ��Ч����ֵ�仯ʱ���ô˷���
    void EffectHandleSliderValueChanged(float value)
    {
        var audioMager = AudioManager.Instance;
        audioMager.EffectVolume = value;
        audioMager.UpdateAllEffectVolume(value);
    }
    //�ر����������ð�ť
    void CloseMainPanelSettingToggle()
    {
        //���mian���
        var panelMager = PanelManager.instance;
        var uiMager = panelMager._UIManager;
        var mainPanel = uiMager.GetSingleUI("MainPanel");
        if (mainPanel == null) return;
        //���setting Toggle
        var setting = _UITool.FindChildGameObject(mainPanel, "Setting");
        if (setting == null) return;
        Toggle settingToggle = setting.GetComponent<Toggle>();
        settingToggle.onValueChanged.RemoveAllListeners(); // �Ƴ����м�����
        settingToggle.isOn = false;
        // ��Ҫʱ��������ض��ļ�����
        settingToggle.onValueChanged.AddListener(delegate { Push(new SettingPanel()); });
    }
    //ͬ��������Ч�Ƿ���ʾ��ť
    void SyncGiftEffectsToggle()
    {
        var gameRoot = GameRoot.Instance;
        if(gameRoot.Is_Gift1EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle��Ů������").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle��Ů���ر�").isOn = true;
        if (gameRoot.Is_Gift2EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle����Ȧ����").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle����Ȧ�ر�").isOn = true;
        if (gameRoot.Is_Gift3EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle������ؿ���").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle������عر�").isOn = true;
        if (gameRoot.Is_Gift4EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle��ħը������").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle��ħը���ر�").isOn = true;
        if (gameRoot.Is_Gift5EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle���ؿ�Ͷ����").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle���ؿ�Ͷ�ر�").isOn = true;
        if (gameRoot.Is_Gift6EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle�������俪��").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle��������ر�").isOn = true;

    }
    //��ʼ��������Ч��ʾ��ť
    void InitGiftEffectsToggle()
    {
        //��ͬ����ť
        SyncGiftEffectsToggle();
        var gameRoot = GameRoot.Instance;
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle��Ů���ر�").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift1EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle��Ů������").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift1EffectsCanPlayed = true;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle����Ȧ�ر�").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift2EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle����Ȧ����").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift2EffectsCanPlayed = true;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle������عر�").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift3EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle������ؿ���").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift3EffectsCanPlayed = true;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle��ħը���ر�").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift4EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle��ħը������").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift4EffectsCanPlayed = true;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle���ؿ�Ͷ�ر�").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift5EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle���ؿ�Ͷ����").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift5EffectsCanPlayed = true;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle��������ر�").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift6EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle�������俪��").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift6EffectsCanPlayed = true;
        });
    }
    //��ʼ������ͼ
    void InitGiftImage()
    {
        var gameRoot = GameRoot.Instance;
        //��ͬ������ͼ��ť
        switch(gameRoot.giftImage)
        {
            case GiftImage.NUll: _UITool.GetOrAddComponentInChildren<Toggle>("����ʾToggle").isOn = true;break;
            case GiftImage.Heel: _UITool.GetOrAddComponentInChildren<Toggle>("��ʾ���Toggle").isOn = true;break;
            case GiftImage.VErtical: _UITool.GetOrAddComponentInChildren<Toggle>("��ʾ����Toggle").isOn = true;break;
        }

        //������ʾ�����尴ť
        _UITool.GetOrAddComponentInChildren<Toggle>("��ʾ���Toggle").onValueChanged.AddListener(delegate
        {
            gameRoot.giftImage = GiftImage.Heel;
        });
        //������ʾ�������尴ť
        _UITool.GetOrAddComponentInChildren<Toggle>("��ʾ����Toggle").onValueChanged.AddListener(delegate
        {
            gameRoot.giftImage = GiftImage.VErtical;
        });
        //���²���ʾ����ͼ��ť
        _UITool.GetOrAddComponentInChildren<Toggle>("����ʾToggle").onValueChanged.AddListener(delegate
        {
            gameRoot.giftImage = GiftImage.NUll;
        });
    }
}
