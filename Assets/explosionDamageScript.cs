using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionDamageScript : MonoBehaviour
{
    public float shakeMagnitude;

    public float shakeDuration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "explosion" && this.gameObject.GetComponent<PhotonView>().isMine)
        {
            if (this.GetComponent<invincibilityFrames>() != null)
            {
                if (this.GetComponent<invincibilityFrames>().invincible == false)
                {
                    if (this.gameObject.GetComponent<shipUIScript>().shipType == "assassin")
                    {
                        this.gameObject.GetComponent<assassinScript>().ultActive = false;
                        this.gameObject.GetComponent<assassinScript>().canActivateUltimate = false;
                        this.gameObject.GetComponent<assassinScript>().ultimateReloadTimer = 0;
                    }
                    Vector2 direction = (this.transform.position - other.transform.position).normalized;
                    float distance = Vector2.Distance(other.transform.position, this.transform.position);
                    //other.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    // if (other.GetComponent<PhotonView>().isMine)
                    //{
                    if (distance > 1)
                    {
                        if (this.transform.GetChild(0).gameObject.GetComponent<CamerShake>() != null)
                        {
                            
                            StartCoroutine(this.transform.GetChild(0).gameObject.GetComponent<CamerShake>()
                                .Shake(other.gameObject.GetComponent<explosionScript>().shakeDuration, other.gameObject.GetComponent<explosionScript>().shakeMagnitude/distance));

                            
                           
                            
                        }
                        this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2((direction.x * other.gameObject.GetComponent<explosionScript>().explosionForce) / distance, (direction.y * other.gameObject.GetComponent<explosionScript>().explosionForce) / distance));
                    }
                    else
                    {
                        if (this.transform.GetChild(0).gameObject.GetComponent<CamerShake>() != null)
                        {
                            
                            StartCoroutine(this.transform.GetChild(0).gameObject.GetComponent<CamerShake>()
                                .Shake(other.gameObject.GetComponent<explosionScript>().shakeDuration, other.gameObject.GetComponent<explosionScript>().shakeMagnitude));

                            
                           
                            
                        }
                        this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2((direction.x * other.gameObject.GetComponent<explosionScript>().explosionForce), (direction.y * other.gameObject.GetComponent<explosionScript>().explosionForce)));
                    }
            
                    //}
            
            
                    float damage = (other.GetComponent<explosionScript>().explosionDamage/distance);
                    if(distance < 1)
                    {
                        damage = (other.GetComponent<explosionScript>().explosionDamage);
                    }
                
                    this.gameObject.GetComponent<PhotonView>().RPC("punRemoveLife", PhotonTargets.All, damage);

                }
            }
            else
            {
                Vector2 direction = (this.transform.position - other.transform.position).normalized;
                float distance = Vector2.Distance(other.transform.position, this.transform.position);
                //other.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                // if (other.GetComponent<PhotonView>().isMine)
                //{
                if (distance > 1)
                {
                    if (this.transform.GetChild(0).gameObject.GetComponent<CamerShake>() != null)
                    {
                            
                        StartCoroutine(this.transform.GetChild(0).gameObject.GetComponent<CamerShake>()
                            .Shake(other.gameObject.GetComponent<explosionScript>().shakeDuration, other.gameObject.GetComponent<explosionScript>().shakeMagnitude/distance));

                            
                           
                            
                    }
                    this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2((direction.x * other.gameObject.GetComponent<explosionScript>().explosionForce) / distance, (direction.y * other.gameObject.GetComponent<explosionScript>().explosionForce) / distance));
                }
                else
                {
                    if (this.transform.GetChild(0).gameObject.GetComponent<CamerShake>() != null)
                    {
                            
                        StartCoroutine(this.transform.GetChild(0).gameObject.GetComponent<CamerShake>()
                            .Shake(other.gameObject.GetComponent<explosionScript>().shakeDuration, other.gameObject.GetComponent<explosionScript>().shakeMagnitude));
                            
                           
                            
                    }
                    this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2((direction.x * other.gameObject.GetComponent<explosionScript>().explosionForce), (direction.y * other.gameObject.GetComponent<explosionScript>().explosionForce)));
                }
            
                //}
            
            
                float damage = (other.GetComponent<explosionScript>().explosionDamage/distance);
                if(distance < 1)
                {
                    damage = (other.GetComponent<explosionScript>().explosionDamage);
                }
                
                this.gameObject.GetComponent<PhotonView>().RPC("punRemoveLife", PhotonTargets.All, damage);

            }
            
        }
    }
}
