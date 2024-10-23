using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Dialogue_Tree : MonoBehaviour
{
    [Header("Dialogue Option Data")]
    [SerializeField] private DialogueOptionData OptionData;

    [Header("Option Text")]
    [SerializeField] private TextMeshProUGUI OptionText;

    // Start is called before the first frame update
    void Start()
    {
        OptionText.text = OptionData.OptionTitle;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
