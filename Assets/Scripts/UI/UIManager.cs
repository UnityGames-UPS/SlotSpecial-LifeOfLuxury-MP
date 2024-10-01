using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;
using UnityEngine.Networking;

public class UIManager : MonoBehaviour
{

    [Header("Popus UI")]
    [SerializeField]
    private GameObject MainPopup_Object;

    [Header("Paytable Popup")]
    [SerializeField]
    private GameObject m_Paytable_Object;
    [SerializeField]
    private TMP_Text[] SymbolsText;
    [SerializeField]
    private GameObject[] InfoSlides;
    [SerializeField]
    private Button Right_Button;
    [SerializeField]
    private Button Left_Button;
    [SerializeField]
    private Button Paytable_Button;
    [SerializeField]
    private Button ClosePayTable_Button;

    [Header("FreeSpins Popup")]
    [SerializeField]
    private GameObject FreeSpinPopup_Object;
    [SerializeField]
    private TMP_Text Free_Text;
    [SerializeField]
    private Button FreeSpin_Button;

    [Header("Disconnection Popup")]
    [SerializeField]
    private Button CloseDisconnect_Button;
    [SerializeField]
    private GameObject DisconnectPopup_Object;

    [Header("AnotherDevice Popup")]
    [SerializeField]
    private Button CloseAD_Button;
    [SerializeField]
    private GameObject ADPopup_Object;

    [Header("Reconnection Popup")]
    [SerializeField]
    private TMP_Text reconnect_Text;
    [SerializeField]
    private GameObject ReconnectPopup_Object;

    [Header("LowBalance Popup")]
    [SerializeField]
    private Button LBExit_Button;
    [SerializeField]
    private GameObject LBPopup_Object;

    [Header("Quit Popup")]
    [SerializeField]
    private GameObject QuitPopup_Object;
    [SerializeField]
    private Button YesQuit_Button;
    [SerializeField]
    private Button NoQuit_Button;

    [Header("Settings Popup")]
    [SerializeField]
    private Button Settings_Button;
    [SerializeField]
    private Button SettingsClose_Button;
    [SerializeField]
    private Button SettingsCloseFull_Button;
    [SerializeField]
    private RectTransform SettingPanel_RT;
    [SerializeField]
    private GameObject SettingMainPanel;
    [SerializeField]
    private RectTransform[] Misc_RT;
    [SerializeField]
    private Toggle Sound_Toggle;
    [SerializeField]
    private Toggle Music_Toggle;

    [SerializeField]
    private AudioController audioController;

    [SerializeField]
    private Button GameExit_Button;

    [SerializeField]
    private SlotBehaviour slotManager;

    private bool isMusic = true;
    private bool isSound = true;
    private bool isExit = false;
    private int FreeSpins;

    private int slideCounter = 0;


