using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public void ShowText(bool show)
    {
        if (_text != null) _text.enabled = show;
    }
}
