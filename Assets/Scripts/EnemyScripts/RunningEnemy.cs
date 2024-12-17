using UnityEngine;
using UnityEngine.AI;

public class RunningEnemy : Enemy
{
    [SerializeField] private float moveSpeed = 7f; // �������� �����
    private Transform player; // ��� ��������� ������
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float fireRate = 1.5f; // �������� ����� ���������
    [SerializeField] private float stopDistance = 12f; // ��������� ��������� �� ������
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] private int bulletDamage = 20;
    [SerializeField] private float inaccuracy = 0.1f; // ���������� ���� ������������ ������ � ������
    private WeaponSoundManager weaponSoundManager;
    [SerializeField] private float viewAngle = 120f; // ���� ������ �����
    [SerializeField] private float viewDistance = 30f; // ������������ ��������� ���������
    private float nextFireTime;
    [SerializeField] private LayerMask targetMask; // ����� ��� ������
    [SerializeField] private LayerMask obstacleMask; // ����� ��� �����������

    private NavMeshAgent agent; // NavMeshAgent ��� ���������
    private bool canSeePlayer = false; // ���� ��������� ������

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        nextFireTime = Time.time + Random.Range(0f, 1f); // ��������� �������� ��� ���������������� ������

        weaponSoundManager = GetComponentInChildren<WeaponSoundManager>();
        if (weaponSoundManager == null)
        {
            Debug.LogError("WeaponSoundManager �� ������ � ����� " + gameObject.name);
        }

        bulletDamage = EnemyStats.RunningEnemyBaseDamage;
        HP = EnemyStats.BaseHP;

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent �� ������ � ����� " + gameObject.name);
        }
        else
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stopDistance;
        }
    }

    void Update()
    {
        // ���������, ����� �� ������
        canSeePlayer = CanSeePlayer();

        // �������� � ��������
        MoveAndTrackPlayer();

        // ��������
        if (canSeePlayer && Time.time > nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void MoveAndTrackPlayer()
    {
        if (player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (canSeePlayer)
        {
            if (distanceToPlayer > stopDistance)
            {
                // ����� � ���� ������, �� ������ - �������� � ����
                agent.SetDestination(player.position);
            }
            else
            {
                // ����� � �������� ��������� ���������
                agent.ResetPath();
            }

            // �������������� � ������
            LookAtPlayer();
        }
        else
        {
            // ���� ����� ��� ���� ������, ���������� ��������� � ��� �����������
            agent.SetDestination(player.position);
        }
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // ������� ������ �����/����
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
    }

    private void Fire()
    {
        if (weaponSoundManager != null)
        {
            weaponSoundManager.PlayShootSound();
        }

        Vector3 shootingDirection = CalculateInaccurateDirection();
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetDamage(bulletDamage);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletSpeed, ForceMode.Impulse);
    }

    private Vector3 CalculateInaccurateDirection()
    {
        Vector3 direction = (player.position - bulletSpawn.position).normalized;
        direction.x += Random.Range(-inaccuracy, inaccuracy);
        direction.y += Random.Range(-inaccuracy, inaccuracy);
        return direction.normalized;
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // �������� ���� � ���������
        if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2 && distanceToPlayer < viewDistance)
        {
            // �������� �����������
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, viewDistance, obstacleMask | targetMask))
            {
                return hit.transform.CompareTag("Player");
            }
        }

        return false;
    }
}
