using UnityEngine;
using UnityEngine.AI;

public class HidingEnemy : Enemy
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float safeDistance = 5f;
    [SerializeField] private float inaccuracy = 0.25f; // Отклонение пуль
    [SerializeField] private LayerMask coverLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float moveSpeed = 7f; // Скорость движения

    private NavMeshAgent agent;
    private float nextFireTime;
    private Transform currentCover;
    private WeaponSoundManager weaponSoundManager;
    private bool isInCover;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed; // Устанавливаем скорость из параметра
        agent.acceleration = 50f; // Быстрый разгон
        agent.angularSpeed = 720f; // Быстрые повороты

        weaponSoundManager = GetComponentInChildren<WeaponSoundManager>();
        if (weaponSoundManager == null)
        {
            Debug.LogError("WeaponSoundManager не найден у врага " + gameObject.name);
        }

        FindCover();
    }


    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= safeDistance)
            {
                FindCover(); // Пытаемся сменить укрытие или отступить
            }
            else if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                // Враг в движении, ничего не делаем
            }
            else if (isInCover && Time.time > nextFireTime && HasLineOfSight())
            {
                // Если враг в укрытии, атакуем игрока
                LookAtPlayer();
                Fire();
                nextFireTime = Time.time + fireRate;
            }
            else
            {
                // Если враг стоит на месте, наблюдаем за игроком и атакуем
                LookAtPlayer();
                if (Time.time > nextFireTime && HasLineOfSight())
                {
                    Fire();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Убираем наклон вверх/вниз
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void Fire()
    {
        if (bulletPrefab == null || bulletSpawn == null) return;

        if (weaponSoundManager != null)
        {
            weaponSoundManager.PlayShootSound();
        }

        Vector3 shootingDirection = CalculateInaccurateDirection();
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
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

    private void FindCover()
    {
        Collider[] covers = Physics.OverlapSphere(transform.position, detectionRange, coverLayer);

        Transform bestCover = null;
        float bestScore = Mathf.Infinity;

        foreach (Collider cover in covers)
        {
            Vector3 coverToPlayer = player.position - cover.transform.position;
            Vector3 coverDirection = coverToPlayer.normalized;

            if (Physics.Raycast(cover.transform.position, coverDirection, detectionRange, obstacleLayer))
            {
                float distanceToCover = Vector3.Distance(transform.position, cover.transform.position);
                if (distanceToCover < bestScore)
                {
                    bestCover = cover.transform;
                    bestScore = distanceToCover;
                }
            }
        }

        if (bestCover != null)
        {
            currentCover = bestCover;
            agent.isStopped = false;
            Vector3 coverPosition = currentCover.position;
            Vector3 offset = (coverPosition - player.position).normalized * 1.5f;
            agent.SetDestination(coverPosition + offset);
            isInCover = false;
        }
        else
        {
            RetreatFromPlayer();
        }
    }

    private void RetreatFromPlayer()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 retreatPosition = transform.position + directionAwayFromPlayer * safeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(retreatPosition, out hit, safeDistance, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
            isInCover = false;
        }
        else
        {
            Debug.LogWarning("Не удалось найти точку отступления, враг останется на месте.");
        }
    }

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
