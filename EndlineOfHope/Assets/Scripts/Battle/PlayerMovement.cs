using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] GameObject player;
    Rigidbody2D playerRigidBody = null;

    [Range(-10, 10)]
    [SerializeField] float timePower = 1;
    float powerX = 0;
    float powerY = 0;
    bool direction = false;
    bool directionY = false;

    const float ADD_FORCE       = 250f;

    private void Awake() {

        if(playerRigidBody == null) {

            playerRigidBody = player.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if(playerRigidBody != null) {

            Vector2 addForce = new(0, 0);
            bool input = false;

            if (Input.GetKey(KeyCode.D)) {

                addForce.x += 1;
                input = true;
            }
            if (Input.GetKey(KeyCode.A)) {

                addForce.x -= 1;
                input = true;
            }
            if (Input.GetKey(KeyCode.W)) {

                addForce.y += 1;
                input = true;
            }
            if (Input.GetKey(KeyCode.S)) {

                addForce.y -= 1;
                input = true;
            }

            if(input) {

                addForce.Normalize();
                playerRigidBody.velocity = ADD_FORCE * addForce * Time.deltaTime * timePower;
            }
            else {
                if(Mathf.Abs(playerRigidBody.velocity.x) < 0.1f) {

                    playerRigidBody.velocity = new Vector2(0, playerRigidBody.velocity.y) ;
                }
                if (Mathf.Abs(playerRigidBody.velocity.y) < 0.1f) {

                    playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0);
                }
                playerRigidBody.velocity = playerRigidBody.velocity / 2;
            }
        }   
    }
}