    private void Start()
    {
        if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
        if (GameExit_Button) GameExit_Button.onClick.AddListener(delegate { OpenPopup(QuitPopup_Object); });

        if (audioController) audioController.ToggleMute(false);

        if (Right_Button) Right_Button.onClick.RemoveAllListeners();
        if (Right_Button) Right_Button.onClick.AddListener(delegate { ToggleSlides(true); });

        if (Left_Button) Left_Button.onClick.RemoveAllListeners();
        if (Left_Button) Left_Button.onClick.AddListener(delegate { ToggleSlides(true); });

        if (Paytable_Button) Paytable_Button.onClick.RemoveAllListeners();
        if (Paytable_Button) Paytable_Button.onClick.AddListener(delegate { OpenClosePaytable(true); });

        if (ClosePayTable_Button) ClosePayTable_Button.onClick.RemoveAllListeners();
        if (ClosePayTable_Button) ClosePayTable_Button.onClick.AddListener(delegate { OpenClosePaytable(false); });

        if (Settings_Button) Settings_Button.onClick.RemoveAllListeners();
        if (Settings_Button) Settings_Button.onClick.AddListener(OpenSettings);

        if (SettingsCloseFull_Button) SettingsCloseFull_Button.onClick.RemoveAllListeners();
        if (SettingsCloseFull_Button) SettingsCloseFull_Button.onClick.AddListener(CloseSettings);

        if (SettingsClose_Button) SettingsClose_Button.onClick.RemoveAllListeners();
        if (SettingsClose_Button) SettingsClose_Button.onClick.AddListener(CloseSettings);

        if (Music_Toggle) Music_Toggle.onValueChanged.RemoveAllListeners();
        if (Music_Toggle) Music_Toggle.onValueChanged.AddListener(ToggleMusic);

        if (Sound_Toggle) Sound_Toggle.onValueChanged.RemoveAllListeners();
        if (Sound_Toggle) Sound_Toggle.onValueChanged.AddListener(ToggleSound);

        if (NoQuit_Button) NoQuit_Button.onClick.RemoveAllListeners();
        if (NoQuit_Button) NoQuit_Button.onClick.AddListener(delegate { if (!isExit) { ClosePopup(QuitPopup_Object); } });

        if (YesQuit_Button) YesQuit_Button.onClick.RemoveAllListeners();
        if (YesQuit_Button) YesQuit_Button.onClick.AddListener(CallOnExitFunction);

        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(CallOnExitFunction);

        if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
        if (CloseAD_Button) CloseAD_Button.onClick.AddListener(CallOnExitFunction);

        if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
        if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); });

        isMusic = true;
        isSound = true;
    }

    private void StartFreeSpins(int spins)
    {
        if (MainPopup_Object) MainPopup_Object.SetActive(false);
        slotManager.FreeSpin(spins);
    }

    internal void FreeSpinProcess(int spins)
    {
        FreeSpins = spins;
        StartFreeSpins(spins);
    }

    internal void DisconnectionPopup(bool isReconnection)
    {
        //if(isReconnection)
        //{
        //    OpenPopup(ReconnectPopup_Object);
        //}
        //else
        //{
        //    ClosePopup(ReconnectPopup_Object);
        //}

        if (!isExit)
        {
            if(!ADPopup_Object.activeSelf)OpenPopup(DisconnectPopup_Object);
        }
    }

    internal void ADfunction()
    {
        OpenPopup(ADPopup_Object);
    }

    internal void InitialiseUIData(string SupportUrl, string AbtImgUrl, string TermsUrl, string PrivacyUrl, Paylines symbolsText)
    {
        //if (Support_Button) Support_Button.onClick.RemoveAllListeners();
        //if (Support_Button) Support_Button.onClick.AddListener(delegate { UrlButtons(SupportUrl); });

        //if (Terms_Button) Terms_Button.onClick.RemoveAllListeners();
        //if (Terms_Button) Terms_Button.onClick.AddListener(delegate { UrlButtons(TermsUrl); });

        //if (Privacy_Button) Privacy_Button.onClick.RemoveAllListeners();
        //if (Privacy_Button) Privacy_Button.onClick.AddListener(delegate { UrlButtons(PrivacyUrl); });

        //StartCoroutine(DownloadImage(AbtImgUrl));
        PopulateSymbolsPayout(symbolsText);
    }

    private void PopulateSymbolsPayout(Paylines paylines)
    {
        //for (int i = 0; i < paylines.symbols.Count; i++)
        //{
        //    string text = null;
        //    if (paylines.symbols[i].multiplier._5x != 0)
        //    {
        //        text += "<color=white>5</color>  " + paylines.symbols[i].multiplier._5x.ToString("f2");
        //    }
        //    if (paylines.symbols[i].multiplier._4x != 0)
        //    {
        //        text += "\n<color=white>4</color>  " + paylines.symbols[i].multiplier._4x.ToString("f2");
        //    }
        //    if (paylines.symbols[i].multiplier._3x != 0)
        //    {
        //        text += "\n<color=white>3</color>  " + paylines.symbols[i].multiplier._3x.ToString("f2");
        //    }
        //    if (paylines.symbols[i].multiplier._2x != 0)
        //    {
        //        text += "\n<color=white>2</color>  " + paylines.symbols[i].multiplier._2x.ToString("f2");
        //    }
        //    if (SymbolsText[i]) SymbolsText[i].text = text;
        //}
    }

    private void CallOnExitFunction()
    {
        isExit = true;
        slotManager.CallCloseSocket();
        Application.ExternalCall("window.parent.postMessage", "onExit", "*");
    }

    private void OpenSettings()
    {
        if (SettingsCloseFull_Button) SettingsCloseFull_Button.gameObject.SetActive(true);
        if (SettingMainPanel) SettingMainPanel.SetActive(true);

        if (SettingPanel_RT) SettingPanel_RT.DOAnchorPosX(SettingPanel_RT.anchoredPosition.x + 700, 0.3f).OnComplete(() =>
        {
            if (SettingsClose_Button) SettingsClose_Button.gameObject.SetActive(true);
            if (Settings_Button) Settings_Button.gameObject.SetActive(false);
        }); ;

        for (int i = 0; i < Misc_RT.Length; i++)
        {
            Misc_RT[i].DOScale(Vector3.zero, 0.3f);
        }
    }

    private void CloseSettings()
    {
        if (SettingsCloseFull_Button) SettingsCloseFull_Button.gameObject.SetActive(false);

        for (int i = 0; i < Misc_RT.Length; i++)
        {
            Misc_RT[i].DOScale(Vector3.one, 0.3f);
        }

        if (SettingPanel_RT) SettingPanel_RT.DOAnchorPosX(SettingPanel_RT.anchoredPosition.x - 700, 0.3f).OnComplete(() =>
        {
            SettingMainPanel.SetActive(false);
            Settings_Button.gameObject.SetActive(true);
            SettingsClose_Button.gameObject.SetActive(false);
        });

    }

    private void OpenClosePaytable(bool m_config)
    {
        if (m_config)
        {
            m_Paytable_Object.SetActive(true);
            Paytable_Button.gameObject.SetActive(false);
            ClosePayTable_Button.gameObject.SetActive(true);
        }
        else
        {
            m_Paytable_Object.SetActive(false);
            Paytable_Button.gameObject.SetActive(true);
            ClosePayTable_Button.gameObject.SetActive(false);
        }
    }

    private void OpenPopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    internal void ClosePopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(false);
        if (!DisconnectPopup_Object.activeSelf)
        {
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
        }
    }

    private void ToggleSlides(bool isRight)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (isRight)
        {
            if (InfoSlides[slideCounter]) InfoSlides[slideCounter].SetActive(false);
            slideCounter++;
            if (slideCounter >= InfoSlides.Length)
            {
                slideCounter = 0;
            }
            if (InfoSlides[slideCounter]) InfoSlides[slideCounter].SetActive(true);
        }
        else
        {
            if (InfoSlides[slideCounter]) InfoSlides[slideCounter].SetActive(false);
            slideCounter--;
            if (slideCounter < 0)
            {
                slideCounter = InfoSlides.Length - 1;
            }
            if (InfoSlides[slideCounter]) InfoSlides[slideCounter].SetActive(true);
        }
    }

    private void ToggleMusic(bool isOn)
    {
        if (isOn)
        {
            audioController.ToggleMute(false, "bg");
        }
        else
        {
            audioController.ToggleMute(true, "bg");
        }
    }

    private void UrlButtons(string url)
    {
        Application.OpenURL(url);
    }

    private void ToggleSound(bool isOn)
    {
        if (isOn)
        {
            if (audioController) audioController.ToggleMute(false, "button");
            if (audioController) audioController.ToggleMute(false, "wl");
        }
        else
        {
            if (audioController) audioController.ToggleMute(true, "button");
            if (audioController) audioController.ToggleMute(true, "wl");
        }
    }

}
