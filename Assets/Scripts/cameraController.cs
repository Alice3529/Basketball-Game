using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    Camera camera;
    [SerializeField] float upSpeed = 15f;
    [SerializeField] float bottomSpeed = 15f;
    [SerializeField] float bottomBasketSpeed = 15f;
    Vector3 startBallPos;
    ball1 ball1;
    Vector3 endPos;
    Vector3 startPos;
    coordManager coordManager;
    int up = 0;
    float currentSpeed;

    void Awake()
    {
        ball1 = FindObjectOfType<ball1>();
        camera = GetComponent<Camera>();
        coordManager = FindObjectOfType<coordManager>();
        ball1.camUp += CameraUp;
        ball1.camBottom += CameraBottom;
     
    }

    private void CameraBottom(Vector3 obj, int speedMode)
    {
        currentSpeed=(speedMode==1)?bottomSpeed:bottomBasketSpeed;
        up = 2;
        startBallPos = obj;
    }

 

    public void CameraUp(Vector3 basketDif, float fingerDif)
    {
        if (fingerDif <= coordManager.cameraNotMove) { return; }
        startPos=Camera.main.transform.position;
        endPos = basketDif;
        up = 1;
    }




    private void LateUpdate()
    {
        if (up == 1)
        {
            Vector3 desiredPosition = new Vector3(camera.transform.position.x, startPos.y + endPos.y, -10f);
            Vector3 smoothedPosition = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * upSpeed);
            transform.position = new Vector3(Camera.main.transform.position.x, smoothedPosition.y, -10f);
        }
        else if (up == 2)
        {
            Vector3 desiredPosition = new Vector3(camera.transform.position.x, startBallPos.y, -10f);
            Vector3 smoothedPosition = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * currentSpeed);
            transform.position = new Vector3(Camera.main.transform.position.x, smoothedPosition.y, -10f);
        }
    }


}
