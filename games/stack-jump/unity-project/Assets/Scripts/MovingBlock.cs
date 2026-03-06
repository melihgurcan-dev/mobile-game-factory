using UnityEngine;

/// <summary>
/// Attached to the currently active moving block.
/// Slides back and forth between -range and +range.
/// </summary>
public class MovingBlock : MonoBehaviour
{
    private float speed;
    private float range;
    private int   direction;
    private bool  isMoving;

    public void Initialize(float speed, float range, int direction)
    {
        this.speed     = speed;
        this.range     = range;
        this.direction = direction;
        isMoving = true;
    }

    public void Stop() => isMoving = false;

    void Update()
    {
        if (!isMoving) return;

        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        if (transform.position.x >  range) direction = -1;
        if (transform.position.x < -range) direction =  1;
    }
}
