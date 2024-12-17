using UnityEngine;
using UnityEngine.AI;

public class HidingEnemy : Enemy
{
    [Header("�������� ���������")]
    private Transform player; // ������ �� ������
    [SerializeField] private GameObject bulletPrefab; // ������ ����
    [SerializeField] private Transform bulletSpawn; // ����� ��������� ����
    [SerializeField] private float moveSpeed = 7f; // �������� ��������
    [SerializeField] private float detectionRange = 20f; // ������ ����������� ������
    [SerializeField] private float safeDistance = 5f; // ���������, �� ������� ���� ����� ������ �������
    [SerializeField] private float retreatDistance = 3f; // ��������� ��� ��������������� ���������

    [Header("��������")]
    [SerializeField] private float fireRate = 1.5f; // �������� ��������
    [SerializeField] private float bulletSpeed = 100f; // �������� ����
    [SerializeField] private float inaccuracy = 0.25f; // ������� ����
    [SerializeField] private int bulletDamage = 10; //���� ����

    [Header("�������")]
    [SerializeField] private LayerMask coverLayer; // ���� �������
    [SerializeField] private LayerMask obstacleLayer; // ���� �����������

    private NavMeshAgent agent;
    private float nextFireTime;
    private Transform currentCover; // ������� �������
    private Transform lastUsedCover; // ��������� �������
    private bool isRetreating; // ����, ������������, ��� ���� ���������
    private WeaponSoundManager weaponSoundManager;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.acceleration = 50f;
        agent.angularSpeed = 720f;

        bulletDamage = EnemyStats.HidingEnemyBaseDamage;
        HP = EnemyStats.BaseHP;

        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }


        FindCover();

        weaponSoundManager = GetComponentInChildren<WeaponSoundManager>();
        if (weaponSoundManager == null)
        {
            Debug.LogError("WeaponSoundManager �� ������ � ����� " + gameObject.name);
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= safeDistance && !isRetreating)
            {
                // ����� ������� ������ - ���� ������� ���������
                RetreatFromCover();
            }
            else if (isRetreating)
            {
                // ���������, ����������� �� �����������
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    isRetreating = false;
                    FindCover();
                }
            }
            else if (isInCover() && Time.time > nextFireTime && HasLineOfSight())
            {
                // ���� ���� � ������� � ����� ������, ��������
                LookAtPlayer();
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    // ���������, ��������� �� ���� � �������.
    private bool isInCover()
    {
        return currentCover != null && !isRetreating;
    }

    // ������������ ����� � ������� ������.
    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // ������� ������ �����/����
        transform.rotation = Quaternion.LookRotation(direction);
    }

    // �������� � ������ � �������� �����������.
    private void Fire()
    {
        if (bulletPrefab == null || bulletSpawn == null) return;

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

    // ������������ ����������� �������� � ������ ����������.
    private Vector3 CalculateInaccurateDirection()
    {
        Vector3 direction = (player.position - bulletSpawn.position).normalized;
        direction.x += Random.Range(-inaccuracy, inaccuracy);
        direction.y += Random.Range(-inaccuracy, inaccuracy);
        return direction.normalized;
    }

    // ��������� �� �������� ������� � ������ �� �������� ����������.
    private void RetreatFromCover()
    {
        isRetreating = true;

        // ������������ ����������� �����������
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        Vector3 retreatTarget = transform.position + retreatDirection * retreatDistance;

        // ���������, �������� �� ������� �� NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(retreatTarget, out hit, retreatDistance, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
    }

    // ���� ��������� ������� ����� �����������.
    private void FindCover()
    {
        Collider[] covers = Physics.OverlapSphere(transform.position, detectionRange, coverLayer);

        Transform bestCover = null;
        float bestScore = Mathf.Infinity;

        foreach (Collider cover in covers)
        {
            if (cover.transform == lastUsedCover)
            {
                continue;
            }

            Vector3 coverPosition = cover.transform.position;
            Vector3 directionToPlayer = (player.position - coverPosition).normalized;
            Vector3 coverOffset = coverPosition - directionToPlayer * 1.5f;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(coverOffset, out navHit, 2.0f, NavMesh.AllAreas))
            {
                float distanceToCover = Vector3.Distance(transform.position, navHit.position);
                if (distanceToCover < bestScore)
                {
                    bestCover = cover.transform;
                    bestScore = distanceToCover;
                }
            }
        }

        if (bestCover != null)
        {
            Debug.Log($"����� �������: {bestCover.name}");
            lastUsedCover = currentCover; // ���������� ������� ������� ��� "��������� ��������������"
            currentCover = bestCover;

            Vector3 directionToPlayer = (player.position - currentCover.position).normalized;
            Vector3 targetPosition = currentCover.position - directionToPlayer * 1.5f;

            agent.isStopped = false;
            agent.SetDestination(targetPosition);
        }
    }

    // ���������, ���� �� � ����� ������ ��������� ������.
    private bool HasLineOfSight()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (!Physics.Raycast(transform.position, directionToPlayer, detectionRange, obstacleLayer))
        {
            return true;
        }

        return false;
    }
}