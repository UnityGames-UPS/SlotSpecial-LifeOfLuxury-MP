using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
    [SerializeField]
    private GameObject animrun_Object;
    [SerializeField]
    private GameObject animbox_Object;
    [SerializeField]
    private Image animrun_Image;
    [SerializeField]
    private Image animbox_Image;

    internal void SetBox(Color imgcolor)
    {
        if (animrun_Object) animrun_Object.SetActive(true);
        if (animbox_Object) animbox_Object.SetActive(true);
        if (animrun_Image) animrun_Image.color = imgcolor;
        if (animbox_Image) animbox_Image.color = imgcolor;
    }

    internal void ResetBox()
    {
        if (animrun_Object) animrun_Object.SetActive(false);
        if (animbox_Object) animbox_Object.SetActive(false);
    }

    internal void DefaultBox()
    {
        if (animrun_Object) animrun_Object.SetActive(true);
        if (animbox_Object) animbox_Object.SetActive(true);
        if (animrun_Image) animrun_Image.color = Color.white;
        if (animbox_Image) animbox_Image.color = Color.white;
    }
}
