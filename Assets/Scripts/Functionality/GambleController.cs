using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GambleController : MonoBehaviour
{
  [Header("Slot Behaviour")]
  [SerializeField]
  private SlotBehaviour m_SlotBehaviour;

  [Header("Socket Manager")]
  [SerializeField]
  private SocketIOManager SocketManager;
  [SerializeField]
  private Sprite[] m_Card_Sprites;
  [SerializeField]
  private Sprite[] m_Mini_Card_Sprites;
  [SerializeField]
  private Button m_InitButton;
  [SerializeField]
  private Button m_RedButton;
  [SerializeField]
  private Button m_BlackButton;
  [SerializeField]
  private Button m_TakeButton;
  [SerializeField]
  private Button m_SlotStartButton;
  [SerializeField] private Button BetPlus;
  [SerializeField] private Button BetMinus;

  [Header("Card Show References")]
  [SerializeField]
  private Image m_ShowCard;
  [SerializeField]
  private Image m_ResultCard;

  [Header("Upper Array References")]
  [SerializeField]
  private List<Image> m_Upper_Sprites;
  [SerializeField]
  private List<int> m_Sprite_Indices;

  [Header("Game Panel References")]
  [SerializeField]
  private GameObject m_Main_Game_Panel;
  [SerializeField]
  private GameObject m_Gamble_Game_Panel;

  [Header("Gamble Controller")]
  [SerializeField]
  private GameObject m_AutoPlayRef;
  [SerializeField]
  private Toggle m_GambleToggle;

  [Header("Audio Manager")]
  [SerializeField]
  private AudioController audioController;

  private Coroutine m_GetGambleResult = null;
  private Coroutine m_ResetToDefault = null;

  private bool m_Is_Gambling = false;

  private void Start()
  {
    if (m_RedButton) m_RedButton.onClick.RemoveAllListeners();
    if (m_RedButton) m_RedButton.onClick.AddListener(delegate { OnRedButtonClicked(); audioController.PlayButtonAudio(); });

    if (m_BlackButton) m_BlackButton.onClick.RemoveAllListeners();
    if (m_BlackButton) m_BlackButton.onClick.AddListener(delegate { OnBlackButtonClicked(); audioController.PlayButtonAudio(); });

    if (m_TakeButton) m_TakeButton.onClick.RemoveAllListeners();
    if (m_TakeButton) m_TakeButton.onClick.AddListener(delegate
    {
      audioController.PlayButtonAudio();
      StartCoroutine(OnCollectButtonClicked());
    });

    // m_GambleToggle.onValueChanged.AddListener((arg) =>
    // {
    //   if (arg)
    //   {
    //     if(m_Is_Gambling || )
    //   }
    // });

    m_ShowCard.gameObject.SetActive(true);
    m_ResultCard.gameObject.SetActive(false);

    AssignUpperSprites();
  }

  internal void ResetToDefault()
  {
    if (m_Is_Gambling)
    {
      m_ResetToDefault = StartCoroutine(Reset());
    }
    else
    {
      m_Is_Gambling = false;
      m_InitButton.interactable = true;
      m_TakeButton.interactable = false;
      m_InitButton.gameObject.SetActive(false);
      m_AutoPlayRef.gameObject.SetActive(true);
      m_Main_Game_Panel.SetActive(true);
      m_Gamble_Game_Panel.SetActive(false);
      m_TakeButton.gameObject.SetActive(false);
      m_SlotStartButton.gameObject.SetActive(true);
      BetMinus.interactable = true;
      BetMinus.interactable = true;
    }
  }

  internal void TurnOnGambleButton(bool init)
  {
    if (m_GambleToggle.isOn)
    {
      m_InitButton.onClick.RemoveAllListeners();
      m_InitButton.onClick.AddListener(() =>
      {
        StartCoroutine(StartGamble(init));
        m_InitButton.interactable = false;
      });
      m_InitButton.interactable = true;
      m_InitButton.gameObject.SetActive(true);
    }
    else
    {
      m_InitButton.gameObject.SetActive(false);
      m_AutoPlayRef.SetActive(true);
    }
  }

  internal IEnumerator StartGamble(bool init)
  {
    m_TakeButton.interactable = false;
    m_TakeButton.gameObject.SetActive(true);
    BetPlus.interactable = false;
    BetMinus.interactable = false;
    m_SlotStartButton.gameObject.SetActive(false);
    if (m_SlotBehaviour.IsAutoSpin)
    {
      m_SlotBehaviour.WasAutoSpinOn = true;
      m_SlotBehaviour.StopAutoSpin();
    }
    m_Is_Gambling = true;
    m_Main_Game_Panel.SetActive(false);
    m_Gamble_Game_Panel.SetActive(true);

    if (init)
    {
      SocketManager.isGambledone = false;
      SocketManager.GambleInit();
      yield return new WaitUntil(() => SocketManager.isGambledone);
    }

    StartGambleUI();
  }

  private void OnRedButtonClicked()
  {
    m_RedButton.interactable = false;
    m_BlackButton.interactable = false;

    SocketManager.isGambledone = false;
    SocketManager.GambleDraw("RED");
    if (m_GetGambleResult != null)
    {
      StopCoroutine(m_GetGambleResult);
      m_GetGambleResult = null;
    }
    m_GetGambleResult = StartCoroutine(GetGambleResult());
  }

  private void OnBlackButtonClicked()
  {
    m_RedButton.interactable = false;
    m_BlackButton.interactable = false;

    SocketManager.isGambledone = false;
    SocketManager.GambleDraw("BLACK");
    if (m_GetGambleResult != null)
    {
      StopCoroutine(m_GetGambleResult);
      m_GetGambleResult = null;
    }
    m_GetGambleResult = StartCoroutine(GetGambleResult());
  }

  private IEnumerator OnCollectButtonClicked()
  {
    m_TakeButton.interactable = false;
    SocketManager.isGambledone = false;
    SocketManager.GambleCollect();
    yield return new WaitUntil(() => SocketManager.isGambledone);
    m_Is_Gambling = false;
    ResetToDefault();
  }

  private void StartGambleUI()
  {
    m_ShowCard.gameObject.SetActive(true);
    m_ResultCard.gameObject.SetActive(false);

    m_RedButton.interactable = true;
    m_BlackButton.interactable = true;
  }

  private IEnumerator GetGambleResult()
  {
    yield return new WaitUntil(() => SocketManager.isGambledone);
    //TODO: Rotate Animation Here
    DOTweenUIManager.Instance.RotateUI(m_ShowCard.rectTransform, "Y", 2f, 360f * 2.5f);
    yield return new WaitForSeconds(2f); // Round Animation Show Time
    ShowGambleUI(SocketManager.gambleData.payload.playerWon, SocketManager.gambleData.payload.winAmount, SocketManager.gambleData.player.balance, SocketManager.gambleData.payload.cardId);

    if (SocketManager.gambleData.payload.playerWon)
    {
      m_TakeButton.interactable = true;
      TurnOnGambleButton(false);
    }
    else
    {
      yield return new WaitForSeconds(2f);
      m_Is_Gambling = false;
      ResetToDefault();
    }
  }

  private void ShowGambleUI(bool won, double win, double balance, int id)
  {
    m_ShowCard.gameObject.SetActive(false);
    m_ResultCard.gameObject.SetActive(true);

    m_ResultCard.sprite = m_Card_Sprites[id];
    PushIndex(id);
    m_SlotBehaviour.UpdateBottomUI(won, win, balance);
  }
  private void PushIndex(int index)
  {
    int temp = index;
    int temp_2 = 0;
    for (int i = 0; i < m_Sprite_Indices.Count; i++)
    {
      temp_2 = m_Sprite_Indices[i];
      m_Sprite_Indices[i] = temp;
      temp = temp_2;
    }
    AssignUpperSprites();
  }

  private void AssignUpperSprites()
  {
    for (int i = 0; i < m_Upper_Sprites.Count; i++)
    {
      m_Upper_Sprites[i].sprite = m_Mini_Card_Sprites[m_Sprite_Indices[i]];
    }
  }

  private IEnumerator Reset()
  {
    m_TakeButton.interactable = false;
    m_RedButton.interactable = false;
    m_BlackButton.interactable = false;
    yield return new WaitForSeconds(1.5f);
    m_Is_Gambling = false;
    m_InitButton.interactable = true;
    m_TakeButton.interactable = true;
    m_BlackButton.interactable = true;
    m_RedButton.interactable = true;
    m_InitButton.gameObject.SetActive(false);
    m_AutoPlayRef.gameObject.SetActive(true);
    m_Main_Game_Panel.SetActive(true);
    m_Gamble_Game_Panel.SetActive(false);
    m_TakeButton.gameObject.SetActive(false);
    m_SlotStartButton.gameObject.SetActive(true);
    if (m_SlotBehaviour.WasAutoSpinOn)
    {
      m_SlotBehaviour.AutoSpin();
      m_SlotBehaviour.WasAutoSpinOn = false;
    }
    StopCoroutine(m_ResetToDefault);
    m_ResetToDefault = null;
  }
}
