using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class minigunScript : MonoBehaviour
{
    public bool speeding;
    private float speed;
    public bool shooting;
    private float shootTime;
    public float fireRate;
    private Vector2 directionFacing;
    public float lives = 10;
    private Rigidbody2D rb;
    public float maxLives;
    
    public float ultimateReloadTime;
    private float ultimateReloadTimer;
    private bool canActivateUltimate;
    private bool ultActive;
    public float ultActiveTime;
    private float ultActiveTimer;
    private float lerpTime;
    public string enterUltEffect;
    public string exitUltEffect;
    
    
    public float ammoCount;
    public float reloadTime;
    private float reloadTimer;
    private float maxAmmoCount;
    private bool reloading = false;

    public string bulletPrefName;

    private invincibilityFrames iF;

    public Transform shootingArea;

    public bool healing;
    // Start is called before the first frame update
    void Start()
    {
        iF = this.GetComponent<invincibilityFrames>();
        maxAmmoCount = ammoCount;
        lives = maxLives;
        rb = this.GetComponent<Rigidbody2D>();
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
            if(this.transform.GetChild(0).GetComponent<Camera>().orthographicSize < 17)
            {
                //ppb.profile.motionBlur.enabled = true;
                this.transform.GetChild(0).GetComponent<Camera>().orthographicSize += Time.deltaTime * Random.Range(50,100);
            }
        }
        if (!speeding)
        {
            if (this.transform.GetChild(0).GetComponent<Camera>().orthographicSize > 12)
            {
                this.transform.GetChild(0).GetComponent<Camera>().orthographicSize -= Time.deltaTime * Random.Range(10, 20);
            }
        }
        if (lives <= 0.1)
        {
            ultActive = false;
            ultimateReloadTimer = 0;
            canActivateUltimate = false;
            GameObject.Find("teamScoreManager").GetComponent<PhotonView>().RPC("addScore", PhotonTargets.All, 1f, this.gameObject.GetComponent<teamManager>().team);
            this.GetComponent<PhotonView>().RPC("setInvincible", PhotonTargets.All, true);
            PhotonNetwork.Instantiate("bigExplosion", this.transform.position, this.transform.rotation, 0);
            this.transform.position = this.gameObject.GetComponent<respawnPosSave>().respawnPos;
            this.GetComponent<PhotonView>().RPC("punResetLife", PhotonTargets.All, maxLives);
            rb.velocity = Vector2.zero;
        }
        shootTime += Time.deltaTime;
        ShipRotation();
        Ultimate();
        this.transform.GetChild(0).rotation = Quaternion.identity;
        this.transform.GetChild(0).position = new Vector3(this.transform.position.x, this.transform.position.y, -30);
        shipMovement();
        if (ammoCount > 0 && reloading == false)
        {
            Shoot();
        }
        
        
        Reload();
    }
    
        [PunRPC]
        void setHealingTrue()
        {
            healing = true;
        }
    
        [PunRPC]
        void setHealingFalse()
        {
            healing = false;
        }
    
    void ShipRotation(){
        Vector3 mousePos = this.transform.GetChild(0).GetComponent<Camera>().ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        float xdis = mousePos.x - this.transform.position.x;
        float ydis = mousePos.y - this.transform.position.y;
        
        directionFacing = new Vector2(xdis, ydis).normalized;
        float angle = Mathf.Atan2(ydis, xdis);

        
        this.transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg);
        
		

			
    }
    
    [PunRPC]
    void setTrailColorBlue()
    {
        Gradient g = new Gradient();
        GradientColorKey[] gradientColorKeys = new GradientColorKey[1];
        GradientColorKey gC = new GradientColorKey();
        gC.color = new Color(15f/255f, 195f/255f, 219f/255f, 1);
        gradientColorKeys[0] = gC;
        g.colorKeys = gradientColorKeys;
        this.gameObject.GetComponent<TrailRenderer>().colorGradient = g;
    }

    [PunRPC]
    void setTrailColorUlt()
    {
        Gradient g = new Gradient();
        GradientColorKey[] gradientColorKeys = new GradientColorKey[1];
        GradientColorKey gC = new GradientColorKey();
        gC.color = new Color(0.235f, 0.192f, 70f/255f, 1f);
        gradientColorKeys[0] = gC;
        g.colorKeys = gradientColorKeys;
        this.gameObject.GetComponent<TrailRenderer>().colorGradient = g;
    }

    void Ultimate()
    {
        if (ultimateReloadTimer < ultimateReloadTime)
        {
            ultimateReloadTimer += Time.deltaTime;
            canActivateUltimate = false;
        }
        else
        {
            ultimateReloadTimer = ultimateReloadTime;
            canActivateUltimate = true;
        }

        if (canActivateUltimate)
        {
            if (Input.GetKeyDown(KeyCode.Space) && ultActive == false)
            {
                this.GetComponent<PhotonView>().RPC("setTrailColorUlt", PhotonTargets.All);
                StartCoroutine(this.transform.GetChild(0).gameObject.GetComponent<CamerShake>().Shake(0.2f, 0.6f));
                PhotonNetwork.Instantiate(enterUltEffect, this.transform.position, Quaternion.identity, 0);
                lerpTime = 0;
                ultActive = true;
                canActivateUltimate = false;
            }
        }

        if (ultActive)
        {
            lerpTime += Time.deltaTime;
            PostProcessProfile ppF = this.transform.GetChild(0).GetComponent<PostProcessVolume>().profile;
            Vignette v = (Vignette) ppF.settings[0];
            Bloom b = (Bloom) ppF.settings[1];
            v.intensity.value = Mathf.Lerp(0, 0.32f, lerpTime);
            b.intensity.value = Mathf.Lerp(0, 5f, lerpTime);
            ultActiveTimer += Time.deltaTime;
            if (ultActiveTimer >= ultActiveTime)
            {
                StartCoroutine(this.transform.GetChild(0).gameObject.GetComponent<CamerShake>().Shake(0.2f, 0.6f));
                PhotonNetwork.Instantiate(exitUltEffect, this.transform.position, Quaternion.identity, 0);
                lerpTime = 0;
                
                ultActive = false;
                ultimateReloadTimer = 0;
                ultActiveTimer = 0;
                canActivateUltimate = false;
            }
        }
        else
        {
            this.GetComponent<PhotonView>().RPC("setTrailColorBlue", PhotonTargets.All);
            lerpTime += Time.deltaTime;
            PostProcessProfile ppF = this.transform.GetChild(0).GetComponent<PostProcessVolume>().profile;
            Vignette v = (Vignette) ppF.settings[0];
            Bloom b = (Bloom) ppF.settings[1];
            v.intensity.value = Mathf.Lerp(0.32f, 0f, lerpTime);
            b.intensity.value = Mathf.Lerp(5f, 0f, lerpTime);
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
                GameObject bullet;
                if (ultActive)
                {
                    bullet =
                        PhotonNetwork.Instantiate("minigunBulletNoRecoil", shootingArea.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                    bullet.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                    bulletScript bScript = bullet.GetComponent<bulletScript> ();
                }
                else
                {
                    bullet =
                        PhotonNetwork.Instantiate(bulletPrefName, shootingArea.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                    bullet.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                    bulletScript bScript = bullet.GetComponent<bulletScript> ();
                }
                
                
                //bScript.origShooter = this.GetComponent<Collider2D>();
                //bScript.direction = new Vector2 (directionFacing.x, directionFacing.y);
                bullet.gameObject.GetComponent<PhotonView>().RPC("setDirection", PhotonTargets.All, new Vector2(directionFacing.x, directionFacing.y));

                shootTime = 0;
                ammoCount -= 1;
            }
        }
    }
    
    void Reload()
    {
        if (ammoCount <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            reloading = true;
            if (ultActive)
            {
                shooting = true;
            }
            else
            {
                shooting = false; 
            }
            
        }

        if (reloading)
        {
            reloadTimer += Time.deltaTime;
            if (ultActive == false)
            {
                if (reloadTimer >= reloadTime)
                {
                    reloadTimer = 0;
                    ammoCount = maxAmmoCount;
                    reloading = false;
                }
            }
            else
            {
                reloadTimer = 0;
                ammoCount = maxAmmoCount;
                reloading = false;
            }
            
        }
    }
    
    
    
    void shipMovement(){
        if (speeding) {
            
                speed = 400;
           
			
        }
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            speed = 400;
            speeding = true;
        }
        if (Input.GetKeyUp (KeyCode.LeftShift)) {
            speed = 100;
            
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
    
    [PunRPC]
    void resetMaxLives()
    {
        lives = maxLives;
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
