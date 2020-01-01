using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class healerShipScript : MonoBehaviour
{
    private Text livesText;
    public bool speeding;
    private float speed;
    public bool shooting;
    private float shootTime;
    public float fireRate;
    private Vector2 directionFacing;
    public float lives = 10;
    public float maxLives;
    private Rigidbody2D rb;
    
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
    private float lerpTime;
    
    private bool shootingRight;
    public float fireRateSecondary;
    public string secondaryPrefName;

    public string bulletPrefName;

    private invincibilityFrames iF;

    public Transform shootingArea;

    public float baseSpeed;

    public bool healing;
    

    private float lerpTimeHealing;
    // Start is called before the first frame update
    void Start()
    {
        iF = this.GetComponent<invincibilityFrames>();
        maxAmmoCount = ammoCount;
        rb = this.GetComponent<Rigidbody2D>();
        lives = maxLives;
        livesText = this.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>();
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
        livesText.text = "Lives: " + lives.ToString();
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
            Secondary();
        }
        doHealingEffect();
        
        Reload();
    }

    void doHealingEffect()
    {
        if (healing)
        {
            lerpTimeHealing += Time.deltaTime;
            PostProcessProfile ppF = this.transform.GetChild(0).GetComponent<PostProcessVolume>().profile;
            Vignette v = (Vignette) ppF.settings[0];
            Bloom b = (Bloom) ppF.settings[1];
            v.color.value = Color.yellow;
            b.color.value = Color.yellow;
            v.intensity.value = Mathf.Lerp(0, 0.38f, lerpTimeHealing * 0.5f);
            b.intensity.value = Mathf.Lerp(0, 20f, lerpTimeHealing * 0.5f);
            if (lerpTimeHealing >= 2)
            {
                lerpTimeHealing = 0;
                healing = false;
            }
        }
        else
        {
            lerpTimeHealing += Time.deltaTime;
            PostProcessProfile ppF = this.transform.GetChild(0).GetComponent<PostProcessVolume>().profile;
            Vignette v = (Vignette) ppF.settings[0];
            Bloom b = (Bloom) ppF.settings[1];
            v.color.value = Color.Lerp(Color.yellow, Color.black, lerpTimeHealing);
            b.color.value = Color.Lerp(Color.yellow, Color.black, lerpTimeHealing);
            v.intensity.value = Mathf.Lerp(0.38f, 0, lerpTimeHealing * 0.5f);
            b.intensity.value = Mathf.Lerp(20f, 0f, lerpTimeHealing * 0.5f);
        }
    }
    
    void ShipRotation(){
        Vector3 mousePos = this.transform.GetChild(0).GetComponent<Camera>().ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        float xdis = mousePos.x - this.transform.position.x;
        float ydis = mousePos.y - this.transform.position.y;
        
        directionFacing = new Vector2(xdis, ydis).normalized;
        float angle = Mathf.Atan2(ydis, xdis);

        
        this.transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg - 90);
        
		

			
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
                GameObject[] shipArray = GameObject.FindGameObjectsWithTag("ship");
                for (var i = 0; i < shipArray.Length; i++)
                {
                    if (shipArray[i].GetComponent<teamManager>().team ==
                        this.gameObject.GetComponent<teamManager>().team)
                    {
                        shipArray[i].GetComponent<PhotonView>().RPC("setHealingTrue", PhotonTargets.All);
                        shipArray[i].GetComponent<PhotonView>().RPC("resetMaxLives", PhotonTargets.All);
                    }
                }
                lerpTime = 0;
                ultActive = true;
                canActivateUltimate = false;
            }
        }

        if (ultActive)
        {
            lerpTime += Time.deltaTime;
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

    void Secondary()
    {
        if (Input.GetMouseButtonDown(1))
        {
            shootingRight = true;
        }
        if (Input.GetMouseButtonUp (1)) {
            shootingRight = false;
        }

        if (shootingRight) {
            if (shootTime > 1 / fireRateSecondary)
            {
                GameObject bullet =
                    PhotonNetwork.Instantiate(secondaryPrefName, shootingArea.position, Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                bullet.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                bulletScript bScript = bullet.GetComponent<bulletScript> ();
                //bScript.origShooter = this.GetComponent<Collider2D>();
                //bScript.direction = new Vector2 (directionFacing.x, directionFacing.y);
                bullet.gameObject.GetComponent<PhotonView>().RPC("setDirection", PhotonTargets.All, new Vector2(directionFacing.x, directionFacing.y));

                shootTime = 0;
                ammoCount -= 1;
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
                    PhotonNetwork.Instantiate(bulletPrefName, shootingArea.position,Quaternion.Euler(0, 0, Mathf.Atan2(directionFacing.y, directionFacing.x) * Mathf.Rad2Deg), 0);
                bullet.gameObject.GetComponent<PhotonView>().RPC("setOrigShooter", PhotonTargets.All, gameObject.tag, this.GetComponent<PhotonView>().ownerId);
                bulletScript bScript = bullet.GetComponent<bulletScript> ();
                //bScript.origShooter = this.GetComponent<Collider2D>();
                //bScript.direction = new Vector2 (directionFacing.x, directionFacing.y);
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
    void resetMaxLives()
    {
        lives = maxLives;
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
