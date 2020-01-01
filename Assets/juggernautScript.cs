using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class juggernautScript : MonoBehaviour
{
    public bool speeding;
    private float speed;
    public bool shooting;
    private float shootTime;
    public float fireRate;
    private Vector2 directionFacing;
    public float lives = 10;
    public float maxLives;
    private Rigidbody2D rb;
    public float baseSpeed;

    public string bulletPrefNameSecondary;
    public float angleSpread;
    public float shotCount;
    public float fireRateSecondary;
    private bool shootingSecondary;
    public float ammoLossSecondary;
    private float shootTimeSecondary;

    public float ammoCount;
    public float reloadTime;
    private float reloadTimer;
    private float maxAmmoCount;
    private bool reloading = false;
    public Transform shooting1;
    public Transform shooting2;

    public string bulletPrefName;

    private invincibilityFrames iF;
    // Start is called before the first frame update
    void Start()
    {
        maxAmmoCount = ammoCount;
        rb = this.GetComponent<Rigidbody2D>();
        lives = maxLives;
        iF = this.GetComponent<invincibilityFrames>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((bool) PhotonNetwork.room.CustomProperties["gameRunning"] == false)
        {
            return;
        }
        if (lives >= maxLives)
        {
            lives = maxLives;
        }
        if (speeding)
        {
            if(this.transform.GetChild(0).GetComponent<Camera>().orthographicSize < 22)
            {
                //ppb.profile.motionBlur.enabled = true;
                this.transform.GetChild(0).GetComponent<Camera>().orthographicSize += Time.deltaTime * Random.Range(50,100);
            }
        }
        if (!speeding)
        {
            if (this.transform.GetChild(0).GetComponent<Camera>().orthographicSize > 17)
            {
                this.transform.GetChild(0).GetComponent<Camera>().orthographicSize -= Time.deltaTime * Random.Range(10, 20);
            }
        }
        if (lives <= 0.1)
        {
            GameObject.Find("teamScoreManager").GetComponent<PhotonView>().RPC("addScore", PhotonTargets.All, 1f, this.gameObject.GetComponent<teamManager>().team);
            GameObject.Find("teamScoreManager").GetComponent<PhotonView>().RPC("setJuggernautDead", PhotonTargets.All, 1);
            this.GetComponent<PhotonView>().RPC("setInvincible", PhotonTargets.All, true);
            PhotonNetwork.Instantiate("bigExplosion", this.transform.position, this.transform.rotation, 0);
            this.transform.position = this.gameObject.GetComponent<respawnPosSave>().respawnPos;
            this.GetComponent<PhotonView>().RPC("punResetLife", PhotonTargets.All, maxLives);
            rb.velocity = Vector2.zero;
        }
        shootTime += Time.deltaTime;
        shootTimeSecondary += Time.deltaTime;
        ShipRotation();
        this.transform.GetChild(0).rotation = Quaternion.identity;
        this.transform.GetChild(0).position = new Vector3(this.transform.position.x, this.transform.position.y, -30);
        shipMovement();
        if (ammoCount > 0 && reloading == false) 
        {
            Secondary();
            Shoot();
        }
        
        
        Reload();
    }
    
    void ShipRotation(){
        Vector3 mousePos = this.transform.GetChild(0).GetComponent<Camera>().ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        float xdis = mousePos.x - this.transform.position.x;
        float ydis = mousePos.y - this.transform.position.y;
        
        directionFacing = new Vector2(xdis, ydis).normalized;
        float angle = Mathf.Atan2(ydis, xdis);

        
        this.transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg - 90);
        
		

			
    }
    
    void Reload()
    {
        if (ammoCount <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            reloading = true;
            shooting = false;
        }

        if (reloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= reloadTime)
            {
                reloadTimer = 0;
                ammoCount = maxAmmoCount;
                reloading = false;
            }
        }
    }

    void Secondary()
    {
        if (Input.GetMouseButtonDown (1)) {
            shootingSecondary = true;
        }
        if (Input.GetMouseButtonUp (1)) {
            shootingSecondary = false;
        }

        if (shootingSecondary == true) {
            if (shootTimeSecondary > 1 / fireRateSecondary)
            {
                float angle = Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg;
                float angleStart = angle + angleSpread / 2;
                
                float angleEnd = angle - angleSpread / 2;
                float shotSpread = (angleSpread/2) / shotCount;
                for (var i = 0; i < shotCount; i++)
                {
                    float angleShotStart = angleStart - (shotSpread * (i));
                    float angleShotEnd = angleEnd + (shotSpread * (i));
                    GameObject bulletShotSide = PhotonNetwork.Instantiate(bulletPrefNameSecondary, this.transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                    bulletShotSide.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                    bulletScript bScriptShotSide = bulletShotSide.GetComponent<bulletScript> ();
                    bulletShotSide.gameObject.GetComponent<PhotonView>().RPC("setDirection", PhotonTargets.All, new Vector2(Mathf.Cos(angleShotStart * Mathf.Deg2Rad), Mathf.Sin(angleShotStart * Mathf.Deg2Rad)));
                    GameObject bulletEndSide = PhotonNetwork.Instantiate(bulletPrefNameSecondary, this.transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                    bulletEndSide.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                    bulletScript bScriptEndSide = bulletEndSide.GetComponent<bulletScript> ();
                    bulletEndSide.gameObject.GetComponent<PhotonView>().RPC("setDirection", PhotonTargets.All, new Vector2(Mathf.Cos(angleShotEnd * Mathf.Deg2Rad), Mathf.Sin(angleShotEnd * Mathf.Deg2Rad)));
                }
                GameObject bulletMiddle =
                    PhotonNetwork.Instantiate(bulletPrefNameSecondary, this.transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                bulletMiddle.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                bulletScript bScript = bulletMiddle.GetComponent<bulletScript> ();
                //bScript.origShooter = this.GetComponent<Collider2D>();
                //bScript.direction = new Vector2 (directionFacing.x, directionFacing.y);
                bulletMiddle.gameObject.GetComponent<PhotonView>().RPC("setDirection", PhotonTargets.All, new Vector2(directionFacing.x, directionFacing.y));

                shootTimeSecondary = 0;
                ammoCount -= ammoLossSecondary;
            }
        }
    }
    
    void Shoot(){
        if (Input.GetMouseButtonDown (0)) {
            shooting = true;
        }
        if (Input.GetMouseButtonUp (0)) {
            shooting = false;
        }

        if (shooting == true) {
            if (shootTime > 1 / fireRate)
            {
                GameObject bullet =
                    PhotonNetwork.Instantiate(bulletPrefName, shooting1.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                GameObject bullet2 = PhotonNetwork.Instantiate(bulletPrefName, shooting2.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                bullet.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                bulletScript bScript = bullet.GetComponent<bulletScript> ();
                bullet2.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                bulletScript bScript2 = bullet2.GetComponent<bulletScript> ();
                //bScript.origShooter = this.GetComponent<Collider2D>();
                //bScript.direction = new Vector2 (directionFacing.x, directionFacing.y);
                bullet2.gameObject.GetComponent<PhotonView>().RPC("setDirection", PhotonTargets.All, new Vector2(directionFacing.x, directionFacing.y));
                bullet.gameObject.GetComponent<PhotonView>().RPC("setDirection", PhotonTargets.All, new Vector2(directionFacing.x, directionFacing.y));

                shootTime = 0;
                ammoCount -= 1;
            }
        }
    }
    
    
    
    void shipMovement(){
        if (speeding) {
            
                speed = baseSpeed * 4;
           
			
        }
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            speed = baseSpeed * 4;
            speeding = true;
        }
        if (Input.GetKeyUp (KeyCode.LeftShift)) {
            speed = baseSpeed;
            
            speeding = false;
        }



        Vector2 moveDirection = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));



        /*if (speeding == false) {
			
			
            if (moveDirection.x == 0 && moveDirection.y == 0) {
                anim.SetFloat ("Speed", 0);
            } else {
                anim.SetFloat ("Speed", 2);
            }


        }*/

        Vector3 finalMove = moveDirection * speed * Time.deltaTime;
        rb.AddForce (finalMove);

    }
    
    public void removeLife()
    {
        this.GetComponent<PhotonView>().RPC("punRemoveLife", PhotonTargets.All, 0);
    }
    
    [PunRPC]
    void punRemoveLife(float amount)
    {
        lives -= amount;
    }

    [PunRPC]
    void punResetLife(float setAmount)
    {
        lives = setAmount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.GetComponent<PhotonView>().isMine)
        {
            /*if (other.gameObject.tag == "bullet")
            {
                if (other.gameObject.GetComponent<bulletScript>().origShooter != this.gameObject.GetComponent<Collider2D>())
                {
                    lives -= other.gameObject.GetComponent<bulletScript>().damage;
                    other.GetComponent<PhotonView>().RPC("destroyThis", PhotonTargets.All, 0);
                    
                    // this.GetComponent<PhotonView>().RPC("punRemoveLife", PhotonTargets.All,0);
                }
            }*/
        }
        
    }
}
