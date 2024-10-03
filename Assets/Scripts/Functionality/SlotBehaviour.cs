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
    private Sprite[] myImages;  //images taken initially

    [Header("Slot Images")]
    [SerializeField]
    private List<SlotImage> images;     //class to store total images
    [SerializeField]
    private List<SlotImage> Tempimages;     //class to store the result matrix

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
    private Button AutoSpin_Button;
    [SerializeField] private Button AutoSpinStop_Button;
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

    private Coroutine AutoSpinRoutine = null;
    private Coroutine FreeSpinRoutine = null;
    private Coroutine tweenroutine;
    private Coroutine Textroutine;

    private bool IsAutoSpin = false;
    private bool IsFreeSpin = false;
    private bool IsSpinning = false;
    internal bool CheckPopups = false;

    private int BetCounter = 0;
    private int LineCounter = 0;
    protected int Lines = 20;

    private double currentBalance = 0;
    private double currentTotalBet = 0;

    private void Start()
    {
        IsAutoSpin = false;
        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(delegate { StartSlots(); });

        if (BetPlus_Button) BetPlus_Button.onClick.RemoveAllListeners();
        if (BetPlus_Button) BetPlus_Button.onClick.AddListener(delegate { ChangeBet(true); });
        if (BetMinus_Button) BetMinus_Button.onClick.RemoveAllListeners();
        if (BetMinus_Button) BetMinus_Button.onClick.AddListener(delegate { ChangeBet(false); });

        if (LinePlus_Button) LinePlus_Button.onClick.RemoveAllListeners();
        if (LinePlus_Button) LinePlus_Button.onClick.AddListener(delegate { ChangeLine(true); });
        if (LineMinus_Button) LineMinus_Button.onClick.RemoveAllListeners();
        if (LineMinus_Button) LineMinus_Button.onClick.AddListener(delegate { ChangeLine(false); });

        if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

        if (AutoSpin_Button) AutoSpin_Button.onClick.RemoveAllListeners();
        if (AutoSpin_Button) AutoSpin_Button.onClick.AddListener(AutoSpin);


        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.RemoveAllListeners();
        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.AddListener(StopAutoSpin);

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
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(false);

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
            ToggleButtonGrp(false);
            if (NormalImage_Object) NormalImage_Object.SetActive(false);
            if (TotalSpins_Text) TotalSpins_Text.text = spins.ToString();
            if (SpinUtilised_Text) SpinUtilised_Text.text = spins.ToString();
            if (SpinLeft_Text) SpinLeft_Text.text = "0";
            if (FreeSpin_Slider) FreeSpin_Slider.value = 1;
            if (Slider_Object) Slider_Object.SetActive(true);

            if (FreeSpinRoutine != null)
            {
                StopCoroutine(FreeSpinRoutine);
                FreeSpinRoutine = null;
            }
            FreeSpinRoutine = StartCoroutine(FreeSpinCoroutine(spins));

        }
    }

    private void StopAutoSpin()
    {
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
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

    private IEnumerator FreeSpinCoroutine(int spinchances)
    {
        int i = 0;
        float step = 1f / spinchances;
        while (i < spinchances)
        {
            if (SpinUtilised_Text) SpinUtilised_Text.text = (spinchances - i - 1).ToString();
            if (SpinLeft_Text) SpinLeft_Text.text = (i + 1).ToString();
            if (FreeSpin_Slider) FreeSpin_Slider.DOValue(FreeSpin_Slider.value - step, 0.2f);
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
            i++;
        }
        ToggleButtonGrp(true);
        IsFreeSpin = false;
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
        if (IncDec)
        {
            if(LineCounter < SocketManager.initialData.LinesCount.Count - 1)
            {
                LineCounter++;
            }
        }
        else
        {
            if (LineCounter > 0)
            {
                LineCounter--;
            }
        }

        if (Lines_text) Lines_text.text = SocketManager.initialData.LinesCount[LineCounter].ToString();

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
        if (MainBet_text) MainBet_text.text = (SocketManager.initialData.Bets[BetCounter] * 25).ToString();
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * 25;

        //HACK: To Be Uncommented After Parse Sheet Recieving
        CompareBalance();
    }


    //just for testing purposes delete on production
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) && SlotStart_Button.interactable)
    //    {
    //        StartSlots();
    //    }
    //}

    //populate the slots with the values recieved from backend
    //internal void PopulateInitalSlots(int number, List<int> myvalues)
    //{
    //    PopulateSlot(myvalues, number);
    //}

    internal void SetInitialUI()
    {
        //HACK: To Be Uncommented After Parse Sheet Recieving
        currentBalance = SocketManager.playerdata.Balance;
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;

        BetCounter = 0;
        LineCounter = 0;

        if (TotalBet_text) TotalBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (MainBet_text) MainBet_text.text = (SocketManager.initialData.Bets[BetCounter] * 25).ToString();
        if (Lines_text) Lines_text.text = SocketManager.initialData.LinesCount[LineCounter].ToString();
        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.haveWon.ToString();
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

    private void PopulateSlot(List<int> values, int number)
    {
        if (Slot_Objects[number]) Slot_Objects[number].SetActive(true);
        for (int i = 0; i < values.Count; i++)
        {
            GameObject myImg = Instantiate(Image_Prefab, Slot_Transform[number]);
            images[number].slotImages.Add(myImg.transform.GetChild(0).GetComponent<Image>());
            images[number].slotRects.Add(myImg.transform.GetChild(0).GetComponent<RectTransform>());
            if (values[i] == 11)
            {
                images[number].slotRects[i].offsetMin = new Vector2(-60, -60);
                images[number].slotRects[i].offsetMax = new Vector2(60, 60);
            }
            else if (values[i] == 12)
            {
                images[number].slotRects[i].offsetMin = new Vector2(-30, -30);
                images[number].slotRects[i].offsetMax = new Vector2(30, 30);
            }
            else
            {
                images[number].slotRects[i].offsetMin = new Vector2(0, 0);
                images[number].slotRects[i].offsetMax = new Vector2(0, 0);
            }
            images[number].slotImages[i].sprite = myImages[values[i]];
        }
        for (int k = 0; k < 2; k++)
        {
            GameObject mylastImg = Instantiate(Image_Prefab, Slot_Transform[number]);
            images[number].slotImages.Add(mylastImg.transform.GetChild(0).GetComponent<Image>());
            images[number].slotRects.Add(mylastImg.transform.GetChild(0).GetComponent<RectTransform>());
            if (values[k] == 11)
            {
                images[number].slotRects[images[number].slotRects.Count - 1].offsetMin = new Vector2(-60, -60);
                images[number].slotRects[images[number].slotRects.Count - 1].offsetMax = new Vector2(60, 60);
            }
            else if (values[k] == 12)
            {
                images[number].slotRects[images[number].slotRects.Count - 1].offsetMin = new Vector2(-30, -30);
                images[number].slotRects[images[number].slotRects.Count - 1].offsetMax = new Vector2(30, 30);
            }
            else
            {
                images[number].slotRects[images[number].slotRects.Count - 1].offsetMin = new Vector2(0, 0);
                images[number].slotRects[images[number].slotRects.Count - 1].offsetMax = new Vector2(0, 0);
            }
            images[number].slotImages[images[number].slotImages.Count - 1].sprite = myImages[values[k]];
        }
        if (mainContainer_RT) LayoutRebuilder.ForceRebuildLayoutImmediate(mainContainer_RT);
        tweenHeight = (values.Count * IconSizeFactor) - 280;
        GenerateMatrix(number);
    }

    //function to populate animation sprites accordingly
    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        animScript.AnimationSpeed = 36f;
        switch (val)
        {
            case 6:
                for (int i = 0; i < Diamond_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Diamond_Sprite[i]);
                }
                break;
            case 7:
                for (int i = 0; i < Watch_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Watch_Sprite[i]);
                }
                break;
            case 8:
                for (int i = 0; i < Car_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Car_Sprite[i]);
                }
                break;
            case 9:
                for (int i = 0; i < Ship_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Ship_Sprite[i]);
                }
                break;
            case 10:
                for (int i = 0; i < Plane_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Plane_Sprite[i]);
                }
                break;
            case 11:
                for (int i = 0; i < Bottle_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Bottle_Sprite[i]);
                }
                animScript.AnimationSpeed = 60f;
                break;
            case 12:
                for (int i = 0; i < FiveStar_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(FiveStar_Sprite[i]);
                }
                animScript.AnimationSpeed = 96f;
                break;
            default:
                    break;
        }
    }

    //starts the spin process
    private void StartSlots(bool autoSpin = false)
    {
        if (audioController) audioController.PlayWLAudio("spin");
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
        if (TempList.Count > 0)
        {
            AnimStoppedProcess();
        }
        tweenroutine = StartCoroutine(TweenRoutine());
    }

    //manage the Routine for spinning of the slots
    private IEnumerator TweenRoutine()
    {
        if (currentBalance < currentTotalBet && !IsFreeSpin)
        {
            CompareBalance();
            StopAutoSpin();
            yield return new WaitForSeconds(1);
            yield break;
        }
        IsSpinning = true;
        ToggleButtonGrp(false);
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

        yield return new WaitUntil(() => SocketManager.isResultdone);


        //for (int j = 0; j < SocketManager.resultData.ResultReel.Count; j++)
        //{
        //    List<int> resultnum = SocketManager.resultData.FinalResultReel[j]?.Split(',')?.Select(Int32.Parse)?.ToList();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        if (Tempimages[i].slotImages[j]) Tempimages[i].slotImages[j].sprite = myImages[resultnum[i]];

        //        if (resultnum[i] == 11)
        //        {
        //            //if (images[i].slotRects[images[i].slotRects.Count - 5 + j]) images[i].slotRects[images[i].slotRects.Count - 5 + j].offsetMin = new Vector2(-60, -60);
        //            //if (images[i].slotRects[images[i].slotRects.Count - 5 + j]) images[i].slotRects[images[i].slotRects.Count - 5 + j].offsetMax = new Vector2(60, 60);
        //            PopulateAnimationSprites(Tempimages[i].slotImages[j].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>(), resultnum[i]);
        //        }
        //        else if (resultnum[i] == 12)
        //        {
        //            //if (images[i].slotRects[images[i].slotRects.Count - 5 + j]) images[i].slotRects[images[i].slotRects.Count - 5 + j].offsetMin = new Vector2(-30, -30);
        //            //if (images[i].slotRects[images[i].slotRects.Count - 5 + j]) images[i].slotRects[images[i].slotRects.Count - 5 + j].offsetMax = new Vector2(30, 30);
        //            PopulateAnimationSprites(Tempimages[i].slotImages[j].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>(), resultnum[i]);
        //        }
        //        else
        //        {
        //            //if (images[i].slotRects[images[i].slotRects.Count - 5 + j]) images[i].slotRects[images[i].slotRects.Count - 5 + j].offsetMin = new Vector2(0, 0);
        //            //if (images[i].slotRects[images[i].slotRects.Count - 5 + j]) images[i].slotRects[images[i].slotRects.Count - 5 + j].offsetMax = new Vector2(0, 0);
        //            PopulateAnimationSprites(Tempimages[i].slotImages[j].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>(), resultnum[i]);
        //        }
        //    }
        //}

        for(int j = 0; j < Tempimages.Count; j++)
        {
            List<int> resultnum = SocketManager.resultData.FinalResultReel[j]?.Split(',')?.Select(Int32.Parse)?.ToList();
            Debug.Log(string.Join(",", resultnum));
            for (int i = 0; i < Tempimages[j].slotImages.Count; i++)
            {
                if(resultnum[i] == 11)
                {
                    PopulateAnimationSprites(Tempimages[j].slotImages[i].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>(), resultnum[i]);
                }
                else if(resultnum[i] == 12)
                {
                    PopulateAnimationSprites(Tempimages[j].slotImages[i].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>(), resultnum[i]);
                }
                else
                {
                    PopulateAnimationSprites(Tempimages[j].slotImages[i].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>(), resultnum[i]);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < numberOfSlots; i++)
        {
            yield return StopTweening(5, Slot_Transform[i], i);
        }

        yield return new WaitForSeconds(0.3f);
        CheckPayoutLineBackend(SocketManager.resultData.linesToEmit, SocketManager.resultData.FinalsymbolsToEmit, SocketManager.resultData.jackpot);
        KillAllTweens();


        CheckPopups = true;

        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.haveWon.ToString();

        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString();

        currentBalance = SocketManager.playerdata.Balance;

        if (IsFreeSpin || SocketManager.resultData.freeSpins.count > 0)
        {
            yield return new WaitForSeconds(4);
        }

        CheckBonusGame();

        yield return new WaitUntil(() => !CheckPopups);
        if (!IsAutoSpin)
        {
            ToggleButtonGrp(true);
            IsSpinning = false;
        }
        else
        {
            IsSpinning = false;
        }
        if (SocketManager.resultData.freeSpins.count > 0)
        {
            uiManager.FreeSpinProcess((int)SocketManager.resultData.freeSpins.count);
        }
    }

    private void CompareBalance()
    {
        if (currentBalance < currentTotalBet)
        {
            uiManager.LowBalPopup();
            if (AutoSpin_Button) AutoSpin_Button.interactable = false;
            if (SlotStart_Button) SlotStart_Button.interactable = false;
        }
        else
        {
            if (AutoSpin_Button) AutoSpin_Button.interactable = true;
            if (SlotStart_Button) SlotStart_Button.interactable = true;
        }
    }

    internal void CallCloseSocket()
    {
        SocketManager.CloseSocket();
    }

    [SerializeField]
    private List<int> TempLineIds;

    private IEnumerator slotLineAnim()
    {
        int n = 0;
        if (TempLineIds.Count > 1)
        {
            while (n < 5)
            {
                List<int> y_anim = null;
                for (int i = 0; i < TempLineIds.Count; i++)
                {
                    y_anim = y_string[TempLineIds[i]]?.Split(',')?.Select(Int32.Parse)?.ToList();

                    Color mycolor = SetRandomBrightColor();
                    for (int k = 0; k < y_anim.Count; k++)
                    {
                        if (Tempimages[k].slotImages[y_anim[k]].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>().currentAnimationState == ImageAnimation.ImageState.PLAYING)
                        {
                            Tempimages[k].slotImages[y_anim[k]].gameObject.GetComponent<SlotScript>().SetBox(mycolor);
                            Tempimages[k].MiniImages[y_anim[k]].color = mycolor;
                            Tempimages[k].MiniImages[y_anim[k]].gameObject.SetActive(true);
                        }
                    }
                    yield return new WaitForSeconds(3);
                    for (int k = 0; k < y_anim.Count; k++)
                    {
                        if (Tempimages[k].slotImages[y_anim[k]].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>().currentAnimationState == ImageAnimation.ImageState.PLAYING)
                        {
                            Tempimages[k].slotImages[y_anim[k]].gameObject.GetComponent<SlotScript>().ResetBox();
                            Tempimages[k].MiniImages[y_anim[k]].gameObject.SetActive(false);
                        }
                    }
                }
                for (int i = 0; i < TempLineIds.Count; i++)
                {
                    List<int> y_all = null;
                    y_all = y_string[TempLineIds[i]]?.Split(',')?.Select(Int32.Parse)?.ToList();

                    for (int k = 0; k < y_all.Count; k++)
                    {
                        if (Tempimages[k].slotImages[y_all[k]].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>().currentAnimationState == ImageAnimation.ImageState.PLAYING)
                        {
                            Tempimages[k].slotImages[y_all[k]].gameObject.GetComponent<SlotScript>().DefaultBox();
                            Tempimages[k].MiniImages[y_all[k]].color = Color.white;
                            Tempimages[k].MiniImages[y_all[k]].gameObject.SetActive(true);
                        }
                    }
                }
                yield return new WaitForSeconds(3);
                for (int i = 0; i < TempLineIds.Count; i++)
                {
                    List<int> y_all = null;
                    y_all = y_string[TempLineIds[i]]?.Split(',')?.Select(Int32.Parse)?.ToList();

                    for (int k = 0; k < y_all.Count; k++)
                    {
                        if (Tempimages[k].slotImages[y_all[k]].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>().currentAnimationState == ImageAnimation.ImageState.PLAYING)
                        {
                            Tempimages[k].slotImages[y_all[k]].gameObject.GetComponent<SlotScript>().ResetBox();
                            Tempimages[k].MiniImages[y_all[k]].gameObject.SetActive(false);
                        }
                    }
                }
                n++;
            }
        }
        else
        {
            List<int> y_all = null;
            for (int i = 0; i < TempLineIds.Count; i++)
            {
                y_all = y_string[TempLineIds[i]]?.Split(',')?.Select(Int32.Parse)?.ToList();

                for (int k = 0; k < y_all.Count; k++)
                {
                    if (Tempimages[k].slotImages[y_all[k]].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>().currentAnimationState == ImageAnimation.ImageState.PLAYING)
                    {
                        Tempimages[k].slotImages[y_all[k]].gameObject.GetComponent<SlotScript>().DefaultBox();
                        Tempimages[k].MiniImages[y_all[k]].color = Color.white;
                        Tempimages[k].MiniImages[y_all[k]].gameObject.SetActive(true);
                    }
                }
            }
            yield return new WaitForSeconds(9);
        }
        AnimStoppedProcess();
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

        if (SocketManager.resultData.freeSpins.count > 0) 
        {
            if(IsAutoSpin)
            {
                StopAutoSpin();
            }
        }
    }

    void ToggleButtonGrp(bool toggle)
    {

        if (SlotStart_Button) SlotStart_Button.interactable = toggle;
        if (MaxBet_Button) MaxBet_Button.interactable = toggle;
        if (AutoSpin_Button) AutoSpin_Button.interactable = toggle;
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
        for (int i = 0; i < TempList.Count; i++)
        {
            if (TempList[i].textureArray.Count > 0)
            {
                TempList[i].StopAnimation();
            }
            else
            {
                TempList[i].currentAnimationState = ImageAnimation.ImageState.NONE;
                Color newColor = TempListImg[i].color;
                newColor.a = 1.0f;
                TempListImg[i].color = newColor;
            }
        }
    }

    private Coroutine SlotAnimRoutine = null;

    //generate the payout lines generated 
    private void CheckPayoutLineBackend(List<int> LineId, List<string> points_AnimString, double jackpot = 0)
    {
        TempLineIds = LineId;
        List<int> y_points = null;
        List<int> points_anim = null;
        if (LineId.Count > 0)
        {
            if (Textroutine != null)
            {
                StopCoroutine(Textroutine);
                Textroutine = null;
            }
            Textroutine = StartCoroutine(FlickerText("You Win !!!"));
            if (audioController) audioController.PlayWLAudio("win");

            for (int i = 0; i < LineId.Count; i++)
            {
                y_points = y_string[LineId[i]-1]?.Split(',')?.Select(Int32.Parse)?.ToList();
            }

            if (jackpot > 0)
            {
                for (int i = 0; i < Tempimages.Count; i++)
                {
                    for (int k = 0; k < Tempimages[i].slotImages.Count; k++)
                    {
                        StartGameAnimation(Tempimages[i].slotImages[k].gameObject);
                    }
                }
            }
            else
            { 
                for (int i = 0; i < points_AnimString.Count; i++)
                {
                    points_anim = points_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

                    for (int k = 0; k < points_anim.Count; k++)
                    {
                        if (points_anim[k] >= 10)
                        {
                            StartGameAnimation(Tempimages[(points_anim[k] / 10) % 10].slotImages[points_anim[k] % 10].gameObject);
                        }
                        else
                        {
                            StartGameAnimation(Tempimages[0].slotImages[points_anim[k]].gameObject);
                        }
                    }
                }
            }
            if (SlotAnimRoutine != null)
            {
                StopCoroutine(SlotAnimRoutine);
                SlotAnimRoutine = null;
            }
            SlotAnimRoutine = StartCoroutine(slotLineAnim());
        }
        else
        {
            if(Textroutine != null)
            {
                StopCoroutine(Textroutine);
                Textroutine = null;
            }
            Textroutine = StartCoroutine(FlickerText("Better luck next time !!!"));
            if (audioController) audioController.PlayWLAudio("lose");
        }
    }

    private void AnimStoppedProcess()
    {
        for (int i = 0; i < TempLineIds.Count; i++)
        {
            List<int> y_all = y_string[TempLineIds[i]]?.Split(',')?.Select(Int32.Parse)?.ToList();

            for (int k = 0; k < y_all.Count; k++)
            {
                if (Tempimages[k].slotImages[y_all[k]].transform.GetChild(0).gameObject.GetComponent<ImageAnimation>().currentAnimationState == ImageAnimation.ImageState.PLAYING)
                {
                    Tempimages[k].slotImages[y_all[k]].gameObject.GetComponent<SlotScript>().ResetBox();
                    Tempimages[k].MiniImages[y_all[k]].gameObject.SetActive(false);
                }
            }
        }
        TempLineIds.Clear();
        TempLineIds.TrimExcess();
        StopGameAnimation();
    }

    //generate the result matrix
    private void GenerateMatrix(int value)
    {
        for (int j = 0; j < 3; j++)
        {
            Tempimages[value].slotImages.Add(images[value].slotImages[images[value].slotImages.Count - 5 + j]);
            Tempimages[value].slotRects.Add(images[value].slotRects[images[value].slotRects.Count - 5 + j]);
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
        yield return new WaitForSeconds(0.2f);
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

