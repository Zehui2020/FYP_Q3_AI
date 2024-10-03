using UnityEngine;
using UnityEngine.Events;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private SimpleAnimation keycodeUI;
    [SerializeField] private WeaponData weapon;
    private ObjectBobbing bobbing;

    public UnityEvent OnPickup;

    private void Start()
    {
        bobbing = GetComponent<ObjectBobbing>();
        bobbing.InitBobbing();
    }

    public bool OnInteract()
    {
        keycodeUI.Hide();
        PlayerController.Instance.PickupWeapon(weapon);
        OnPickup?.Invoke();
        gameObject.SetActive(false);
        return true;
    }

    public void OnEnterRange()
    {
        keycodeUI.Show();
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
    }
}