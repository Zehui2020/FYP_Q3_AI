using UnityEngine;
using UnityEngine.Events;

public class WeaponPickup : Interactable
{
    [SerializeField] private WeaponData weapon;
    private ObjectBobbing bobbing;

    public UnityEvent OnPickup;

    private void Start()
    {
        bobbing = GetComponent<ObjectBobbing>();
        bobbing.InitBobbing();
    }

    public override bool OnInteract()
    {
        PlayerController.Instance.PickupWeapon(weapon);
        OnPickup?.Invoke();
        gameObject.SetActive(false);
        return true;
    }
}