using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [SerializeField] private PlayerCombatDataSO combatData;
    public int CurrentAmmo { get; private set; }
    public int CurrentGrenade { get; private set; }

    private void Awake()
    {
        CurrentAmmo = combatData.MaxAmmo;
        CurrentGrenade = combatData.MaxGrenade;
    }

    private void Start()
    {
        UI_PlayerStat.Instance.Init(new WeaponData { MaxAmmo = combatData.MaxAmmo, MaxGranade = combatData.MaxGrenade });
        UI_PlayerStat.Instance.SetAmmo(CurrentAmmo);
        UI_PlayerStat.Instance.SetGranade(CurrentGrenade);
    }
    public bool UseAmmo(int amount = 1)
    {
        if (CurrentAmmo < amount) return false;
        CurrentAmmo -= amount;
        UI_PlayerStat.Instance.SetAmmo(CurrentAmmo);
        return true;
    }

    public void Reload()
    {
        CurrentAmmo = combatData.MaxAmmo;
        UI_PlayerStat.Instance.SetAmmo(CurrentAmmo);
    }

    public bool UseGrenade()
    {
        if (CurrentGrenade <= 0) return false;
        CurrentGrenade--;
        UI_PlayerStat.Instance.SetGranade(CurrentGrenade);
        return true;
    }
}