using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class assassinScript : MonoBehaviour
{
    public bool speeding;
    private float speed;
    //public float speedSpeeding;
    public float baseSpeed;
    public bool shooting;
    private float shootTime;
    public float fireRate;
    private Vector2 directionFacing;
    public float lives = 10;
    private Rigidbody2D rb;
    public float maxLives;
    
    public float ammoCount;
    public float angleSpread;
    public int shotCount;
    public float reloadTime;
    private float reloadTimer;
    private float maxAmmoCount;
    private bool reloading = false;
    
    public float ultimateReloadTime;
    public float ultimateReloadTimer;
    public bool canActivateUltimate;
    public bool ultActive;
    public float ultActiveTime;
    private float ultActiveTimer;
    public Color ultSRColor;
    private float lerpTime;
    
    private bool zooming;
    public float zoomTime;
    private float zoomTimer;
    public int zoomCopies;
    private int currentZoomCopies;
    public string zoomCopy;
    public int totalZoomCount;
    public int zoomCount;
    public float zoomReloadTime;
    private float zoomReloadTimer;
    private invincibilityFrames iF;

    public Transform shootingArea;

    public string bulletPrefName;

    public bool healing;

    private float lerpTimeHealing;
    // Start is called before the first frame update
    void Start()
    {
        iF = this.GetComponent<invincibilityFrames>();
        zoomCount = totalZoomCount;
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
        if (zooming)
        {
            float zoomStep = zoomTime / zoomCopies;
            zoomTimer += Time.deltaTime;
            for (var i = 0; i < zoomCopies; i++)
            {
                if (zoomTimer > (zoomStep * i) && zoomTimer < (zoomStep * (i + 1)))
                {
                    if (currentZoomCopies <= i)
                    {
                        PhotonNetwork.Instantiate(zoomCopy, this.transform.position,Quaternion.Euler(0,0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg - 90f), 0);
                        currentZoomCopies += 1;
                    }
                }
            }
            if (zoomTimer >= zoomTime)
            {
                zooming = false;
                currentZoomCopies = 0;
                rb.velocity = directionFacing * 10;
                zoomTimer = 0;
            }
        }
        if (lives >= maxLives)
        {
            lives = maxLives;
            
        }
        if (speeding)
        {
            if(this.transform.GetChild(0).GetComponent<Camera>().orthographicSize < 17)
            {
                if (ultActive)
                {
                    this.transform.GetChild(0).GetComponent<Camera>().orthographicSize += Time.deltaTime * Random.Range(75,125);
                }
                else
                {
                    this.transform.GetChild(0).GetComponent<Camera>().orthographicSize += Time.deltaTime * Random.Range(50,100);
                }
                
            }
        }
        if (!speeding)
        {
            if (this.transform.GetChild(0).GetComponent<Camera>().orthographicSize > 12)
            {
                if (ultActive)
                {
                    this.transform.GetChild(0).GetComponent<Camera>().orthographicSize -= Time.deltaTime * Random.Range(50, 100);
                }
                else
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
        if (zooming == false)
        {
            ShipRotation();
        }
        Ultimate();
        
        this.transform.GetChild(0).rotation = Quaternion.identity;
        this.transform.GetChild(0).position = new Vector3(this.transform.position.x, this.transform.position.y, -30);
        shipMovement();
        if (ammoCount > 0 && reloading == false)
        {
            Shoot();
        }
        
        
        Reload();
        Dash();
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
                lerpTime = 0;
                ultActive = true;
                canActivateUltimate = false;
            }
        }

        if (ultActive)
        {
            lerpTime += Time.deltaTime;
            PostProcessProfile ppF = this.transform.GetChild(0).GetComponent<PostProcessVolume>().profile;
            ChromaticAberration c = (ChromaticAberration) ppF.settings[0];
            MotionBlur m = (MotionBlur) ppF.settings[1];
            ColorGrading cG = (ColorGrading) ppF.settings[2];
            c.intensity.value = Mathf.Lerp(0, 0.1f, lerpTime * 2);
            m.shutterAngle.value = Mathf.Lerp(0, 360f, lerpTime);
            cG.toneCurveToeStrength.value = Mathf.Lerp(0, 0.25f, lerpTime);
            this.gameObject.GetComponent<PhotonView>().RPC("setSROpacity", PhotonTargets.All,1f, 0f, lerpTime);
            this.gameObject.GetComponent<PhotonView>().RPC("setTrailColorUlt", PhotonTargets.All);
            this.transform.GetChild(2).gameObject.SetActive(true);
            this.transform.GetChild(3).gameObject.SetActive(true);
            ultActiveTimer += Time.deltaTime;
            if (ultActiveTimer >= ultActiveTime)
            {
                lerpTime = 0;
                
                ultActive = false;
                ultimateReloadTimer = 0;
                ultActiveTimer = 0;
                canActivateUltimate = false;
            }
        }
        else
        {
            lerpTime += Time.deltaTime;
            PostProcessProfile ppF = this.transform.GetChild(0).GetComponent<PostProcessVolume>().profile;
            ChromaticAberration c = (ChromaticAberration) ppF.settings[0];
            MotionBlur m = (MotionBlur) ppF.settings[1];
            ColorGrading cG = (ColorGrading) ppF.settings[2];
            c.intensity.value = Mathf.Lerp(0.1f, 0f, lerpTime);
            m.shutterAngle.value = Mathf.Lerp(360f, 0f, lerpTime * 2f);
            cG.toneCurveToeStrength.value = Mathf.Lerp(0.25f, 0f, lerpTime * 2f);
            this.gameObject.GetComponent<PhotonView>().RPC("setSROpacity", PhotonTargets.All,0f, 1f, lerpTime);
            this.GetComponent<PhotonView>().RPC("setTrailColorReg", PhotonTargets.All);
        }
        
        
    }

     [PunRPC]
     void setUltActiveFalse()
     {
         if (ultActive)
         {
             lerpTime = 0;
                
             ultActive = false;
             ultimateReloadTimer = 0;
             ultActiveTimer = 0;
             canActivateUltimate = false;
         }
         
     }

     [PunRPC]
     void setSROpacity(float startOpacity, float opacity, float lerp)
     {
         
         if (this.gameObject.GetComponent<teamManager>().team ==
             GameObject.Find("teamScoreManager").GetComponent<teamScoreScript>().team)
         {
             this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,Mathf.Clamp(Mathf.Lerp(Mathf.Clamp(startOpacity + 0.2f, 0,1), opacity + 0.2f, lerp * 0.75f),0,1));
         }
         else
         {
             this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,Mathf.Lerp(startOpacity, opacity, lerp));
         }
         
     }
     
     [PunRPC]
     void setTrailColorReg()
     {
         Gradient g = new Gradient();
         GradientColorKey[] gradientColorKeys = new GradientColorKey[1];
         GradientColorKey gC = new GradientColorKey();
         gC.color = new Color(0.058f, 0.811f, 0.858f, 1);
         gradientColorKeys[0] = gC;
         g.colorKeys = gradientColorKeys;
         GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[1];
         GradientAlphaKey gCA = new GradientAlphaKey();
         gCA.alpha = 0;
         gradientAlphaKeys[0] = gCA;
         g.alphaKeys = gradientAlphaKeys;
         this.gameObject.GetComponent<TrailRenderer>().colorGradient = g;
         this.transform.GetChild(2).gameObject.SetActive(true);
         this.transform.GetChild(3).gameObject.SetActive(true);
         

     }

     [PunRPC]
     void setTrailColorUlt()
     {
         
         Gradient g = new Gradient();
         GradientAlphaKey[] gradientColorKeys = new GradientAlphaKey[1];
         GradientAlphaKey gC = new GradientAlphaKey();
         gC.alpha = 0;
         gradientColorKeys[0] = gC;
         g.alphaKeys = gradientColorKeys;
         this.gameObject.GetComponent<TrailRenderer>().colorGradient = g;
         this.transform.GetChild(2).gameObject.SetActive(false);
         this.transform.GetChild(3).gameObject.SetActive(false);
     }
    
    void ShipRotation(){
        Vector3 mousePos = this.transform.GetChild(0).GetComponent<Camera>().ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        float xdis = mousePos.x - this.transform.position.x;
        float ydis = mousePos.y - this.transform.position.y;
        
        directionFacing = new Vector2(xdis, ydis).normalized;
        float angle = Mathf.Atan2(ydis, xdis);

        
        this.transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg - 90);
        
		

			
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
                float angle = Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg;
                float angleStart = angle + angleSpread / 2;
                
                float angleEnd = angle - angleSpread / 2;
                float shotSpread = (angleSpread/2) / shotCount;
                for (var i = 0; i < shotCount; i++)
                {
                    float angleShotStart = angleStart - (shotSpread * (i));
                    float angleShotEnd = angleEnd + (shotSpread * (i));
                    GameObject bulletShotSide = PhotonNetwork.Instantiate(bulletPrefName, shootingArea.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                    bulletShotSide.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                    bulletScript bScriptShotSide = bulletShotSide.GetComponent<bulletScript> ();
                    bulletShotSide.gameObject.GetComponent<PhotonView>().RPC("setDirection", PhotonTargets.All, new Vector2(Mathf.Cos(angleShotStart * Mathf.Deg2Rad), Mathf.Sin(angleShotStart * Mathf.Deg2Rad)));
                    GameObject bulletEndSide = PhotonNetwork.Instantiate(bulletPrefName, shootingArea.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                    bulletEndSide.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                    bulletScript bScriptEndSide = bulletEndSide.GetComponent<bulletScript> ();
                    bulletEndSide.gameObject.GetComponent<PhotonView>().RPC("setDirection", PhotonTargets.All, new Vector2(Mathf.Cos(angleShotEnd * Mathf.Deg2Rad), Mathf.Sin(angleShotEnd * Mathf.Deg2Rad)));
                }
                GameObject bulletMiddle =
                    PhotonNetwork.Instantiate(bulletPrefName, shootingArea.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                bulletMiddle.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                bulletScript bScript = bulletMiddle.GetComponent<bulletScript> ();
                //bScript.origShooter = this.GetComponent<Collider2D>();
                //bScript.direction = new Vector2 (directionFacing.x, directionFacing.y);
                bulletMiddle.gameObject.GetComponent<PhotonView>().RPC("setDirection", PhotonTargets.All, new Vector2(directionFacing.x, directionFacing.y));

                shootTime = 0;
                ammoCount -= 1;
            }
        }
    }

    void Dash()
    {
        if (zooming == false)
        {
            if (Input.GetMouseButtonDown(1) && zoomCount > 0)
            {
                zooming = true;
                rb.AddForce(new Vector2(directionFacing.x * 5000,directionFacing.y  * 5000));
                zoomCount -= 1;
            }

            if (zoomCount <= 0)
            {
                zoomTimer += Time.deltaTime;
                if (zoomTimer >= zoomReloadTime)
                {
                    zoomCount = totalZoomCount;
                    zoomTimer = 0;
                }
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
    
    [PunRPC]
    void resetMaxLives()
    {
        lives = maxLives;
    }
    
    void doHealingEffect()
    {
        if (healing)
        {
            lerpTimeHealing += Time.deltaTime;
            PostProcessProfile ppF = this.transform.GetChild(0).GetComponent<PostProcessVolume>().profile;
            Vignette v = (Vignette) ppF.settings[4];
            Bloom b = (Bloom) ppF.settings[3];
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
            Vignette v = (Vignette) ppF.settings[4];
            Bloom b = (Bloom) ppF.settings[3];
            v.color.value = Color.Lerp(Color.yellow, Color.black, lerpTimeHealing);
            b.color.value = Color.Lerp(Color.yellow, Color.black, lerpTimeHealing);
            v.intensity.value = Mathf.Lerp(0.38f, 0, lerpTimeHealing);
            b.intensity.value = Mathf.Lerp(20f, 0f, lerpTimeHealing);
        }
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
