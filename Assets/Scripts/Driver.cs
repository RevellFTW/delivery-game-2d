using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Driver : MonoBehaviour
{
   [SerializeField] float steerSpeed = 0.1f;
  [SerializeField] public static float moveSpeed = 20f;
  [SerializeField] float slowSpeed = 15f;
  [SerializeField] float boostSpeed = 25f;

   Vector2 oldposition = Vector2.zero;

    public static bool canMove = true;
    // Start is called before the first frame update
    void Start()
    {
        oldposition = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
      
        if (canMove)
        {
            float steerAmount = Input.GetAxis("Horizontal") * steerSpeed * Time.deltaTime;
            float moveAmount = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            transform.Rotate(0, 0, -steerAmount);
            transform.Translate(0, moveAmount, 0);
        }
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        if (currentPosition  != oldposition)
        {
            Delivery.DeductFuel();
            oldposition = currentPosition;
        }
    }
    void OnCollisionEnter2D(Collision2D other){

         moveSpeed = slowSpeed;
    }


    void OnTriggerEnter2D(Collider2D other) 
    {
      
        if(other.tag == "Boost"){
            
         moveSpeed = boostSpeed;
        }
        
      }

    public static void setMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
