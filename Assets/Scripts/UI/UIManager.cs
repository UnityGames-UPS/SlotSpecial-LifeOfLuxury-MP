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

  [Header("DoTween Manager")]
  [SerializeField]
  private DOTweenUIManager m_DoTweenUIManager;

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
  private List<int> m_TempValues = new() { 0, 0, 0, 0, 0 };

  [Header("Title View References")]
  [SerializeField]
  private Image m_Title_Image;
  [SerializeField]
  private Transform m_FreeSpinHolder;
  [SerializeField]
  private Image[] m_FreeSpinIcons;

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

  [Header("Big Win Popup")]
  [SerializeField]
  private GameObject WinPopup_Object;

  [Header("5 Of A Kind")]
  [SerializeField]
  private GameObject Five_Of_A_Kind_Object;

  [Header("Settings Popup")]
  [SerializeField]
  internal Button Settings_Button;
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
  private Button SkipWinAnimation;
  [SerializeField]
  private Button SkipWinAnimation2;
  private Tween WinPopupTextTween;
  private Tween ClosePopupTween;

  [SerializeField]
  private AudioController audioController;

  [SerializeField]
  private Button GameExit_Button;

  [SerializeField]
  private SlotBehaviour slotManager;

  [SerializeField]
  private SocketIOManager socketManager;

  private bool isMusic = true;
  private bool isSound = true;
  private bool isExit = false;
  internal int FreeSpins;

  private int slideCounter = 0;


  private void Start()
  {
    if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
    if (GameExit_Button) GameExit_Button.onClick.AddListener(delegate { OpenPopup(QuitPopup_Object); audioController.PlayButtonAudio(); });

    if (audioController) audioController.ToggleMute(false);

    if (Right_Button) Right_Button.onClick.RemoveAllListeners();
    if (Right_Button) Right_Button.onClick.AddListener(delegate { ToggleSlides(true); audioController.PlayButtonAudio(); });

    if (Left_Button) Left_Button.onClick.RemoveAllListeners();
    if (Left_Button) Left_Button.onClick.AddListener(delegate { ToggleSlides(false); audioController.PlayButtonAudio(); });

    if (Paytable_Button) Paytable_Button.onClick.RemoveAllListeners();
    if (Paytable_Button) Paytable_Button.onClick.AddListener(delegate { OpenClosePaytable(true); audioController.PlayButtonAudio(); });

    if (ClosePayTable_Button) ClosePayTable_Button.onClick.RemoveAllListeners();
    if (ClosePayTable_Button) ClosePayTable_Button.onClick.AddListener(delegate
    {
      OpenClosePaytable(false);
      if (InfoSlides[slideCounter]) InfoSlides[slideCounter].SetActive(false);
      slideCounter = 0;
      if (InfoSlides[slideCounter]) InfoSlides[slideCounter].SetActive(true);
      audioController.PlayButtonAudio();
    });

    if (Settings_Button) Settings_Button.onClick.RemoveAllListeners();
    if (Settings_Button) Settings_Button.onClick.AddListener(delegate { OpenSettings(); audioController.PlayButtonAudio(); });

    //if (SettingsCloseFull_Button) SettingsCloseFull_Button.onClick.RemoveAllListeners();
    //if (SettingsCloseFull_Button) SettingsCloseFull_Button.onClick.AddListener(delegate { CloseSettings(); audioController.PlayButtonAudio(); });

    if (SettingsClose_Button) SettingsClose_Button.onClick.RemoveAllListeners();
    if (SettingsClose_Button) SettingsClose_Button.onClick.AddListener(delegate { CloseSettings(); audioController.PlayButtonAudio(); });

    if (Music_Toggle) Music_Toggle.onValueChanged.RemoveAllListeners();
    if (Music_Toggle) Music_Toggle.onValueChanged.AddListener(delegate { ToggleMusic(Music_Toggle.isOn); audioController.PlayButtonAudio(); });

    if (Sound_Toggle) Sound_Toggle.onValueChanged.RemoveAllListeners();
    if (Sound_Toggle) Sound_Toggle.onValueChanged.AddListener(delegate { ToggleSound(Sound_Toggle.isOn); audioController.PlayButtonAudio(); });

    if (NoQuit_Button) NoQuit_Button.onClick.RemoveAllListeners();
    if (NoQuit_Button) NoQuit_Button.onClick.AddListener(delegate { if (!isExit) { ClosePopup(QuitPopup_Object); } audioController.PlayButtonAudio(); });

    if (YesQuit_Button) YesQuit_Button.onClick.RemoveAllListeners();
    if (YesQuit_Button) YesQuit_Button.onClick.AddListener(delegate { CallOnExitFunction(); audioController.PlayButtonAudio(); });

    if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
    if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(delegate { CallOnExitFunction(); }); //BackendChanges

    if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
    if (CloseAD_Button) CloseAD_Button.onClick.AddListener(delegate { CallOnExitFunction(); audioController.PlayButtonAudio(); });

    if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
    if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); audioController.PlayButtonAudio(); });

    isMusic = true;
    isSound = true;

    if (SkipWinAnimation) SkipWinAnimation.onClick.RemoveAllListeners();
    if (SkipWinAnimation) SkipWinAnimation.onClick.AddListener(SkipWin);

    if (SkipWinAnimation2) SkipWinAnimation2.onClick.RemoveAllListeners();
    if (SkipWinAnimation2) SkipWinAnimation2.onClick.AddListener(SkipWin);
  }

  void SkipWin()
  {
    Debug.Log("Skip win called");
    if (ClosePopupTween != null)
    {
      ClosePopupTween.Kill();
      ClosePopupTween = null;
    }
    if (WinPopupTextTween != null)
    {
      WinPopupTextTween.Kill();
      WinPopupTextTween = null;
    }
    ClosePopup(WinPopup_Object);
    slotManager.CheckPopups = false;
  }

  private void StartFreeSpins(int spins)
  {
    if (MainPopup_Object) MainPopup_Object.SetActive(false);
    slotManager.FreeSpin(spins);
  }

  internal void FreeSpinProcess(int spins)
  {
    int ExtraSpins = spins - FreeSpins;
    FreeSpins = spins;
    DOVirtual.DelayedCall(1f, () =>
    {
      StartFreeSpins(spins);
    });
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
      if (!ADPopup_Object.activeSelf) OpenPopup(DisconnectPopup_Object);
    }
  }

  internal void SwitchFreeSpinMode(bool m_config)
  {
    if (m_config)
    {
      m_Title_Image.gameObject.SetActive(false);
      m_FreeSpinHolder.gameObject.SetActive(true);
    }
    else
    {
      m_Title_Image.gameObject.SetActive(true);
      m_FreeSpinHolder.gameObject.SetActive(false);
    }
  }

  internal void PopulateWin(int value, double amount)
  {
    switch (value)
    {
      case 1:
        StartPopupAnim(amount, WinPopup_Object);
        break;
      case 2:
        StartPopupAnim(amount, Five_Of_A_Kind_Object);
        break;
    }
  }

  private void StartPopupAnim(double amount, GameObject m_object)
  {
    if (m_object) m_object.SetActive(true);
    m_object.GetComponent<ImageAnimation>().StartAnimation();
    if (MainPopup_Object) MainPopup_Object.SetActive(true);

    //DOTween.To(() => initAmount, (val) => initAmount = val, (int)amount, 5f).OnUpdate(() =>
    //{
    //    if (Win_Text) Win_Text.text = initAmount.ToString();
    //});

    DOVirtual.DelayedCall(3f, () =>
    {
      m_object.GetComponent<ImageAnimation>().StopAnimation();
      ClosePopup(m_object);
      slotManager.CheckPopups = false;
    });
  }

  internal void UpdateFreeSpinUI(List<int> m_values)
  {
    for (int i = 0; i < m_values.Count; i++)
    {
      if (m_values[i] != m_TempValues[i])
      {
        m_FreeSpinIcons[i].transform.GetChild(0).GetComponent<TMP_Text>().text = "X" + m_values[i].ToString();
        DOTweenUIManager.Instance.Jump(m_FreeSpinIcons[i].rectTransform, 25f, 1, 1f);
        DOTweenUIManager.Instance.PopIn(m_FreeSpinIcons[i].rectTransform, 1f);
        DOTweenUIManager.Instance.BounceIn(m_FreeSpinIcons[i].transform.GetChild(0).transform.GetComponent<RectTransform>(), 1f);
      }
      else
      {
        m_FreeSpinIcons[i].transform.GetChild(0).GetComponent<TMP_Text>().text = "X" + m_values[i].ToString();
      }
    }
    m_TempValues.Clear();
    m_TempValues.TrimExcess();
    m_TempValues = new List<int>(m_values);
  }

  internal void ADfunction()
  {
    OpenPopup(ADPopup_Object);
  }

  internal void LowBalPopup()
  {
    OpenPopup(LBPopup_Object);
  }

  internal void InitialiseUIData(Paylines symbolsText)
  {
    PopulateSymbolsPayout(symbolsText);
  }

  private void PopulateSymbolsPayout(Paylines paylines)
  {
    for (int i = 0; i < paylines.symbols.Count - 2; i++)
    {
      string text = null;
      if (paylines.symbols[i].multiplier[0] != 0)
      {
        text += "<color=white>5</color>  " + paylines.symbols[i].multiplier[0].ToString("f2") + "x";
      }
      if (paylines.symbols[i].multiplier[1] != 0)
      {
        text += "\n<color=white>4</color>  " + paylines.symbols[i].multiplier[1].ToString("f2") + "x";
      }
      if (paylines.symbols[i].multiplier[2] != 0)
      {
        text += "\n<color=white>3</color>  " + paylines.symbols[i].multiplier[2].ToString("f2") + "x";
      }

      if (SymbolsText[i]) SymbolsText[i].text = text;
    }
  }

  private void CallOnExitFunction()
  {
    isExit = true;
    slotManager.CallCloseSocket();
  }

  private void OpenSettings()
  {
    //if (SettingsCloseFull_Button) SettingsCloseFull_Button.gameObject.SetActive(true);
    if (SettingMainPanel) SettingMainPanel.SetActive(true);
    Settings_Button.interactable = false;
    SettingsClose_Button.interactable = false;
    if (SettingPanel_RT) SettingPanel_RT.DOAnchorPosX(SettingPanel_RT.anchoredPosition.x + 700, 0.3f).OnComplete(() =>
    {
      if (SettingsClose_Button) SettingsClose_Button.gameObject.SetActive(true);
      if (Settings_Button) Settings_Button.gameObject.SetActive(false);
      Settings_Button.interactable = true;
      SettingsClose_Button.interactable = true;
    }); ;

    for (int i = 0; i < Misc_RT.Length; i++)
    {
      Misc_RT[i].DOScale(Vector3.zero, 0.3f);
    }
  }

  private void CloseSettings()
  {
    //if (SettingsCloseFull_Button) SettingsCloseFull_Button.gameObject.SetActive(false);
    Settings_Button.interactable = false;
    SettingsClose_Button.interactable = false;
    for (int i = 0; i < Misc_RT.Length; i++)
    {
      Misc_RT[i].DOScale(Vector3.one, 0.3f);
    }

    if (SettingPanel_RT) SettingPanel_RT.DOAnchorPosX(SettingPanel_RT.anchoredPosition.x - 700, 0.3f).OnComplete(() =>
    {
      SettingMainPanel.SetActive(false);
      Settings_Button.gameObject.SetActive(true);
      SettingsClose_Button.gameObject.SetActive(false);
      Settings_Button.interactable = true;
      SettingsClose_Button.interactable = true;
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
