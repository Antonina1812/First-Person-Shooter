using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float delay = 2f; // �������� ����� �������
    [SerializeField] float damageRadius = 20f;
    [SerializeField] float explosionForce = 1200f; // ���� ������
    [SerializeField] Transform grenadeSpawnPoint; // ������� ��������� �������
    [SerializeField] float throwForce = 15f; // ���� ������
    [SerializeField] int maxGrenades = 3; // ������������ ���������� ������
    [SerializeField] private int grenadeDamage = 100; // ���� �� �������

    private int curr_level = 1;
    private int currentGrenades;
    private bool canThrow = true;
    private bool isInHand = true;
    private WeaponSwitch weaponSwitch;
    private GrenadeSoundManager grenadeSoundManager;

    private void Awake()
    {
        grenadeSoundManager = GetComponentInChildren<GrenadeSoundManager>();
    }
    private void Start()
    {
        currentGrenades = maxGrenades;
        weaponSwitch = FindObjectOfType<WeaponSwitch>();
    }

    private void Update()
    {
        ReturnGrenades();
        if (currentGrenades > 0 && isInHand)
        {
            canThrow = true;
        }
        if (Input.GetMouseButtonDown(1) && canThrow)
        {
            if (currentGrenades > 0)
            {
                ThrowGrenade();
                currentGrenades--;
                UpdateUI();

                if (currentGrenades == 0)
                {
                    weaponSwitch.SwitchToWeapon();
                }
            }
        }
    }

    private void ReturnGrenades()
    {
        if (curr_level < Current_Maze.level)
        {
            Debug.Log("Yes");
            currentGrenades = 3;
            weaponSwitch.UpdateGrenadeSwitch(true);
            curr_level = Current_Maze.level;
        }
    }
    private void ThrowGrenade()
    {
        if (grenadeSpawnPoint == null)
        {
            Debug.LogError("Grenade Spawn Point is not assigned in the inspector.");
            return;
        }

        GameObject grenade = Instantiate(gameObject, grenadeSpawnPoint.position, grenadeSpawnPoint.rotation);
        Rigidbody rb = grenade.AddComponent<Rigidbody>();
        rb.mass = 1f;
        Vector3 throwDirection = CalculateThrowDirection();
        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);

        Grenade grenadeComponent = grenade.GetComponent<Grenade>();
        grenadeComponent.canThrow = false; // ��������� ����������� ������ ��� ������� �������
        grenadeComponent.isInHand = false;
        grenadeComponent.StartExplosionCountdown(); // �������� ������ �� ������
    }

    private Vector3 CalculateThrowDirection()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        return ray.direction;
    }

    private void UpdateUI()
    {
        // ����� ��������� ������ UI ��� ����������� ���������� ������
        Debug.Log($"Grenades left: {currentGrenades}");
    }

    public void StartExplosionCountdown()
    {
        Invoke(nameof(Explode), delay); // ��������� ����� ����� �������� ��������
    }

    private void Explode()
    {
        CreateExplosionEffect();
        ApplyDamageAndForce();

        // ���������� �������� ��� ��������� �����
        AudioClip explosionClip = grenadeSoundManager.ExplosionSound;
        if (explosionClip != null)
        {
            AudioSource.PlayClipAtPoint(explosionClip, transform.position);
        }
        else
        {
            Debug.LogWarning("Explosion sound is not assigned!");
        }

        Destroy(gameObject);
    }

    private void CreateExplosionEffect()
    {
        GameObject explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
    }

    private void ApplyDamageAndForce()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider objectInRange in colliders)
        {
            // ��������� �������� �����������
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }

            // ������� ���� ������
            if (objectInRange.CompareTag("Enemy"))
            {
                Enemy enemy = objectInRange.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(grenadeDamage); // ���� ����������
                }
            }
        }
    }

}
