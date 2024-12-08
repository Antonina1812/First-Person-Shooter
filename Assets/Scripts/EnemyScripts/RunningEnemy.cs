using UnityEngine;

public class RunningEnemy : Enemy
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float stopDistance = 10f; // Дистанция остановки от игрока
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float inaccuracy = 1f;
    private WeaponSoundManager weaponSoundManager;
    private float nextFireTime;

    void Start()
    {
        nextFireTime = Time.time + Random.Range(0f, 1f); // Случайная задержка для рассинхронизации врагов
        weaponSoundManager = GetComponentInChildren<WeaponSoundManager>();
        if (weaponSoundManager == null)
        {
            Debug.LogError("WeaponSoundManager не найден у врага " + gameObject.name);
        }
    }

    void Update()
    {
        // Движение
        MoveTowardsPlayer();

        // Стрельба
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

            // Поворачиваем врага к игроку
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else
        {
            // Останавливаемся и смотрим на игрока
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }

private void Fire()
{
    // Проигрываем звук стрельбы
    if (weaponSoundManager != null)
    {
        weaponSoundManager.PlayShootSound();
    }

    // Создаём пулю
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
}
