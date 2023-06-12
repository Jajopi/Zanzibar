using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    List<KeyCode> keysPressed = new List<KeyCode>();
    float moveSpeed = 3f;
    float orthoSize;
    Vector3 directionUp, directionDown, directionLeft, directionRight;
    Vector3 directionAscend, directionDescend;

    float maxHeight = 150f, minHeight = 50f;

    void Start()
    {
        directionUp = new Vector3(1, 0, 0);
        directionDown = new Vector3(-1, 0, 0);
        directionLeft = new Vector3(0, 0, 1);
        directionRight = new Vector3(0, 0, -1);

        directionAscend = new Vector3(0, 1, 0);
        directionDescend = new Vector3(0, -1, 0);

        orthoSize = transform.GetComponent<Camera>().orthographicSize;
    }

    void Update()
    {
        GetInputKeys();
        Move();
        LimitHeight();
    }

    void GetInputKeys()
    {
        void UpdateKey(KeyCode k)
        {
            if (Input.GetKey(k) && !keysPressed.Contains(k))
            {
                keysPressed.Add(k);
            }
            else if (!Input.GetKey(k) && keysPressed.Contains(k))
            {
                keysPressed.Remove(k);
            }

            /*if (Input.GetKeyUp(k)) { keysPressed.Remove(k); }
            else if (Input.GetKeyDown(k)) { keysPressed.Add(k); }

            else if (keysPressed.Contains(k) && !Input.GetKey(k)) {
                keysPressed.Remove(k);
            }*/
        }

        UpdateKey(KeyCode.W);
        UpdateKey(KeyCode.S);
        UpdateKey(KeyCode.A);
        UpdateKey(KeyCode.D);
        UpdateKey(KeyCode.UpArrow);
        UpdateKey(KeyCode.DownArrow);
        UpdateKey(KeyCode.LeftArrow);
        UpdateKey(KeyCode.RightArrow);

        UpdateKey(KeyCode.Q);
        UpdateKey(KeyCode.E);
    }

    void Move()
    {
        if (keysPressed.Count == 0) return;
        KeyCode k = keysPressed[keysPressed.Count - 1];

        if (k == KeyCode.W || k == KeyCode.UpArrow)
        {
            Translate(directionUp);
        }
        else if (k == KeyCode.S || k == KeyCode.DownArrow)
        {
            Translate(directionDown);
        }
        else if (k == KeyCode.A || k == KeyCode.LeftArrow)
        {
            Translate(directionLeft);
        }
        else if (k == KeyCode.D || k == KeyCode.RightArrow)
        {
            Translate(directionRight);
        }

        else if(keysPressed.Contains(KeyCode.W) || keysPressed.Contains(KeyCode.UpArrow))
        {
            Translate(directionUp);
        }
        else if (keysPressed.Contains(KeyCode.S) || keysPressed.Contains(KeyCode.DownArrow))
        {
            Translate(directionDown);
        }
        else if (keysPressed.Contains(KeyCode.A) || keysPressed.Contains(KeyCode.LeftArrow))
        {
            Translate(directionLeft);
        }
        else if (keysPressed.Contains(KeyCode.D) || keysPressed.Contains(KeyCode.RightArrow))
        {
            Translate(directionRight);
        }


        if (keysPressed.Contains(KeyCode.E))
        {
            Translate(directionAscend);
        }
        else if (keysPressed.Contains(KeyCode.Q))
        {
            Translate(directionDescend);
        }
        else return;
    }

    void Translate(Vector3 direction)
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime *
            Mathf.Sqrt(maxHeight - transform.position.y + maxHeight / minHeight),
            Space.World);
    }

    void LimitHeight()
    {
        float height = transform.position.y;
        height = Mathf.Max(Mathf.Min(height, maxHeight), minHeight);
        transform.position = new Vector3(transform.position.x,
            height, transform.position.z);
    }
}
