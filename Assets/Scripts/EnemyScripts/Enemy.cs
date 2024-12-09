using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int HP = 100;
    [SerializeField]
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {

    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
            Destroy(gameObject);
        else
            Debug.Log("Enemy damaged");
    }
}
