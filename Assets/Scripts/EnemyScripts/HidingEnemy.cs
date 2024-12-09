using UnityEngine;
using UnityEngine.AI;

public class HidingEnemy : Enemy
{
    [Header("Основные параметры")]
    [SerializeField] private Transform player; // Ссылка на игрока
    [SerializeField] private GameObject bulletPrefab; // Префаб пули
    [SerializeField] private Transform bulletSpawn; // Точка появления пули
    [SerializeField] private float moveSpeed = 7f; // Скорость движения
    [SerializeField] private float detectionRange = 20f; // Радиус обнаружения игрока
    [SerializeField] private float safeDistance = 5f; // Дистанция, на которой враг будет искать укрытие
    [SerializeField] private float retreatDistance = 3f; // Дистанция для первоначального отбегания

    [Header("Стрельба")]
    [SerializeField] private float fireRate = 1.5f; // Скорость стрельбы
    [SerializeField] private float bulletSpeed = 100f; // Скорость пули
    [SerializeField] private float inaccuracy = 0.25f; // Разброс пуль

    [Header("Укрытия")]
    [SerializeField] private LayerMask coverLayer; // Слой укрытий
    [SerializeField] private LayerMask obstacleLayer; // Слой препятствий

    private NavMeshAgent agent;
    private float nextFireTime;
    private Transform currentCover; // Текущее укрытие
    private Transform lastUsedCover; // Последнее укрытие
    private bool isRetreating; // Флаг, обозначающий, что враг отступает
    private WeaponSoundManager weaponSoundManager;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.acceleration = 50f;
        agent.angularSpeed = 720f;

        weaponSoundManager = GetComponentInChildren<WeaponSoundManager>();
        if (weaponSoundManager == null)
        {
            Debug.LogError("WeaponSoundManager не найден у врага " + gameObject.name);
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
                // Игрок слишком близко - враг сначала отступает
                RetreatFromCover();
            }
            else if (isRetreating)
            {
                // Проверяем, завершилось ли отступление
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    isRetreating = false;
                    FindCover();
                }
            }
            else if (isInCover() && Time.time > nextFireTime && HasLineOfSight())
            {
                // Если враг в укрытии и видит игрока, стреляет
                LookAtPlayer();
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    /// <summary>
    /// Проверяет, находится ли враг в укрытии.
    /// </summary>
    private bool isInCover()
    {
        return currentCover != null && !isRetreating;
    }

    /// <summary>
    /// Поворачивает врага в сторону игрока.
    /// </summary>
    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Убираем наклон вверх/вниз
        transform.rotation = Quaternion.LookRotation(direction);
    }

    /// <summary>
    /// Стреляет в игрока с заданной неточностью.
    /// </summary>
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

    /// <summary>
    /// Рассчитывает направление выстрела с учетом неточности.
    /// </summary>
    private Vector3 CalculateInaccurateDirection()
    {
        Vector3 direction = (player.position - bulletSpawn.position).normalized;
        direction.x += Random.Range(-inaccuracy, inaccuracy);
        direction.y += Random.Range(-inaccuracy, inaccuracy);
        return direction.normalized;
    }

    /// <summary>
    /// Отступает от текущего укрытия и игрока на заданное расстояние.
    /// </summary>
    private void RetreatFromCover()
    {
        isRetreating = true;

        // Рассчитываем направление отступления
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        Vector3 retreatTarget = transform.position + retreatDirection * retreatDistance;

        // Проверяем, доступна ли позиция на NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(retreatTarget, out hit, retreatDistance, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning("Не удалось найти точку для отступления, враг останется на месте.");
        }
    }

    /// <summary>
    /// Ищет ближайшее укрытие после отступления.
    /// </summary>
    private void FindCover()
    {
        Collider[] covers = Physics.OverlapSphere(transform.position, detectionRange, coverLayer);
        Debug.Log($"Найдено укрытий: {covers.Length}");

        Transform bestCover = null;
        float bestScore = Mathf.Infinity;

        foreach (Collider cover in covers)
        {
            if (cover.transform == lastUsedCover)
            {
                Debug.Log($"Укрытие {cover.name} пропущено (это последнее использованное укрытие).");
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
            Debug.Log($"Нашли укрытие: {bestCover.name}");
            lastUsedCover = currentCover; // Запоминаем текущее укрытие как "последнее использованное"
            currentCover = bestCover;

            Vector3 directionToPlayer = (player.position - currentCover.position).normalized;
            Vector3 targetPosition = currentCover.position - directionToPlayer * 1.5f;

            agent.isStopped = false;
            agent.SetDestination(targetPosition);
        }
        else
        {
            Debug.LogWarning("Укрытие не найдено, враг будет продолжать двигаться.");
        }
    }

    /// <summary>
    /// Проверяет, есть ли у врага прямая видимость игрока.
    /// </summary>
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
