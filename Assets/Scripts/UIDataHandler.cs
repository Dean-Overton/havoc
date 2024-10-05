using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class UIDataHandler : MonoBehaviour
{
    private Gun weaponData;
    private GameController gameController;
    private TeleportSlash dash;
    private health_component healthComponent; 
    public int ammo;
    public int maxAmmo;
    public int health;
    public int maxhealth;
    public int dashes;
    public int maxDashes;
    

    // Start is called before the first frame update
    void Start()
    {
        weaponData = GetComponent<Gun>();
        dash = GetComponent<TeleportSlash>();
        healthComponent = GetComponent<health_component>();


    }

    // Update is called once per frame
    void Update()
    {

        ammo = weaponData.ammo;
        maxAmmo = weaponData.maxAmmo;
        health = healthComponent.getCurrentHealth();
        maxhealth = healthComponent.getMaxHealth();
        dashes = dash.currentDashAmount;
        maxDashes = dash.dashAmount;

    }


    //method for displaying messages to player
}
