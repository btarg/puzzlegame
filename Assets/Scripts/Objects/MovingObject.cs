using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TimeBody))]
public class MovingObject : MonoBehaviour
{
    public Transform[] target;
    public float speed;
    public bool reset = false;
    public bool loop = false;

    public bool isMoving = true;
    public UnityEvent onReachEnd;


    int current = 0;
    bool reachedEnd = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<TimeBody>().isRewinding || GetComponent<TimeBody>().isFrozen || !isMoving)
        {
            return;
        }

        if (transform.position != target[current].position)
        {
			// Move towards the next target point
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target[current].position, step);
        }
        else
        {
            if (loop)
            {
				// Move towards the start point, useful for making stuff go in circles or around a track
                current = (current + 1) % target.Length;
            }
            else
            {
                current++;
            }
            if (current == target.Length)
            {
                isMoving = false;
                current -= 1;
                onReachEnd.Invoke();
                reachedEnd = true;

                if (reset) {
					// When "reset" is enabled, teleport back to the start point
                    transform.position = target[0].position;
                    StartMoving();
                }
            }
        }
    }

    // This can be used to move again once the object has stopped moving: 
    // it will move towards the first target if it has already finished a cycle
    public void StartMoving()
    {
        if (reachedEnd)
        {
            current = 0;
            reachedEnd = false;
        }

        isMoving = true;
    }
    public void StopMoving()
    {
        isMoving = false;
    }
}