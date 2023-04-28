using System;
using Mirror;
using UnityEngine;

public class DCMRigidBodyPush : MonoBehaviour
{
    public LayerMask pushLayers;
    public bool canPush;
    [Range(0.5f, 5f)] public float strength = 1.1f;
    private MyNetworkPlayer player;

    private void Start()
    {
        player = GetComponent<MyNetworkPlayer>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (canPush) PushRigidBodies(hit);
    }

    private void PushRigidBodies(ControllerColliderHit hit)
    {
        // https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

        // make sure we hit a non kinematic rigidbody
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;

        // make sure we only push desired layer(s)
        var bodyLayerMask = 1 << body.gameObject.layer;
        if ((bodyLayerMask & pushLayers.value) == 0) return;

        OnHit(hit.gameObject);
        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f) return;

        // Calculate push direction from move direction, horizontal motion only
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

        // Apply the push and take strength into account
        body.AddForce(pushDir * strength, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision!=null)
            OnHit(collision.gameObject);
    }

    public void OnHit(GameObject other)
    {
        
        if (other.CompareTag("Collidable"))
        {
            other.tag = "Uncollidable";
            // Collided components must have rigidbody to be registered to score
            if (other.GetComponent<Rigidbody>() == null)
                return;
            if(other.name.Contains("Building"))
                player.setScore(500);
            if (other.name.Contains("Car_"))
                player.setScore(300);
            player.setScore(100);
            // player.RpcDisplayScore();
            // Debug.Log("Collided");
        }
    }
}