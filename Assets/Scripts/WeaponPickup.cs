using UnityEngine;
using UnityEngine.Events;

public class WeaponPickup : MonoBehaviour, IInteractable
{
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
        PlayerController.Instance.PickupWeapon(weapon);
        OnPickup?.Invoke();
        gameObject.SetActive(false);
        return true;
    }

    public void OnEnterRange()
    {
    }

    public void OnLeaveRange()
    {
    }
}