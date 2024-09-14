using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    // declaring the "ShootProjectile" script, which will be influenced when the player takes this item
    public ShootProjectile ShootScript;
    // drag your Custom-Made Item Prefab into this field
    public GameObject itemPrefab;
    // declaring the UI image which will be used to display player's newly found crystals * * * You will need to assign your image from your UI canvas into this field * * * 
    public GameObject currentItemImage;
    // the sprite used on the above Image ( * the prefab already has this field setup * )
    public Sprite currentItemSprite;

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
                // activate the UI Image (displaying your dynamite sprite)
                currentItemImage.SetActive(true);
                // assign the Sprite to the above Image
                ShootScript.currentItem.sprite = currentItemSprite;
                // assign the "Dynamite-Explosive" Prefab to the "ShootProjectile" script (from now on your player shoots this item)
                ShootScript.projectile = itemPrefab;
                // congrats! you now have 10 of these items to shoot
                ShootScript.ammoCount = 10;
                // the item will self destruct in 0.5 seconds...
                Destroy(gameObject, 0.5f);
            }
        }
    }
}
