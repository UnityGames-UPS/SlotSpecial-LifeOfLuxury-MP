using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotBehaviour : MonoBehaviour
{
    [SerializeField]
    private RectTransform mainContainer_RT;

    [Header("Sprites")]
    [SerializeField]
    private Sprite[] myImages;

    [Header("Slot Images")]
    [SerializeField]
    private List<SlotImage> images;
    [SerializeField]
    private List<SlotImage> Tempimages;

    [Header("Slots Objects")]
    [SerializeField]
    private GameObject[] Slot_Objects;
    [Header("Slots Elements")]
    [SerializeField]
    private LayoutElement[] Slot_Elements;

    [Header("Slots Transforms")]
    [SerializeField]
    private Transform[] Slot_Transform;
    private Dictionary<int, string> y_string = new Dictionary<int, string>();

    [Header("Buttons")]
    [SerializeField]
    private Button SlotStart_Button;
    [SerializeField]
    private Button Take_Button;
    [SerializeField]
    private Button AutoSpinStart_Button;
    [SerializeField]
    private Button AutoSpinStop_Button;
    [SerializeField]
    private Button MaxBet_Button;
    [SerializeField]
    private Button BetPlus_Button;
    [SerializeField]
    private Button BetMinus_Button;
    [SerializeField]
    private Button LinePlus_Button;
    [SerializeField]
    private Button LineMinus_Button;

    [Header("Animated Sprites")]
    [SerializeField]
    private Sprite[] Diamond_Sprite;
    [SerializeField]
    private Sprite[] Watch_Sprite;
    [SerializeField]
    private Sprite[] Car_Sprite;
    [SerializeField]
    private Sprite[] Ship_Sprite;
    [SerializeField]
    private Sprite[] Plane_Sprite;
    [SerializeField]
    private Sprite[] Bottle_Sprite;
    [SerializeField]
    private Sprite[] FiveStar_Sprite;

    [Header("Miscellaneous UI")]
    [SerializeField]
    private TMP_Text Balance_text;
    [SerializeField]
    private TMP_Text TotalBet_text;
    [SerializeField]
    private TMP_Text MainBet_text;
    [SerializeField]
    private TMP_Text Lines_text;
    [SerializeField]
    private TMP_Text TotalWin_text;


    [Header("Audio Management")]
    [SerializeField]
    private AudioController audioController;

    [Header("Free Spins")]
    [SerializeField]
    private Slider FreeSpin_Slider;
    [SerializeField]
    private GameObject Slider_Object;
    [SerializeField]
    private GameObject NormalImage_Object;
    [SerializeField]
    private TMP_Text SpinLeft_Text;
    [SerializeField]
    private TMP_Text SpinUtilised_Text;
    [SerializeField]
    private TMP_Text TotalSpins_Text;

    [SerializeField]
    private TMP_Text MainDisplayText;

    [Header("Bar Text References")]
    [SerializeField]
    private TMP_Text m_L_Bar;
    [SerializeField]
    private TMP_Text m_R_Bar;

    [Header("UI Manager References")]
    [SerializeField]
    private UIManager uiManager;

    int tweenHeight = 0;  //calculate the height at which tweening is done

    [SerializeField]
    private GameObject Image_Prefab;    //icons prefab
    private List<Tweener> alltweens = new List<Tweener>();
    [SerializeField]
    private List<ImageAnimation> TempList;  //stores the sprites whose animation is running at present 
    [SerializeField]
    private List<Image> TempListImg;  //stores the sprites whose animation is running at present 
    [SerializeField]
    private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing
    [SerializeField]
    private int numberOfSlots = 5;          //number of columns
    [SerializeField]
    int verticalVisibility = 3;
    [SerializeField]
    private SocketIOManager SocketManager;

    [Header("Gameble Controller")]
    [SerializeField]
    private GambleController m_GambleController;

    private Coroutine AutoSpinRoutine = null;
    private Coroutine FreeSpinRoutine = null;
    private Coroutine tweenroutine;
    private Coroutine Textroutine;
    private Coroutine AnimationToggleRoutine = null;
    private Coroutine SlotAnimRoutine = null;

    private bool IsAutoSpin = false;
    private bool IsFreeSpin = false;
    private bool IsSpinning = false;
    internal bool CheckPopups = false;
    private bool CheckSpinAudio = false;
    bool m_Is_5_Of_A_Kind = false;
    internal bool m_Is_Playing_Animation_In_Loop = false;

    private int BetCounter = 0;
    private int LineCounter = 0;
    protected int Lines = 15;

    private double currentBalance = 0;
    private double currentTotalBet = 0;

    //TEMP VARIABLES
    List<ImageAnimation> m_TempList = new List<ImageAnimation>();
    ImageAnimation anim;

    private void Start()
    {
        IsAutoSpin = false;
        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(delegate { StartSlots(); audioController.PlayButtonAudio(); });

        if (Take_Button) Take_Button.onClick.RemoveAllListeners();
        if (Take_Button) Take_Button.onClick.AddListener(() => { m_GambleController.ResetToDefault(); audioController.PlayButtonAudio(); });

        if (BetPlus_Button) BetPlus_Button.onClick.RemoveAllListeners();
        if (BetPlus_Button) BetPlus_Button.onClick.AddListener(delegate { ChangeBet(true); audioController.PlayButtonAudio(); });
        if (BetMinus_Button) BetMinus_Button.onClick.RemoveAllListeners();
        if (BetMinus_Button) BetMinus_Button.onClick.AddListener(delegate { ChangeBet(false); audioController.PlayButtonAudio(); });

        if (LinePlus_Button) LinePlus_Button.onClick.RemoveAllListeners();
        if (LinePlus_Button) LinePlus_Button.onClick.AddListener(delegate { ChangeLine(true); audioController.PlayButtonAudio(); });
        if (LineMinus_Button) LineMinus_Button.onClick.RemoveAllListeners();
        if (LineMinus_Button) LineMinus_Button.onClick.AddListener(delegate { ChangeLine(false); audioController.PlayButtonAudio(); });

        if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        if (MaxBet_Button) MaxBet_Button.onClick.AddListener(delegate { MaxBet(); audioController.PlayButtonAudio(); });

        if (AutoSpinStart_Button) AutoSpinStart_Button.onClick.RemoveAllListeners();
        if (AutoSpinStart_Button) AutoSpinStart_Button.onClick.AddListener(delegate { AutoSpin(); audioController.PlayButtonAudio(); });


        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.RemoveAllListeners();
        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.AddListener(delegate { StopAutoSpin(); audioController.PlayButtonAudio(); });

        string text1 = "please place your bet";
        string text2 = "betting on 243 win ways";
        Textroutine = StartCoroutine(FlickerText(text1, text2));

        tweenHeight = (myImages.Length * IconSizeFactor) - 280;
    }

    private IEnumerator FlickerText(string text1, string text2 = null)
    {
        while(true)
        {
            if (MainDisplayText) MainDisplayText.text = text1;
            yield return new WaitForSeconds(1);
            if (MainDisplayText) MainDisplayText.text = text2;
            yield return new WaitForSeconds(1);
        }
    }

    private void AutoSpin()
    {
        if (!IsAutoSpin)
        {
            IsAutoSpin = true;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(true);
            if (AutoSpinStart_Button) AutoSpinStart_Button.gameObject.SetActive(false);

            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
            AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());
        }
    }

    internal void FreeSpin(int spins)
    {
        if (!IsFreeSpin)
        {
            IsFreeSpin = true;
            uiManager.SwitchFreeSpinMode(true);
            ToggleButtonGrp(false);

            if (NormalImage_Object) NormalImage_Object.SetActive(false);
            if (Slider_Object) Slider_Object.SetActive(true);

            if (TotalSpins_Text) TotalSpins_Text.text = spins.ToString();
            if (SpinUtilised_Text) SpinUtilised_Text.text = spins.ToString();
            if (SpinLeft_Text) SpinLeft_Text.text = "0";
            if (FreeSpin_Slider) FreeSpin_Slider.value = 1;

            if (FreeSpinRoutine != null)
            {
                StopCoroutine(FreeSpinRoutine);
                FreeSpinRoutine = null;
            }
            FreeSpinRoutine = StartCoroutine(FreeSpinCoroutine(spins));

        }
    }

    private IEnumerator FreeSpinCoroutine(int spinchances)
    {
        int i = 0;
        float step = 1f / spinchances;
        while (i < spinchances)
        {
            uiManager.UpdateFreeSpinUI(SocketManager.resultData.freeSpin.freeSpinMultipliers);
            if (SpinUtilised_Text) SpinUtilised_Text.text = (spinchances - i - 1).ToString();
            if (SpinLeft_Text) SpinLeft_Text.text = (i + 1).ToString();
            if (FreeSpin_Slider) FreeSpin_Slider.DOValue(FreeSpin_Slider.value - step, 0.2f);
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
            i++;
        }
        if (NormalImage_Object) NormalImage_Object.SetActive(true);
        if (Slider_Object) Slider_Object.SetActive(false);
        uiManager.SwitchFreeSpinMode(false);
        ToggleButtonGrp(true);
        IsFreeSpin = false;
    }

    private void StopAutoSpin()
    {
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
            if (AutoSpinStart_Button) AutoSpinStart_Button.gameObject.SetActive(true);
            StartCoroutine(StopAutoSpinCoroutine());
        }
    }

    private IEnumerator AutoSpinCoroutine()
    {
        while (IsAutoSpin)
        {
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
        }
    }

    private IEnumerator StopAutoSpinCoroutine()
    {
        yield return new WaitUntil(() => !IsSpinning);
        ToggleButtonGrp(true);
        if (AutoSpinRoutine != null || tweenroutine != null)
        {
            StopCoroutine(AutoSpinRoutine);
            StopCoroutine(tweenroutine);
            tweenroutine = null;
            AutoSpinRoutine = null;
            StopCoroutine(StopAutoSpinCoroutine());
        }
    }

    //Fetch Lines from backend
    internal void FetchLines(string LineVal, int count)
    {
        y_string.Add(count, LineVal);
    }

    private void MaxBet()
    {
        if (audioController) audioController.PlayButtonAudio();
        BetCounter = SocketManager.initialData.Bets.Count - 1;
        if (TotalBet_text) TotalBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (MainBet_text) MainBet_text.text = (SocketManager.initialData.Bets[BetCounter] * 25).ToString();
    }

    private void ChangeLine(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();
        //if (IncDec)
        //{
        //    if(LineCounter < SocketManager.initialData.LinesCount.Count - 1)
        //    {
        //        LineCounter++;
        //    }
        //}
        //else
        //{
        //    if (LineCounter > 0)
        //    {
        //        LineCounter--;
        //    }
        //}

        //if (Lines_text) Lines_text.text = SocketManager.initialData.LinesCount[LineCounter].ToString();

    }


    private void ChangeBet(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (IncDec)
        {
            if (BetCounter < SocketManager.initialData.Bets.Count - 1)
            {
                BetCounter++;
            }
        }
        else
        {
            if (BetCounter > 0)
            {
                BetCounter--;
            }
        }

        if (TotalBet_text) TotalBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (MainBet_text) MainBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString();
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;

        //HACK: To Be Uncommented After Parse Sheet Recieving
        CompareBalance();
    }

    internal void SetInitialUI()
    {
        //HACK: To Be Uncommented After Parse Sheet Recieving
        currentBalance = double.Parse(SocketManager.playerdata.Balance);

        BetCounter = 0;
        LineCounter = 0;

        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        if (TotalBet_text) TotalBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (MainBet_text) MainBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString();
        //if (Lines_text) Lines_text.text = SocketManager.initialData.LinesCount[LineCounter].ToString();
        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.currentWining.ToString("F2");
        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString();

        //HACK: To Be Uncommented After Parse Sheet Recieving
        CompareBalance();

        uiManager.InitialiseUIData(SocketManager.initUIData.AbtLogo.link, SocketManager.initUIData.AbtLogo.logoSprite, SocketManager.initUIData.ToULink, SocketManager.initUIData.PopLink, SocketManager.initUIData.paylines);
    }

    //reset the layout after populating the slots
    internal void LayoutReset(int number)
    {
        if (Slot_Elements[number]) Slot_Elements[number].ignoreLayout = true;
        if (SlotStart_Button) SlotStart_Button.interactable = true;
    }

    //function to populate animation sprites accordingly
    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        animScript.AnimationSpeed = 15f;
        switch (val)
        {
            case 0:
                for (int i = 0; i < Plane_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Plane_Sprite[i]);
                }
                break;
            case 1:
                for (int i = 0; i < Ship_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Ship_Sprite[i]);
                }
                break;
            case 2:
                for (int i = 0; i < Car_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Car_Sprite[i]);
                }
                break;
            case 3:
                for (int i = 0; i < Watch_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Watch_Sprite[i]);
                }
                break;
            case 4:
                for (int i = 0; i < Diamond_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Diamond_Sprite[i]);
                }
                break;
            case 11:
                for (int i = 0; i < FiveStar_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(FiveStar_Sprite[i]);
                }
                //animScript.AnimationSpeed = 20f;
                break;
            case 12:
                for (int i = 0; i < Bottle_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Bottle_Sprite[i]);
                }
                //animScript.AnimationSpeed = 20f;
                break;
            default:
                    break;
        }
    }

    //starts the spin process
    private void StartSlots(bool autoSpin = false)
    {
        if (audioController) audioController.PlaySpinClickedAudio();
        if (!IsFreeSpin)
        {
            if (Textroutine != null)
            {
                StopCoroutine(Textroutine);
                Textroutine = null;
            }
            Textroutine = StartCoroutine(FlickerText("Good luck !!!"));
        }
        else
        {
            if (Textroutine != null)
            {
                StopCoroutine(Textroutine);
                Textroutine = null;
            }
            Textroutine = StartCoroutine(FlickerText("Free spins in progress !!!"));
        }
        //if (uiManager) uiManager.ClosePopup();
        if(!autoSpin)
        {
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                StopCoroutine(tweenroutine);
                tweenroutine = null;
                AutoSpinRoutine = null;
            }
        }

        if (SlotStart_Button) SlotStart_Button.interactable = false;
        tweenroutine = StartCoroutine(TweenRoutine());
    }

    internal void UpdateBottomUI(bool won, double win, double balance)
    {
        currentBalance = balance;

        Balance_text.text = currentBalance.ToString("F2");
        TotalWin_text.text = win.ToString("F2");

        if (!won)
        {
            m_GambleController.ResetToDefault();
        }
    }

    //manage the Routine for spinning of the slots
    private IEnumerator TweenRoutine()
    {
        if (currentBalance < currentTotalBet && !IsFreeSpin)
        {
            CompareBalance();
            StopAutoSpin();
            yield return new WaitForSeconds(1);
            ToggleButtonGrp(true);
            yield break;
        }
        if (audioController) audioController.PlaySpinAudio(true);
        CheckSpinAudio = true;

        StopGameAnimation();
        IsSpinning = true;
        ToggleButtonGrp(false);

        //HACK: Number of spins initialized
        for (int i = 0; i < numberOfSlots; i++)
        {
            InitializeTweening(Slot_Transform[i]);
            yield return new WaitForSeconds(0.1f);
        }

        double bet = 0;
        double balance = 0;
        try
        {
            bet = double.Parse(TotalBet_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            balance = double.Parse(Balance_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        balance = balance - bet;

        if (Balance_text) Balance_text.text = balance.ToString();

        SocketManager.AccumulateResult(BetCounter);

        //HACK: Waits until the result is came from the backend
        yield return new WaitUntil(() => SocketManager.isResultdone);

        //HACK: Updates the result data accordingly.
        for(int j = 0; j < SocketManager.resultData.FinalResultReel.Count; j++)
        {
            List<int> resultnum = SocketManager.resultData.FinalResultReel[j]?.Split(',')?.Select(Int32.Parse)?.ToList();
            Debug.Log(string.Join(",", resultnum));

            for(int i = 0; i < resultnum.Count; i++)
            {
                if (Tempimages[i].slotImages[j]) Tempimages[i].slotImages[j].transform.GetChild(0).GetComponent<Image>().sprite = myImages[resultnum[i]];
            }
        }

        //HACK: Assign the animation sprites to the texture array       
        Assign();

        yield return new WaitForSeconds(0.5f);

        //HACK: Stops the initialized tweening.
        for (int i = 0; i < numberOfSlots; i++)
        {
            yield return StopTweening(5, Slot_Transform[i], i);
        }

        yield return new WaitForSeconds(0.3f);

        KillAllTweens();
        PlayStopAnimation(true);

        if (audioController) audioController.PlaySpinAudio(false);
        CheckSpinAudio = false;

        m_L_Bar.text = SocketManager.resultData.winningCombinations.Count.ToString();
        m_R_Bar.text = SocketManager.resultData.winningCombinations.Count.ToString();

        CheckPopups = true;

        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.currentWining.ToString("F2");

        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString();

        currentBalance = double.Parse(SocketManager.playerdata.Balance);

        if(!IsAutoSpin && !IsFreeSpin && !SocketManager.resultData.freeSpin.isFreeSpin && SocketManager.playerdata.currentWining > 0)
        {
            m_GambleController.CheckGamble();

            SlotStart_Button.gameObject.SetActive(false);
            Take_Button.gameObject.SetActive(true);
        }

        CheckWinPopups();
        //CheckBonusGame();

        if (IsFreeSpin || SocketManager.resultData.freeSpin.isFreeSpin)
        {
            yield return new WaitForSeconds(4);
        }


        yield return new WaitUntil(() => !CheckPopups);

        CheckPopups = true;

        if (!IsAutoSpin && !IsFreeSpin)
        {
            ToggleButtonGrp(true);
            IsSpinning = false;
        }
        else
        {
            yield return new WaitForSeconds(2f);
            IsSpinning = false;
        }

        if (SocketManager.resultData.freeSpin.isFreeSpin)
        {
            if (IsFreeSpin)
            {
                IsFreeSpin = false;
                if (FreeSpinRoutine != null)
                {
                    StopCoroutine(FreeSpinRoutine);
                    FreeSpinRoutine = null;
                }
            }
            uiManager.FreeSpinProcess((int)SocketManager.resultData.freeSpin.freeSpinCount);
            if (IsAutoSpin)
            {
                StopAutoSpin();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    /// <summary>
    /// HACK: The below method is used to assign the populated images animation sprites so that they could be played when required.
    /// </summary>
    private void Assign()
    {
        for(int i = 0; i < SocketManager.resultData.winningCombinations.Count; i++)
        {
            if(SocketManager.resultData.winningCombinations[i].positions.Count == 5 && !m_Is_5_Of_A_Kind)
            {
                m_Is_5_Of_A_Kind = true;
            }
            for(int j = 0; j < SocketManager.resultData.winningCombinations[i].positions.Count; j++)
            {
                var _Zero = SocketManager.resultData.winningCombinations[i].positions[j][0];
                var _One = SocketManager.resultData.winningCombinations[i].positions[j][1];
                int id = SocketManager.resultData.resultSymbols[_Zero][_One];

                if (m_Is_5_Of_A_Kind && id == 11)
                {
                    m_Is_5_Of_A_Kind = false;
                }

                PopulateAnimationSprites(Tempimages[_One].slotImages[_Zero].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>(), id);
            }
        }
    }

    internal void CheckWinPopups()
    {
        //Check Winnings;
        if (SocketManager.resultData.winningCombinations.Count > 0)
        {
            if (m_Is_5_Of_A_Kind)
            {
                //Play The 5 Of A Kind Animation
                if (audioController) audioController.PlayMegaWinAudio();
                uiManager.PopulateWin(2, SocketManager.playerdata.currentWining);
            }
            else if (SocketManager.playerdata.currentWining >= (currentTotalBet * Lines))
            {
                //Play The Big Win Animation
                if (audioController) audioController.PlayMegaWinAudio();
                uiManager.PopulateWin(1, SocketManager.playerdata.currentWining);
            }
            else
            {
                if (audioController) audioController.PlayWinAudio();
                CheckPopups = false;
            }
        }
        else
        {
            CheckPopups = false;
        }
    }

    /// <summary>
    /// HACK: A Toggling Function that handles the play and stop of the animations on the slotimages
    /// </summary>
    /// <param name="m_play"></param>
    private void PlayStopAnimation(bool m_play)
    {
        if (m_play)
        {

            if (AnimationToggleRoutine == null)
            {
                m_Is_Playing_Animation_In_Loop = true;
                AnimationToggleRoutine = StartCoroutine(PlayCoroutine());
            }
        }
        else
        {
            if (AnimationToggleRoutine != null)
            {
                StopCoroutine(AnimationToggleRoutine);
                AnimationToggleRoutine = null;
                m_Is_Playing_Animation_In_Loop = false;
                TurnAllMiniImagesOff();
            }
        }
    }

    /// <summary>
    /// HACK: To show multiple combinations in a loop with an delay between first combination and second one.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayCoroutine()
    {
        int _Zero = 0;
        int _One = 0;
        while (m_Is_Playing_Animation_In_Loop)
        {
            for(int i = 0; i < SocketManager.resultData.winningCombinations.Count; i++)
            {
                Color mycolor = SetRandomBrightColor();
                for (int j = 0; j < SocketManager.resultData.winningCombinations[i].positions.Count; j++)
                {
                    _Zero = SocketManager.resultData.winningCombinations[i].positions[j][0];
                    _One = SocketManager.resultData.winningCombinations[i].positions[j][1];
                    int id = SocketManager.resultData.resultSymbols[_Zero][_One];

                    anim = Tempimages[_One].slotImages[_Zero].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>();
                    if (anim.textureArray.Count > 0)
                    {
                        anim.StartAnimation();
                    }
                    else
                    {
                        anim.transform.parent.GetChild(1).gameObject.SetActive(true);
                        anim.transform.parent.GetChild(2).gameObject.SetActive(true);
                    }
                    Tempimages[_One].MiniImages[_Zero].color = mycolor;
                    Tempimages[_One].MiniImages[_Zero].gameObject.SetActive(true);
                    Tempimages[_One].slotImages[_Zero].gameObject.GetComponent<SlotScript>().SetBox(mycolor);
                    m_TempList.Add(anim);
                }
                yield return new WaitForSeconds(2f);
                foreach(var t in m_TempList)
                {
                    if (t.textureArray.Count > 0)
                    {
                        t.StopAnimation();
                    }
                    else
                    {
                        t.transform.parent.GetChild(1).gameObject.SetActive(false);
                        t.transform.parent.GetChild(2).gameObject.SetActive(false);
                    }
                }
                yield return new WaitForSeconds(0.2f);
                TurnAllMiniImagesOff();
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private void TurnAllMiniImagesOff()
    {
        for(int i = 0; i < Tempimages.Count; i++)
        {
            for(int j = 0; j < Tempimages[i].MiniImages.Count; j++)
            {
                Tempimages[i].MiniImages[j].gameObject.SetActive(false);
                Tempimages[i].slotImages[j].gameObject.GetComponent<SlotScript>().ResetBox();
            }
        }
        m_TempList.Clear();
    }

    private void CompareBalance()
    {
        if (currentBalance < currentTotalBet)
        {
            uiManager.LowBalPopup();
        }
    }

    internal void CallCloseSocket()
    {
        SocketManager.CloseSocket();
    }

    Color SetRandomBrightColor()
    {
        // Generate random values for Hue, Saturation, and Value (Brightness)
        float h = UnityEngine.Random.Range(0f, 1f); // Random hue
        float s = UnityEngine.Random.Range(0.5f, 1f); // Saturation to ensure bright colors
        float v = UnityEngine.Random.Range(0.8f, 1f); // Brightness, avoid very dark colors

        // Convert HSV to RGB
        Color randomColor = Color.HSVToRGB(h, s, v);

        // Assign the random color to the image
        return randomColor;
    }

    internal void CheckBonusGame()
    {
        CheckPopups = false;

        if (SocketManager.resultData.freeSpin.isFreeSpin)
        {
            if (IsAutoSpin)
            {
                StopAutoSpin();
            }
        }
    }

    void ToggleButtonGrp(bool toggle)
    {

        if (SlotStart_Button) SlotStart_Button.interactable = toggle;
        if (MaxBet_Button) MaxBet_Button.interactable = toggle;
        if (AutoSpinStart_Button) AutoSpinStart_Button.interactable = toggle;
        if (LinePlus_Button) LinePlus_Button.interactable = toggle;
        if (LineMinus_Button) LineMinus_Button.interactable = toggle;
        if (BetMinus_Button) BetMinus_Button.interactable = toggle;
        if (BetPlus_Button) BetPlus_Button.interactable = toggle;

    }

    //start the icons animation
    private void StartGameAnimation(GameObject animObjects)
    {
        ImageAnimation temp = animObjects.transform.GetChild(0).GetComponent<ImageAnimation>();
        Image tempImg = animObjects.GetComponent<Image>();
        if (temp.textureArray.Count > 0)
        {
            temp.StartAnimation();
        }
        else
        {
            temp.currentAnimationState = ImageAnimation.ImageState.PLAYING;
            tempImg.DOFade(0.3f, 1f)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo).SetId("fadeLoop");
        }
        TempList.Add(temp);
        TempListImg.Add(tempImg);
    }

    //stop the icons animation
    private void StopGameAnimation()
    {
        DOTween.Kill("fadeLoop");
        PlayStopAnimation(false);
        for(int i = 0; i < Tempimages.Count; i++)
        {
            for(int j = 0; j < Tempimages[i].slotImages.Count; j++)
            {
                ImageAnimation temp_Anim = Tempimages[i].slotImages[j].transform.GetChild(0).GetComponent<ImageAnimation>();
                if (temp_Anim.textureArray.Count > 0)
                {
                    temp_Anim.StopAnimation();
                    temp_Anim.textureArray.Clear();
                    temp_Anim.textureArray.TrimExcess();
                }
                else
                {
                    temp_Anim.currentAnimationState = ImageAnimation.ImageState.NONE;
                    //Color newColor = TempListImg[i].color;
                    //newColor.a = 1.0f;
                    //TempListImg[i].color = newColor;
                }
            }
        }
    }

    internal void shuffleInitialMatrix()
    {
        for (int i = 0; i < Tempimages.Count; i++)
        {
            for (int j = 0; j < Tempimages[i].slotImages.Count; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, myImages.Length);
                Tempimages[i].slotImages[j].transform.GetChild(0).GetComponent<Image>().sprite = myImages[randomIndex];
            }
        }
    }

    #region TweeningCode
    private void InitializeTweening(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener.Play();
        alltweens.Add(tweener);
    }



    private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index)
    {
        alltweens[index].Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        alltweens[index] = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.5f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(0.4f);
    }


    private void KillAllTweens()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
    #endregion

}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
    public List<RectTransform> slotRects = new List<RectTransform>(10);
    public List<Image> MiniImages;
}

