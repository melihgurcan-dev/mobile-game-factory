using UnityEngine;

/// <summary>
/// Moves a cut-off block piece downward and sideways without using Rigidbody2D,
/// avoiding MeshCollider/Rigidbody2D conflicts on primitive GameObjects.
/// </summary>
public class FallingPiece : MonoBehaviour
{
    private float velocityY  = 0f;
    private float velocityX  = 0f;
    private const float gravity = -12f;

    public void Launch(float nudgeX)
    {
        velocityX = nudgeX;
        velocityY = 0f;
    }

    void Update()
    {
        velocityY += gravity * Time.deltaTime;
        transform.Translate(velocityX * Time.deltaTime, velocityY * Time.deltaTime, 0f);
    }
}
