using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class sniperShipScript : MonoBehaviour
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
    
    public float ammoCount;
    public float reloadTime;
    private float reloadTimer;
    private float maxAmmoCount;
    private bool reloading = false;

    public float ultimateReloadTime;
    private float ultimateReloadTimer;
    private bool canActivateUltimate;
    private bool ultActive;
    public float ultActiveTime;
    private float ultActiveTimer;
    public Color ultSRColor;
    public Color ultBulletColor;
    private float lerpTime;
    public string entryEffect;
    public string exitEffect;
    public float entryShakeMag;

    public string bulletPrefName;

    public Transform shootingArea;

    private invincibilityFrames iF;

    public bool healing;

    private float lerpTimeHealing;
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
        doHealingEffect();
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
            if (ultActive)
            {
                if(this.transform.GetChild(0).GetComponent<Camera>().orthographicSize < 26)
                {
                    //ppb.profile.motionBlur.enabled = true;
                    this.transform.GetChild(0).GetComponent<Camera>().orthographicSize += Time.deltaTime * Random.Range(75,100);
                }
            }
            else
            {
                if(this.transform.GetChild(0).GetComponent<Camera>().orthographicSize < 17)
                {
                    //ppb.profile.motionBlur.enabled = true;
                    this.transform.GetChild(0).GetComponent<Camera>().orthographicSize += Time.deltaTime * Random.Range(50,100);
                }
            }
            
        }
        if (!speeding)
        {
            if (ultActive)
            {
                if (this.transform.GetChild(0).GetComponent<Camera>().orthographicSize > 12)
                {
                    this.transform.GetChild(0).GetComponent<Camera>().orthographicSize -= Time.deltaTime * Random.Range(30, 40);
                }
            }
            else
            {
                if (this.transform.GetChild(0).GetComponent<Camera>().orthographicSize > 12)
                {
                    this.transform.GetChild(0).GetComponent<Camera>().orthographicSize -= Time.deltaTime * Random.Range(10, 20);
                } 
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
                this.GetComponent<SpriteRenderer>().color = ultBulletColor; 
                PhotonNetwork.Instantiate(entryEffect, this.transform.position, this.transform.rotation, 0);
                StartCoroutine(this.transform.GetChild(0).gameObject.GetComponent<CamerShake>()
                    .Shake(entryShakeMag, 0.15f));
                lerpTime = 0;
                ultActive = true;
                canActivateUltimate = false;
            }
        }

        if (ultActive)
        {
            lerpTime += Time.deltaTime;

            PostProcessProfile ppF = this.transform.GetChild(0).GetComponent<PostProcessVolume>().profile;
            Vignette v = (Vignette)ppF.settings[0];
            
            v.intensity.value = Mathf.Lerp(0f, 0.45f, lerpTime * 4);
            ChromaticAberration c = (ChromaticAberration) ppF.settings[1];
            c.intensity.value = Mathf.Lerp(0f, 0.27f, lerpTime * 4);
            this.GetComponent<PhotonView>().RPC("setTrailColorUlt", PhotonTargets.All);
            ultActiveTimer += Time.deltaTime;
            if (ultActiveTimer >= ultActiveTime)
            {
                PhotonNetwork.Instantiate(exitEffect, this.transform.position, this.transform.rotation, 0);
                StartCoroutine(this.transform.GetChild(0).gameObject.GetComponent<CamerShake>()
                    .Shake(entryShakeMag, 0.15f));
                ultActive = false;
                ultimateReloadTimer = 0;
                ultActiveTimer = 0;
                lerpTime = 0;
                canActivateUltimate = false;
            }
        }
        else
        {
            lerpTime += Time.deltaTime;
            PostProcessProfile ppF = this.transform.GetChild(0).GetComponent<PostProcessVolume>().profile;
            Vignette v = (Vignette)ppF.settings[0];
            this.GetComponent<SpriteRenderer>().color = Color.white;
            if (healing == false)
            {
                v.intensity.value = Mathf.Lerp(0.45f,0, lerpTime * 0.75f);
            }
            
            ChromaticAberration c = (ChromaticAberration) ppF.settings[1];
            c.intensity.value = Mathf.Lerp(0.27f, 0f, lerpTime * 0.75f);
            this.GetComponent<PhotonView>().RPC("setTrailColorRed", PhotonTargets.All);
        }
        
        
    }
    

    [PunRPC]
    void setTrailColorRed()
    {
        Gradient g = new Gradient();
        GradientColorKey[] gradientColorKeys = new GradientColorKey[1];
        GradientColorKey gC = new GradientColorKey();
        gC.color = new Color(0.858f, 0.149f, 0.058f, 1);
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
        gC.color = ultBulletColor;
        gradientColorKeys[0] = gC;
        g.colorKeys = gradientColorKeys;
        this.gameObject.GetComponent<TrailRenderer>().colorGradient = g;
    }

    
    
    
    void ShipRotation(){
        Vector3 mousePos = this.transform.GetChild(0).GetComponent<Camera>().ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        float xdis = mousePos.x - this.transform.position.x;
        float ydis = mousePos.y - this.transform.position.y;
        
        directionFacing = new Vector2(xdis, ydis).normalized;
        float angle = Mathf.Atan2(ydis, xdis);

        
        this.transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg - 90);
        
		

			
    }
    
    void doHealingEffect()
    {
        if (healing)
        {
            lerpTimeHealing += Time.deltaTime;
            PostProcessProfile ppF = this.transform.GetChild(0).GetComponent<PostProcessVolume>().profile;
            Vignette v = (Vignette) ppF.settings[0];
            Bloom b = (Bloom) ppF.settings[2];
            v.color.value = Color.yellow;
            b.color.value = Color.yellow;
            v.intensity.value = Mathf.Lerp(0, 0.38f, lerpTimeHealing);
            b.intensity.value = Mathf.Lerp(0, 20f, lerpTimeHealing);
            if (lerpTimeHealing >= 1)
            {
                lerpTimeHealing = 0;
                this.GetComponent<PhotonView>().RPC("setHealingFalse", PhotonTargets.All);
            }
        }
        else
        {
            lerpTimeHealing += Time.deltaTime;
            PostProcessProfile ppF = this.transform.GetChild(0).GetComponent<PostProcessVolume>().profile;
            Vignette v = (Vignette) ppF.settings[0];
            Bloom b = (Bloom) ppF.settings[2];
            v.color.value = Color.Lerp(Color.yellow, Color.black, lerpTimeHealing);
            b.color.value = Color.Lerp(Color.yellow, Color.black, lerpTimeHealing);
            v.intensity.value = Mathf.Lerp(0.38f, 0, lerpTimeHealing);
            b.intensity.value = Mathf.Lerp(20f, 0f, lerpTimeHealing);
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
                    PhotonNetwork.Instantiate(bulletPrefName, shootingArea.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                bullet.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                bulletScript bScript = bullet.GetComponent<bulletScript> ();
                if (ultActive)
                {
                    bScript.damage = Mathf.Infinity;
                    bullet.GetComponent<PhotonView>().RPC("setBulletColor", PhotonTargets.All, ultBulletColor.r, ultBulletColor.g, ultBulletColor.b, ultBulletColor.a);
                    bScript.shakeMagnitude *= 2.5f;
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
