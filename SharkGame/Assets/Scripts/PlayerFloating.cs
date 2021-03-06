using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloating : MonoBehaviour
{
    public Rigidbody2D playerRigid;
    public float kFloatk;
    public float kGravity;
    // Start is called before the first frame update
    void Start()
    {
        playerRigid = this.gameObject.GetComponent<Rigidbody2D>();
    }



    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.transform.position.y<0)
        {
           
            float floatForce = 2 - this.gameObject.transform.position.y;
            if (this.playerRigid.velocity.y > 0)
            {
                playerRigid.AddForce(new Vector2(0, kFloatk * floatForce/3), ForceMode2D.Force);
                print("Float");
            }
            else
            {
                playerRigid.AddForce(new Vector2(0, kFloatk * floatForce), ForceMode2D.Force);
                print("Float");
            }
            
        }

        if (this.gameObject.transform.position.y > 0)
        {
          
            playerRigid.AddForce(new Vector2(0, -kGravity), ForceMode2D.Force);
            print("Float");
        }

    }
}
