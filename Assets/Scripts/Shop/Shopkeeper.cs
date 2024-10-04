using UnityEngine;

public class Shopkeeper : MonoBehaviour, IInteractable
{
    private ShopkeeperUIManager uiManager;
    private Shopkeeper_AI_Manager aiManager;
    private TextAnalysis textAnalysis;
    [SerializeField] private SimpleAnimation keycodeE;

    private void Awake()
    {
        uiManager = GetComponent<ShopkeeperUIManager>();
        aiManager = GetComponent<Shopkeeper_AI_Manager>();
        textAnalysis = GetComponent<TextAnalysis>();

        textAnalysis.OnMoodChanged += OnMoodChanged;
    }

    public void OnEnterRange()
    {
        keycodeE.Show();
    }

    public bool OnInteract()
    {
        SetupShop();
        uiManager.ShowUI();
        uiManager.isInteracting = true;
        return true;
    }

    public void OnLeaveRange()
    {
        keycodeE.Hide();
    }

    public void OnLeaveShopkeeper()
    {
        aiManager.OnLeaveShop();
        uiManager.isInteracting = false;
    }

    public void SetupShop()
    {

    }

    public void OnMoodChanged(int modifier)
    {

    }
}