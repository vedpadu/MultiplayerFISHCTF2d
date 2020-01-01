using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shipUIScript : MonoBehaviour
{
    private Text livesText;
    private Text ammoText;
    public string shipType;
    // Start is called before the first frame update
    void Start()
    {
        livesText = this.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>();
        ammoText = this.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shipType == "sniper")
        {
            livesText.text = "Lives: " + this.gameObject.GetComponent<sniperShipScript>().lives;
            ammoText.text = "Ammo: " + this.gameObject.GetComponent<sniperShipScript>().ammoCount;
        }

        if (shipType == "healerSniper")
        {
            livesText.text = "Lives: " + this.gameObject.GetComponent<healerShipSniperScript>().lives;
            ammoText.text = "Ammo: " + this.gameObject.GetComponent<healerShipSniperScript>().ammoCount;
        }

        if (shipType == "machineGun")
        {
            livesText.text = "Lives: " + this.gameObject.GetComponent<machineGunShipScript>().lives;
            ammoText.text = "Ammo: " + this.gameObject.GetComponent<machineGunShipScript>().ammoCount;
        }

        if (shipType == "healer")
        {
            livesText.text = "Lives: " + this.gameObject.GetComponent<healerShipScript>().lives;
            ammoText.text = "Ammo: " + this.gameObject.GetComponent<healerShipScript>().ammoCount;
        }

        if (shipType == "assassin")
        {
            livesText.text = "Lives: " + this.gameObject.GetComponent<assassinScript>().lives;
            ammoText.text = "Ammo: " + this.gameObject.GetComponent<assassinScript>().ammoCount;
            Text dashText = this.transform.GetChild(1).GetChild(2).gameObject.GetComponent<Text>();
            dashText.text = "Dash Count: " + this.gameObject.GetComponent<assassinScript>().zoomCount;

        }

        if (shipType == "bomber")
        {
            livesText.text = "Lives: " + this.gameObject.GetComponent<bomberScript>().lives;
            ammoText.text = "Ammo: " + this.gameObject.GetComponent<bomberScript>().ammoCount;
        }

        if (shipType == "minigun")
        {
            livesText.text = "Lives: " + this.gameObject.GetComponent<minigunScript>().lives;
            ammoText.text = "Ammo: " + this.gameObject.GetComponent<minigunScript>().ammoCount;
        }
        if (shipType == "juggernaut")
        {
            livesText.text = "Lives: " + this.gameObject.GetComponent<juggernautScript>().lives;
            ammoText.text = "Ammo: " + this.gameObject.GetComponent<juggernautScript>().ammoCount;
        }
    }
}
