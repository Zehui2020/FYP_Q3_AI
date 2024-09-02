using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemCountText;
    private int itemCount;

    public Item item;

    public void SetupItemUI(Item item)
    {
        this.item = item;

        itemCount++;
        itemIcon.sprite = item.spriteIcon;
        itemCountText.gameObject.SetActive(false);
    }

    public void AddItemCount()
    {
        itemCount++;

        if (itemCount > 1)
        {
            itemCountText.text = "x" + itemCount;
            itemCountText.gameObject.SetActive(true);
        }
    }
}