using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float MaxHealth = 100;
    private float _currentHealth;
    private CharacterController _characterController;
    public void TakeDamage(Damage dmg)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - dmg.amount);
        Debug.Log(_currentHealth);
        if (_currentHealth == 0)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            UI_PlayerStat.Instance.SetHealth(_currentHealth/MaxHealth);
            Debug.Log("플레이어 넉백!");
        }
    }

    private void Start()
    {
        _currentHealth = MaxHealth;
    }
}
