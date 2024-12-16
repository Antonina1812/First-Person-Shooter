using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int HP;
    [SerializeField] protected int Damage;

    void Start()
    {
        // ”станавливаем здоровье из глобального класса EnemyStats
        HP = EnemyStats.BaseHP;
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
            Destroy(gameObject);
        else
            Debug.Log("Enemy damaged");
    }

    public int GetDamage()
    {
        return Damage;
    }
}
