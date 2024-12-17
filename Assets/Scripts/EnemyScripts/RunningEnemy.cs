using UnityEngine;
using UnityEngine.AI;

public class RunningEnemy : Enemy
{
    [SerializeField] private float moveSpeed = 7f; // Скорость врага
    private Transform player; // Для координат игрока
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float fireRate = 1.5f; // Задержка перед выстрелом
    [SerializeField] private float stopDistance = 12f; // Дистанция остановки от игрока
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] private int bulletDamage = 20;
    [SerializeField] private float inaccuracy = 0.1f; // Отклонение пули относительно прямой к игроку
    private WeaponSoundManager weaponSoundManager;
    [SerializeField] private float viewAngle = 120f; // Угол зрения врага
    [SerializeField] private float viewDistance = 30f; // Максимальная дистанция видимости
    private float nextFireTime;
    [SerializeField] private LayerMask targetMask; // Маска для игрока
    [SerializeField] private LayerMask obstacleMask; // Маска для препятствий

    private NavMeshAgent agent; // NavMeshAgent для навигации
    private bool canSeePlayer = false; // Флаг видимости игрока

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        nextFireTime = Time.time + Random.Range(0f, 1f); // Случайная задержка для рассинхронизации врагов

        weaponSoundManager = GetComponentInChildren<WeaponSoundManager>();
        if (weaponSoundManager == null)
        {
            Debug.LogError("WeaponSoundManager не найден у врага " + gameObject.name);
        }

        bulletDamage = EnemyStats.RunningEnemyBaseDamage;
        HP = EnemyStats.BaseHP;

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent не найден у врага " + gameObject.name);
        }
        else
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stopDistance;
        }
    }

    void Update()
    {
        // Проверяем, видим ли игрока
        canSeePlayer = CanSeePlayer();

        // Движение и слежение
        MoveAndTrackPlayer();

        // Стрельба
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
                // Игрок в поле зрения, но далеко - движемся к нему
                agent.SetDestination(player.position);
            }
            else
            {
                // Игрок в пределах дистанции остановки
                agent.ResetPath();
            }

            // Поворачиваемся к игроку
            LookAtPlayer();
        }
        else
        {
            // Если игрок вне поля зрения, продолжаем двигаться в его направлении
            agent.SetDestination(player.position);
        }
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Убираем наклон вверх/вниз
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

        // Проверка угла и дистанции
        if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2 && distanceToPlayer < viewDistance)
        {
            // Проверка препятствий
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, viewDistance, obstacleMask | targetMask))
            {
                return hit.transform.CompareTag("Player");
            }
        }

        return false;
    }
}
