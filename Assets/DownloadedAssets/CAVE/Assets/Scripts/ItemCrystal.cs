using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCrystal : MonoBehaviour {

    // declaring the "ShootProjectile" script, which will be influenced when the player takes this item
    public ShootProjectile ShootScript;
    // drag your "Projectile-Crystal..." of choice from the Prefabs / Items folder into this field
    public GameObject crystalProjectilePrefab;
    // declaring the UI image which will be used to display player's newly found crystals * * * You will need to assign your image from your UI canvas into this field * * * 
    public GameObject currentItemImage;
    // an array of sprites used for all the crystal variation ( * the prefabs already have these fields setup * )
    public Sprite[] currentItemSprites;

    [Header("Type the Nr of Crystal variation used (1-6)")]
    // You will have to type in this field (ex. 3), the Number which coincides with the Crystal Prefab variation you have chosen for this Item
    public int NrOfCrystalUsed;

    // Use this for initialization
    void Start () {
        // fetching the "ShootProjectile" script, which is attached to your player ( * tagged "Player" * ) gameObject
        ShootScript = GameObject.FindGameObjectWithTag("Player").GetComponent<ShootProjectile>();
    }
	
    // an object is in the trigger zone of this Item...
    void OnTriggerStay(Collider col)
    {
        //... and the object is actually tagged "Player"
        if (col.CompareTag("Player"))
        {
            // the player clicks Left MouseButton
            if (Input.GetMouseButtonUp(0))
            {
                // assign Your chosen Projectile-Crystal Prefab to the "ShootProjectile" script (from now on your player shoots this crystal)
                ShootScript.projectile = crystalProjectilePrefab;
                // the item will self destruct in 0.5 seconds...
                Destroy(gameObject, 0.5f);

                // if You chose crystal #01 and actually typed "1" in the "NrOfCrystalUsed" field
                if (NrOfCrystalUsed == 1)
                {
                    // activate the UI Image (displaying your chosen crystal)
                    currentItemImage.SetActive(true);
                    // assign the first Sprite to the above Image
                    ShootScript.currentItem.sprite = currentItemSprites[0];
                    // congrats! you now have 10 of these items to shoot
                    ShootScript.ammoCount = 10;
                }

                // if You chose crystal #02 and actually typed "2" in the "NrOfCrystalUsed" field
                if (NrOfCrystalUsed == 2)
                {
                    // (displaying your chosen crystal)
                    currentItemImage.SetActive(true);
                    // assign the second Sprite to the above Image
                    ShootScript.currentItem.sprite = currentItemSprites[1];
                    // congrats! you now have 10 of these items to shoot
                    ShootScript.ammoCount = 10;
                }

                // if You chose crystal #03 and actually typed "3" in the "NrOfCrystalUsed" field
                if (NrOfCrystalUsed == 3)
                {
                    // (displaying your chosen crystal)
                    currentItemImage.SetActive(true);
                    // assign the third Sprite to the above Image
                    ShootScript.currentItem.sprite = currentItemSprites[2];
                    // congrats! you now have 10 of these items to shoot
                    ShootScript.ammoCount = 10;
                }

                // if You chose crystal #04 and actually typed "4" in the "NrOfCrystalUsed" field
                if (NrOfCrystalUsed == 4)
                {
                    // (displaying your chosen crystal)
                    currentItemImage.SetActive(true);
                    // assign the fourth Sprite to the above Image
                    ShootScript.currentItem.sprite = currentItemSprites[3];
                    // congrats! you now have 10 of these items to shoot
                    ShootScript.ammoCount = 10;
                }

                // if You chose crystal #05 and actually typed "5" in the "NrOfCrystalUsed" field
                if (NrOfCrystalUsed == 5)
                {
                    // (displaying your chosen crystal)
                    currentItemImage.SetActive(true);
                    // assign the fifth Sprite to the above Image
                    ShootScript.currentItem.sprite = currentItemSprites[4];
                    // congrats! you now have 10 of these items to shoot
                    ShootScript.ammoCount = 10;
                }

                // if You chose crystal #06 and actually typed "6" in the "NrOfCrystalUsed" field
                if (NrOfCrystalUsed == 6)
                {
                    // (displaying your chosen crystal)
                    currentItemImage.SetActive(true);
                    // assign the sixth Sprite to the above Image
                    ShootScript.currentItem.sprite = currentItemSprites[5];
                    // congrats! you now have 10 of these items to shoot
                    ShootScript.ammoCount = 10;
                }
            }
        }
    }
}
