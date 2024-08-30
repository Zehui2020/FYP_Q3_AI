using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private int cost;

    public void OnInteract()
    {
    }

    public void OnEnterRange()
    {
        costText.text = cost.ToString();
        canvas.gameObject.SetActive(true);
    }

    public void OnLeaveRange()
    {
        canvas.gameObject.SetActive(false);
    }
}