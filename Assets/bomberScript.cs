using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bomberScript : MonoBehaviour
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

    private int mineCount;
    public int totalMineCount;
    private List<GameObject> landMines = new List<GameObject>();
    
    public float ammoCount;
    public float reloadTime;
    private float reloadTimer;
    private float maxAmmoCount;
    private bool reloading = false;

    public string minePrefName;
    public string bulletPrefName;

    public Transform shootingArea;

    private invincibilityFrames iF;

    public bool healing;
    // Start is called before the first frame update
    void Start()
    {
        iF = this.GetComponent<invincibilityFrames>();
        maxAmmoCount = ammoCount;
        lives = maxLives;
        mineCount = totalMineCount;
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
            GameObject.Find("teamScoreManager").GetComponent<PhotonView>().RPC("addScore", PhotonTargets.All, 1f, this.gameObject.GetComponent<teamManager>().team);
            this.GetComponent<PhotonView>().RPC("setInvincible", PhotonTargets.All, true);
            PhotonNetwork.Instantiate("bigExplosion", this.transform.position, this.transform.rotation, 0);
            this.transform.position = this.gameObject.GetComponent<respawnPosSave>().respawnPos;
            this.GetComponent<PhotonView>().RPC("punResetLife", PhotonTargets.All, maxLives);
            rb.velocity = Vector2.zero;
        }
        shootTime += Time.deltaTime;
        ShipRotation();

            this.transform.GetChild(0).rotation = Quaternion.identity;
        this.transform.GetChild(0).position = new Vector3(this.transform.position.x, this.transform.position.y, -30);
        shipMovement();
        if (ammoCount > 0 && reloading == false)
        {
            Shoot();
        }
        doLandMines();
        
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

        
        this.transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg - 90);
        
		

			
    }

    void doLandMines()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (mineCount > 0)
            {
                GameObject mine1 = (GameObject)PhotonNetwork.Instantiate(minePrefName, this.transform.position, this.transform.rotation, 0);
                mine1.GetComponent<PhotonView>().RPC("setTeam", PhotonTargets.All, this.gameObject.GetComponent<teamManager>().team);
                landMines.Add(mine1);
                mineCount -= 1;
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            foreach (GameObject mine1 in landMines)
            {
                mine1.GetComponent<landMineScript>().Explode();;
                
            }
            landMines.Clear();
            mineCount = totalMineCount;
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
                int teamTemp = 0;
                if (this.gameObject.GetComponent<teamManager>().team == 1)
                {
                    teamTemp = 10;
                }
                else
                {
                    teamTemp = 11;
                }
                //bullet.gameObject.GetComponent<PhotonView>().RPC("setLayerBullet", PhotonTargets.All, gameObject.tag, teamTemp);
                bulletScript bScript = bullet.GetComponent<bulletScript> ();
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
