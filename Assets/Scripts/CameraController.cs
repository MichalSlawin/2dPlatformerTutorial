using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float CAMERA_TIME = 1f;

    public GameObject player;

    private Vector3 offset;
    private float xOffset;

    private float cameraTime = CAMERA_TIME;


    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
        xOffset = offset.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        follow();
    }

    private void follow()
    {
        float x = player.transform.position.x + offset.x;
        float y = player.transform.position.y + offset.y;
        float z = player.transform.position.z + offset.z;

        iTween.MoveUpdate(this.gameObject, iTween.Hash("x", x, "y", y, "z", z, "time", cameraTime));
    }

    public void swapOffsetX(bool right)
    {
        if(right)
        {
            offset.x = xOffset;
        }
        else
        {
            offset.x = -xOffset;
        }
    }
}
