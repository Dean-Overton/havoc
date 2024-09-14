using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailCheckpoint : MonoBehaviour {
    // declaring the Minecart script, which will be traveling on these rails  ( * You will need to assign the corresponding Minecart to each Rail which has this script on it * )
    public Minecart minecartScript;

    // an object is in the trigger zone of this rail...
    void OnTriggerEnter(Collider collider)
    {
        //... and the object is actually tagged "Minecart"
        if (collider.tag == "Minecart")
        {
            // if the Minecart is currently set to "forward" bool condition
            if (minecartScript.forward)
            {
                // add +1 to the "i" integer (next checkpoint from array)
                minecartScript.i += 1;
            }
            // or else, if the Minecart is currently NOT set to "forward" bool condition
            else if (!minecartScript.forward)
            {
                // subtract -1 from the "i" integer (previous checkpoint from array)
                minecartScript.i -= 1;
            }
        }
    }
}
