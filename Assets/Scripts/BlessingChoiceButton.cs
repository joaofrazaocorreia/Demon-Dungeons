using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class BlessingChoiceButton : MonoBehaviour
{
    public TextMeshProUGUI _name;
    public Image           _image;
    public TextMeshProUGUI _info;

    private void Start()
    {
        _image = GetComponentInChildren<Image>();
    }
}
