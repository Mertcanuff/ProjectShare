using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GunSystem : MonoBehaviour
{
    public float aimFOV;
    public Animator animReload;
    
    //BulletHole
    public GameObject bulletHole;
    public float distance = 10f;

    public Movement movement;
    public GameObject weapon;
    private int _currentAmmoInClip;
    public Image Crosshair1;
    public float FOVChangeSpeed;
    public float aimSmoothing = 10;
    
    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, spreadAfterAim, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools 
    bool shooting, readyToShoot, reloading;

    //Aiming
    public Vector3 normalLocalPosition;
    public Vector3 aimingLocalPosition;

    //Muzzleflash
    public Image muzzleFlashImage;
    public Sprite[] flashes;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    //Graphics
    public GameObject bulletHoleGraphic;
  
    public TextMeshProUGUI text;

    private void Awake()
    {
        _currentAmmoInClip = magazineSize;
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    void Start()
    {
        
    }
    private void Update()
    {
        MyInput();
        DetermineAim();

        //SetText
        text.SetText(bulletsLeft + " / " + magazineSize);
        
        
    }


    private void DetermineAim()
    {
        Vector3 target = normalLocalPosition;
        if (Input.GetMouseButton(1))
        {
            Crosshair1.enabled = false;
            spread /= 4;
            target = aimingLocalPosition;
            movement.mouseSensitivity = movement.mouseSensitivityInAim;
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, movement.aimFOV, movement.FOVChangeSpeed);
            Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmoothing);
            transform.localPosition = desiredPosition;
        }
        else{
            Crosshair1.enabled = true;
            spread = spreadAfterAim;
            movement.mouseSensitivity = movement.mouseSensitivityAfterAim;
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, movement.normalFOV, FOVChangeSpeed);
            Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmoothing);
            transform.localPosition = desiredPosition;
        }
            

        
        
    }
    private void MyInput()
    {
        Vector3 target = normalLocalPosition;

        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        } 
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        } 

        if (Input.GetKey(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
            animReload.SetBool("reload", true);
        } 

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();

            transform.localPosition -= Vector3.forward * 0.1f;
            
            
        }
    }
   
    private void Shoot()
    {
        readyToShoot = false;
        StartCoroutine(MuzzleFlash());

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);
            if((rayHit.collider.CompareTag("Player")))
            {
                Debug.Log("DAMAGEEEE");
                Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 0, 0));
            }
        }

        //Graphics
        RaycastHit hit;
            if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit , distance))
            {
                GameObject bH = Instantiate(bulletHole, hit.point + new Vector3(0f, 0f, -.02f), Quaternion.LookRotation(-hit.normal));
                bH.transform.position += bH.transform.forward / -1000;
                Destroy(bH, 0.5f);
            }
        

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if(bulletsShot > 0 && bulletsLeft > 0)
        Invoke("Shoot", timeBetweenShots);
    }
    private void ResetShot()
    {
        readyToShoot = true;
    }
    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        animReload.SetBool("reload", false);
    }

    private IEnumerator MuzzleFlash()
    {
        muzzleFlashImage.sprite = flashes[Random.Range(0, flashes.Length)];
        muzzleFlashImage.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        muzzleFlashImage.sprite = null;
        muzzleFlashImage.color = new Color(0, 0, 0, 0);
    }

   
}
