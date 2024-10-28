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

    private Coroutine m_GetGambleResult = null;
    private Coroutine m_ResetToDefault = null;

    private bool m_Is_Gambling = false;

    private void Start()
    {
        if (m_InitButton) m_InitButton.onClick.RemoveAllListeners();
        if (m_InitButton) m_InitButton.onClick.AddListener(delegate { StartGamble(); m_InitButton.interactable = false; });

        if (m_RedButton) m_RedButton.onClick.RemoveAllListeners();
        if (m_RedButton) m_RedButton.onClick.AddListener(delegate { OnRedButtonClicked(); });

        if (m_BlackButton) m_BlackButton.onClick.RemoveAllListeners();
        if (m_BlackButton) m_BlackButton.onClick.AddListener(delegate { OnBlackButtonClicked(); });

        if (m_TakeButton) m_TakeButton.onClick.RemoveAllListeners();
        if (m_TakeButton) m_TakeButton.onClick.AddListener(delegate { OnCollectButtonClicked(); });

        m_ShowCard.gameObject.SetActive(true);
        m_ResultCard.gameObject.SetActive(false);

        AssignUpperSprites();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PushIndex(2);
        }
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
            m_TakeButton.interactable = true;
            m_InitButton.gameObject.SetActive(false);
            m_AutoPlayRef.gameObject.SetActive(true);
            m_Main_Game_Panel.SetActive(true);
            m_Gamble_Game_Panel.SetActive(false);
            m_TakeButton.gameObject.SetActive(false);
            m_SlotStartButton.gameObject.SetActive(true);
        }
    }

    internal void CheckGamble()
    {
        if (m_GambleToggle.isOn)
        {
            m_InitButton.gameObject.SetActive(true);
            m_AutoPlayRef.SetActive(false);
        }
        else
        {
            m_InitButton.gameObject.SetActive(false);
            m_AutoPlayRef.SetActive(true);
        }
    }

    internal void StartGamble()
    {
        m_Is_Gambling = true;
        SocketManager.StartGambleGame();
        StartGambleUI();
        m_Main_Game_Panel.SetActive(false);
        m_Gamble_Game_Panel.SetActive(true);
        m_TakeButton.gameObject.SetActive(true);
        m_SlotStartButton.gameObject.SetActive(false);
    }

    private void OnRedButtonClicked()
    {
        m_RedButton.interactable = false;
        m_BlackButton.interactable = false;

        SocketManager.SelectGambleCard("RED");
        if(m_GetGambleResult != null)
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

        SocketManager.SelectGambleCard("BLACK");
        if (m_GetGambleResult != null)
        {
            StopCoroutine(m_GetGambleResult);
            m_GetGambleResult = null;
        }
        m_GetGambleResult = StartCoroutine(GetGambleResult());
    }

    private void OnCollectButtonClicked()
    {
        SocketManager.CollectGambledAmount();
        if(m_GetGambleResult != null)
        {
            StopCoroutine(m_GetGambleResult);
            m_GetGambleResult = null;
        }
    }

    private void StartGambleUI()
    {
        m_ShowCard.gameObject.SetActive(true);
        m_ResultCard.gameObject.SetActive(false);

        m_RedButton.interactable = true;
        m_BlackButton.interactable = true;
    }

    private void ShowGambleUI(bool won, double win, double balance, int id)
    {
        m_ShowCard.gameObject.SetActive(false);
        m_ResultCard.gameObject.SetActive(true);

        m_ResultCard.sprite = m_Card_Sprites[id];
        m_SlotBehaviour.UpdateBottomUI(won, win, balance);
    }

    private void AssignUpperSprites()
    {
        for(int i = 0; i < m_Upper_Sprites.Count; i ++)
        {
            m_Upper_Sprites[i].sprite = m_Mini_Card_Sprites[m_Sprite_Indices[i]];
        }
    }

    private void PushIndex(int index)
    {
        int temp = index;
        int temp_2 = 0;
        for(int i = 0; i < m_Sprite_Indices.Count; i++)
        {
            temp_2 = m_Sprite_Indices[i];
            m_Sprite_Indices[i] = temp;
            temp = temp_2;
        }
        AssignUpperSprites();
    }

    private IEnumerator Reset()
    {
        m_TakeButton.interactable = false;
        m_RedButton.interactable = false;
        m_BlackButton.interactable = false;
        yield return new WaitForSeconds(3f);
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

        StopCoroutine(m_ResetToDefault);
        m_ResetToDefault = null;
    }

    private IEnumerator GetGambleResult()
    {
        yield return new WaitUntil(() => SocketManager.isGambledone);
        //TODO: Rotate Animation Here
        DOTweenUIManager.Instance.RotateUI(m_ShowCard.rectTransform, "Y", 2f, 360f * 2.5f);
        yield return new WaitForSeconds(2f); // Round Animation Show Time
        ShowGambleUI(SocketManager.gambleData.playerWon, SocketManager.gambleData.currentWinning, SocketManager.gambleData.balance, SocketManager.gambleData.cardId);
        SocketManager.isGambledone = false;
        yield return new WaitForSeconds(4f); // Wait Time For Card Show
        StartGambleUI();
    }
}
