using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displayHealth : MonoBehaviour
{
    public string shipType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
            if (shipType == "sniper")
                    {
                        this.transform.localScale = new Vector3(Mathf.Clamp(this.transform.parent.parent.gameObject.GetComponent<sniperShipScript>().lives /
                                                                            this.transform.parent.parent.gameObject.GetComponent<sniperShipScript>().maxLives, 0, 1)
                            , 1, 1);
                    }
            
                    
            
                    if (shipType == "machineGun")
                    {
                        this.transform.localScale = new Vector3(Mathf.Clamp(this.transform.parent.parent.gameObject.GetComponent<machineGunShipScript>().lives /
                                                                            this.transform.parent.parent.gameObject.GetComponent<machineGunShipScript>().maxLives, 0, 1)
                            , 1, 1);
                    }
            
                    if (shipType == "healer")
                    {
                        this.transform.localScale = new Vector3(Mathf.Clamp(this.transform.parent.parent.gameObject.GetComponent<healerShipScript>().lives /
                                                                            this.transform.parent.parent.gameObject.GetComponent<healerShipScript>().maxLives, 0, 1)
                            , 1, 1);
                    }
            
                    if (shipType == "assassin")
                    {
                        this.transform.localScale = new Vector3(Mathf.Clamp(this.transform.parent.parent.gameObject.GetComponent<assassinScript>().lives /
                                                                            this.transform.parent.parent.gameObject.GetComponent<assassinScript>().maxLives, 0, 1)
                            , 1, 1);
                    }

                    if (shipType == "bomber")
                    {
                        this.transform.localScale = new Vector3(Mathf.Clamp(this.transform.parent.parent.gameObject.GetComponent<bomberScript>().lives /
                                                                            this.transform.parent.parent.gameObject.GetComponent<bomberScript>().maxLives, 0, 1)
                            , 1, 1);
                       
                    }

                    if (shipType == "minigun")
                    {
                        this.transform.localScale = new Vector3(
                                Mathf.Clamp(this.transform.parent.parent.gameObject.GetComponent<minigunScript>().lives /
                                            this.transform.parent.parent.gameObject.GetComponent<minigunScript>().maxLives,0,1), 1, 1);
                    }
                    if (shipType == "juggernaut")
                    {
                        
                            this.transform.localScale = new Vector3(Mathf.Clamp(this.transform.parent.parent.gameObject.GetComponent<juggernautScript>().lives /
                                                                                this.transform.parent.parent.gameObject.GetComponent<juggernautScript>().maxLives, 0, 1)
                                , 1, 1);
                        
                        
                    }
        
        
    }
}
