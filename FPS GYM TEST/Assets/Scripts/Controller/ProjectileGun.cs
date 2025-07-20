using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controller
{
    public class ProjectileGun : MonoBehaviour
    {
        #region Fields
        
        [Header("Parameters")]
        
        [SerializeField] private float shootForce,upward;

        [SerializeField] private float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
        [SerializeField] private int magazineSize, bulletPerTap;
        [SerializeField] private bool allowButtonHold;

        private int bulletLeft, bulletsShot;

        private bool shooting, readyToShoot, reloading;

        [SerializeField] private Camera fpsCam;
        [SerializeField] private Transform attackPoint;

        [SerializeField] private bool allowInvoke = true;
        
        [Header("Graphics")]
        [SerializeField] private GameObject bullet;

        [SerializeField] private GameObject muzzleFlash;
        [SerializeField] private TMP_Text ammunitionDisplay;
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
            InitAwake();
        }

        private void Start()
        {
            fpsCam = Camera.main;
        }

        private void Update()
        {
            InputWeapon();
            DisplayText(ammunitionDisplay,$"{bulletLeft / bulletPerTap} / {magazineSize / bulletPerTap}");
        }

        #endregion

        #region Init

        private void InitAwake()
        {
            bulletLeft = magazineSize;
            readyToShoot = true;
        }

        #endregion

        #region Shoot Method

        private void InputWeapon()
        {
            if (allowButtonHold)
            {
                shooting = Input.GetKey(KeyCode.Mouse0);
            }
            else
            {
                shooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletLeft < magazineSize && !reloading)
            {
                Reloading();
            }

            if (readyToShoot && shooting && !reloading && bulletLeft <= 0)
            {
                Reloading();
            }
            
            if (readyToShoot && shooting && !reloading && bulletLeft > 0)
            {
                bulletsShot = 0;

                Shoot();
            }
        }

        private void Shoot()
        {
            readyToShoot = false;

            Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray,out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(75); //apparament juste un point loin du player étrange ???
            }

            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);

            Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);
            
            //TODO POOL SYSTEM

            GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = directionWithSpread.normalized;
            
            currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

            if (muzzleFlash != null)
            {
                Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            }
            
            bulletLeft--;
            bulletsShot++;

            if (allowInvoke)
            {
                Invoke(nameof(ResetShot),timeBetweenShots);
                allowInvoke = false;
            }
            
            //only if machine gun like 
            /*
             if (bulletsShot < bulletPerTap && bulletsShot > 0)
            {
                Invoke(nameof(Shoot),timeBetweenShots);
            }
             */

            
        }

        private void ResetShot()
        {
            readyToShoot = true;
            allowInvoke = true;
        }

        private void Reloading()
        {
            reloading = true;
            Invoke(nameof(ReloadingFinished),reloadTime);
        }

        private void ReloadingFinished()
        {
            bulletLeft = magazineSize;
            reloading = false;
        }
        
        #endregion
        
        #region UI

        private void DisplayText(TMP_Text _text, string _content)
        {
            _text.text = _content;
        }

        #endregion
    }
}
