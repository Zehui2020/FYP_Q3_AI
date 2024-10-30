using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class MoneyPopupCounter : MonoBehaviour
{
    [SerializeField] private Animator aniamtor;
    [SerializeField] private TextMeshProUGUI moneyText;
    private int accumulatedMoney;

    private bool canPlay = true;

    public void AddMoney(int amount)
    {
        if (canPlay)
            aniamtor.Play("PopupEnter", -1, 0);

        accumulatedMoney += amount;
        moneyText.text = "+" + accumulatedMoney.ToString();
        canPlay = false;
    }

    public void CanPlay()
    {
        canPlay = true;
    }

    public void ResetCounter()
    {
        accumulatedMoney = 0;
        canPlay = true;
    }
}