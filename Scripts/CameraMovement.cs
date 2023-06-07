using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    List<KeyCode> keysPressed = new List<KeyCode>();
    float moveSpeed = 10f;
    float orthoSize;
    Vector3 directionUp, directionDown, directionLeft, directionRight;

    void Start()
    {
        directionUp = new Vector3(1, 0, 0);
        directionDown = new Vector3(-1, 0, 0);
        directionLeft = new Vector3(0, 0, 1);
        directionRight = new Vector3(0, 0, -1);

        orthoSize = transform.GetComponent<Camera>().orthographicSize;
    }

    void Update()
    {
        GetInputKeys();
        Move();
    }

    void GetInputKeys()
    {
        void UpdateKey(KeyCode k)
        {
            if (Input.GetKeyUp(k)) { keysPressed.Remove(k); }
            else if (Input.GetKeyDown(k)) { keysPressed.Add(k); }
        }

        UpdateKey(KeyCode.W);
        UpdateKey(KeyCode.S);
        UpdateKey(KeyCode.A);
        UpdateKey(KeyCode.D);
        UpdateKey(KeyCode.UpArrow);
        UpdateKey(KeyCode.DownArrow);
        UpdateKey(KeyCode.LeftArrow);
        UpdateKey(KeyCode.RightArrow);
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
        else return;
    }

    void Translate(Vector3 direction)
    {
        transform.Translate(direction * moveSpeed *
            Time.deltaTime * orthoSize, Space.World);
    }
}
