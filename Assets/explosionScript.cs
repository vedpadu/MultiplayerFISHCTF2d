using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionScript : MonoBehaviour
{
    public float deathTime;

    private float deathTimer;

    public float shakeDuration;
    public float shakeMagnitude;

    public float explosionDamage;

    public float explosionForce;

    public Collider2D maker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        deathTimer += Time.deltaTime;
        if (this.GetComponent<PhotonView>().isMine)
        {
            if (deathTimer >= deathTime - Time.deltaTime)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
        
    }
}
