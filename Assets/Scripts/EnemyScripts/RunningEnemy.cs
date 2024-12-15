using UnityEngine;

public class RunningEnemy : Enemy
{
    [SerializeField] private float moveSpeed = 7f; //�������� �����
    [SerializeField] private Transform player; //��� ��������� ������
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float fireRate = 1.5f; //�������� ����� ���������
    [SerializeField] private float stopDistance = 12f; // ��������� ��������� �� ������
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] private int bulletDamage = 20;
    [SerializeField] private float inaccuracy = 0.25f; //���������� ���� ������������ ������ � ������
    private WeaponSoundManager weaponSoundManager;
    private float nextFireTime;

    void Start()
    {
        nextFireTime = Time.time + Random.Range(0f, 1f); // ��������� �������� ��� ���������������� ������
        weaponSoundManager = GetComponentInChildren<WeaponSoundManager>();
        if (weaponSoundManager == null)
        {
            Debug.LogError("WeaponSoundManager �� ������ � ����� " + gameObject.name);
        }
    }

    void Update()
    {
        // ��������
        MoveTowardsPlayer();

        // ��������
        if (Time.time > nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // ������������ ����� � ������
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else
        {
            // ��������������� � ������� �� ������
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }

    private void Fire()
    {
        // ����������� ���� ��������
        if (weaponSoundManager != null)
        {
            weaponSoundManager.PlayShootSound();
        }

        // ������ ���� � ��������� �� ����
        Vector3 shootingDirection = CalculateInaccurateDirection();
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetDamage(bulletDamage);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletSpeed, ForceMode.Impulse);
    }

    //������� ��������� ���������� �� ������ � ������
    private Vector3 CalculateInaccurateDirection()
    {
        Vector3 direction = (player.position - bulletSpawn.position).normalized;
        direction.x += Random.Range(-inaccuracy, inaccuracy);
        direction.y += Random.Range(-inaccuracy, inaccuracy);
        return direction.normalized;
    }
}
