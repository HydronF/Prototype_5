using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class PlayerDiving : MonoBehaviour
{

    public Rigidbody2D playerRigid;
    public float diveMove = 1;
    // Start is called before the first frame update
    void Start()
    {
        playerRigid = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            print("Jump");
            if (this.gameObject.transform.position.y < 0.05)
            {
               // this.gameObject.transform.position -= new Vector3(0, diveMove, 0);
                 playerRigid.AddForce(new Vector2(0,-diveMove),ForceMode2D.Impulse);
           
            }

        }
    }
}
