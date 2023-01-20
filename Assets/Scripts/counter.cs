using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class counter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    int amount = 0;

    public void UpdateCounter()
    {
        amount++;
        text.text = amount.ToString();
    }

    public void ResetCounter()
    {
        amount = 0;
        text.text = amount.ToString();
    }
}
