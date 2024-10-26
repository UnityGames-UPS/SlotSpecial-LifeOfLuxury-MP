using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GambleController : MonoBehaviour
{
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

    [Header("Gamble Controller")]
    [SerializeField]
    private GameObject m_AutoPlayRef;
    [SerializeField]
    private Toggle m_GambleToggle;

    private void Start()
    {
        if (m_InitButton) m_InitButton.onClick.RemoveAllListeners();
        if (m_InitButton) m_InitButton.onClick.AddListener(delegate { StartGamble(); });

        if (m_RedButton) m_RedButton.onClick.RemoveAllListeners();
        if (m_RedButton) m_RedButton.onClick.AddListener(delegate { OnRedButtonClicked(); });

        if (m_BlackButton) m_BlackButton.onClick.RemoveAllListeners();
        if (m_BlackButton) m_BlackButton.onClick.AddListener(delegate { OnBlackButtonClicked(); });

        if (m_TakeButton) m_TakeButton.onClick.RemoveAllListeners();
        if (m_TakeButton) m_TakeButton.onClick.AddListener(delegate { OnCollectButtonClicked(); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGamble();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            OnBlackButtonClicked();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnRedButtonClicked();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            OnCollectButtonClicked();
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
        SocketManager.StartGambleGame();
    }

    private void OnRedButtonClicked()
    {
        SocketManager.SelectGambleCard("RED");
    }

    private void OnBlackButtonClicked()
    {
        SocketManager.SelectGambleCard("BLACK");
    }

    private void OnCollectButtonClicked()
    {
        SocketManager.CollectGambledAmount();
        m_InitButton.gameObject.SetActive(false);
        m_AutoPlayRef.SetActive(true);
    }
}
