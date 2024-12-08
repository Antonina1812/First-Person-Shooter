using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private bool isShooting, readyToShoot;
    private bool allowReset = true;
    [SerializeField] private float shootingDelay = 2f;

    [SerializeField] private int bulletsPerBurst = 3;
    private int burstBulletsLeft;

    [SerializeField] private float spreadIntensity;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletVelocity = 100;
    [SerializeField] private float bulletPrefabLifeTime = 3f;

    [Header("Recoil Settings")]
    [SerializeField] private Vector3 recoilPower = new Vector3(2f, 1f, 0f); // Мощность отдачи
    [SerializeField] private float recoilReturnSpeed = 5f; // Скорость возврата
    [SerializeField] private float recoilSnap = 10f; // Скорость "рывка" к отдаче
    private Vector3 targetRecoil;
    private Vector3 currentRecoil;

    private WeaponSoundManager weaponSoundManager;

    [SerializeField]
    private enum ShootingMode
    {
        Burst,
        Auto
    }

    [SerializeField] private ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        weaponSoundManager = GetComponentInChildren<WeaponSoundManager>();
    }

    void Update()
    {
        // Обработка отдачи
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        currentRecoil = Vector3.Slerp(currentRecoil, targetRecoil, recoilSnap * Time.deltaTime);
        Camera.main.transform.localRotation = Quaternion.Euler(currentRecoil);

        // Логика стрельбы
        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        readyToShoot = false;

        if (weaponSoundManager != null)
        {
            weaponSoundManager.PlayShootSound();
        }

        // Реализация отдачи
        ApplyRecoil();

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        bullet.transform.forward = shootingDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }


    private void ApplyRecoil()
    {
        targetRecoil += new Vector3(
            -recoilPower.x, // Отдача вверх по оси X (по инверсии камеры)
            Random.Range(-recoilPower.y, recoilPower.y), // Случайная отдача по оси Y
            0 // Z обычно не влияет на камеру
        );
    }


    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
