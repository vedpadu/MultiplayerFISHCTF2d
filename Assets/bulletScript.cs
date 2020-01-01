using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    public bool hasRecoil;
    public float recoilForce;
    public float deathTime;
    public Collider2D origShooter;

    public Vector2 direction;

    public float bulletSpeed;
    
    public float bulletHealth;

    private Rigidbody2D rb;

    public bool isShotgun;
    public float damageFalloff;

    public float damage;

    public bool isGrenade;

    private float destroyTimer;

    public bool isHeal;

    public string explosionEffect;

    private CamerShake shk;

    public float shakeDuration;

    public float shakeMagnitude;

    public float victimShakeDuration;

    public float victimShakeMagnitude;

    private float team;

    private bool explode = true;
    // Start is called before the first frame update
    void Start()
    {
        
        team = origShooter.gameObject.GetComponent<teamManager>().team;
        if (team == 1)
        {
            this.gameObject.GetComponent<PhotonView>().RPC("setLayer", PhotonTargets.All, 10);
        }
        else
        {
            this.gameObject.GetComponent<PhotonView>().RPC("setLayer", PhotonTargets.All, 11);
        }
        Destroy (this.gameObject, deathTime);
        bulletHealth = damage;
        rb = this.GetComponent<Rigidbody2D> ();
        rb.rotation = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
        rb.AddForce (direction * bulletSpeed );
        if (hasRecoil)
        {
            if (origShooter != null)
            {
                if (origShooter.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    origShooter.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * -1 * recoilForce);
                }
            }
        }
        if (origShooter != null)
        {
            if (origShooter.gameObject.transform.GetChild(0) != null)
            {
                if (origShooter.gameObject.transform.GetChild(0).gameObject.GetComponent<CamerShake>() != null)
                {
                    shk = origShooter.gameObject.transform.GetChild(0).gameObject.GetComponent<CamerShake>();
                    StartCoroutine(shk.Shake(shakeDuration, shakeMagnitude));
                }
            }
        }
    }
    

    private void OnDestroy()
    {
        
        if (this.GetComponent<PhotonView>().isMine && explosionEffect != "")
        {
            if (isGrenade)
            {
                GameObject e = PhotonNetwork.Instantiate(explosionEffect, this.transform.position, this.transform.rotation, 0);
            }
            else
            {
                GameObject e = PhotonNetwork.Instantiate(explosionEffect, this.transform.position, this.transform.rotation, 0);
            }
            
            //e.GetComponent<explosionScript>().maker = 
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isShotgun)
        {
            damage -= damageFalloff * Time.deltaTime;
            bulletHealth = damage;
            
        }   
        if ((bulletHealth) <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger == true)
        {
            return;
        }

        if (isGrenade == false)
        {
             if (other.gameObject.tag == "bullet" && other.gameObject.GetComponent<bulletScript>().origShooter != origShooter)
            {
                if (other.gameObject.GetComponent<bulletScript>().isHeal == false)
                {
                    bulletHealth -= other.gameObject.GetComponent<bulletScript>().damage;
                }
                
                
            }

             if (other.gameObject.tag == "pushableRock")
             {
                 Destroy(this.gameObject);
             }

            if (other.gameObject.tag == "obstacle" )
            {
                Destroy(this.gameObject);
            }

            if (other.gameObject.tag == "shield")
            {
                Debug.Log("ree");
                if (other.gameObject.GetComponent<shieldScript>().team != team)
                {
                    Destroy(this.gameObject);
                }
            }
        

            if (other.gameObject.tag == "ship" && other.gameObject.GetComponent<Collider2D>() != origShooter && this.gameObject.tag == "bullet")
            {
                if (this.GetComponent<PhotonView>().isMine)
                {
                    if (isHeal == false)
                    {
                        if (other.gameObject.GetComponent<invincibilityFrames>() != null)
                        {
                            if (other.gameObject.GetComponent<invincibilityFrames>().invincible == false)
                            {
                                if (other.gameObject.GetComponent<shipUIScript>().shipType == "assassin")
                                {
                                    other.gameObject.GetComponent<PhotonView>().RPC("setUltActiveFalse", PhotonTargets.All);
                                }
                                if (other.gameObject.transform.GetChild(0).GetComponent<CamerShake>() != null)
                                {
                                    StartCoroutine(other.gameObject.transform.GetChild(0).GetComponent<CamerShake>().Shake(victimShakeDuration, victimShakeMagnitude));
                                }

                                if (other.gameObject.GetComponent<teamManager>().team != team)
                                {
                                    other.GetComponent<PhotonView>().RPC("punRemoveLife", PhotonTargets.All, damage);
                                }
                                
                                Destroy(this.gameObject);
                            }
                        }
                        else
                        {
                            if (other.gameObject.GetComponent<teamManager>().team != team)
                            {
                                other.GetComponent<PhotonView>().RPC("punRemoveLife", PhotonTargets.All, damage);
                            }
                            Destroy(this.gameObject);
                        }
                        
                    }
                    else
                    {
                        if (other.gameObject.GetComponent<teamManager>().team == team)
                        {
                            other.GetComponent<PhotonView>().RPC("punRemoveLife", PhotonTargets.All, -damage);
                        }
                        Destroy(this.gameObject);
                    }
                    
                }
                
                Destroy(this.gameObject);
            }
        }
           

            if (isGrenade && this.GetComponent<PhotonView>().isMine)
            {
                if (other.gameObject.tag == "bullet")
                {
                    PhotonNetwork.Destroy(this.gameObject);
                
                }
                if (other.gameObject.tag == "grenade")
                {
                    
                    PhotonNetwork.Destroy(this.gameObject);
                
                }
                if (other.gameObject.tag == "obstacle")
                {
                    rb.AddRelativeForce(bulletSpeed * Vector2.right);
                    //rb.velocity = Vector2.Reflect(rb.velocity * -1, rb.velocity.normalized * -1);
                    //PhotonNetwork.Destroy(this.gameObject);
                }
                
                if (other.gameObject.tag == "pushableRock")
                {
                    if (other.gameObject.GetComponent<pushableRockScript>().noDoDamage == false)
                    {
                        PhotonNetwork.Destroy(this.gameObject);
                    }
                    else
                    {
                        rb.AddRelativeForce(bulletSpeed * Vector2.right);
                    }
                    
                }

                if (other.gameObject.tag == "shield")
                {
                    if (other.gameObject.GetComponent<shieldScript>().team != team)
                    {
                        //explode = false;
                        //PhotonNetwork.Destroy(this.gameObject);
                    }
                    
                }
                

        

                if (other.gameObject.tag == "ship" && other.gameObject.GetComponent<teamManager>().team != team)
                {

                    PhotonNetwork.Destroy(this.gameObject);
                } 
            }
        
        
    }

    [PunRPC]
    void setBulletColor(float r, float g, float b, float a)
    {
        this.GetComponent<SpriteRenderer>().color = new Color(r, g, b, a);
    }

    [PunRPC]
    void setOrigShooter(String gameObjectName, int id)
    {
        GameObject[] gOArray = GameObject.FindGameObjectsWithTag(gameObjectName);
        for (var i = 0; i < gOArray.Length; i++)
        {
            if (gOArray[i].GetComponent<PhotonView>().ownerId == id)
            {
                origShooter = gOArray[i].GetComponent<Collider2D>();
            }
        }
        
    }

    [PunRPC]
    void setDirection(Vector2 dir)
    {
        direction = dir;
    }

    [PunRPC]
    void destroyThis(int bruh)
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
}
