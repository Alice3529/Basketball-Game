using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class net : MonoBehaviour
{
    ball1 ball;
    Vector3 startScale;
    [SerializeField] float endNetScale;
    coordManager coordManager;
    Vector3 ballStartPos;
    int change = 0;
    float time;
    [SerializeField] float netSimulatedValue=0.2f;
    [SerializeField] float speed;


    private void Start()
    {
        ball = FindObjectOfType<ball1>();
        startScale= transform.localScale;
        coordManager=FindObjectOfType<coordManager>();
    }

    public void ChangeNetSize(float val1)
    {
        float val = Remap(val1, 0, 0.7f, 0, 1f);
        float yScale = Mathf.Lerp(startScale.y, endNetScale, val);
        transform.localScale=(new Vector3(transform.localScale.x, yScale, transform.localScale.y));
        MoveBall(yScale-startScale.y);

    }

    private void Update()
    {
        if (change==1)
        {
            time += Time.deltaTime*speed;
            float simulatedValue=Mathf.Lerp(0, netSimulatedValue, time);
            if (simulatedValue == netSimulatedValue)
            {
                change = 2;
                time = 0f;
            }
            ChangeNetSize(simulatedValue);
        }
        else if (change==2)
        {
            time += Time.deltaTime * speed;
            float simulatedValue = Mathf.Lerp(0, netSimulatedValue, time);
            if (simulatedValue == netSimulatedValue)
            {
                time = 0f;
                change = 0;
            }
            ChangeNetSize(netSimulatedValue - simulatedValue);
        }
    }

    public void MoveBall(float yScale)
    {
        if (ball.inBasket == true)
        {
            ballStartPos = coordManager.rotateBasket.bottomBallPoint.localPosition;
            ball.transform.localPosition = new Vector3(0f, ballStartPos.y - yScale, transform.localPosition.z);
        }
    }

 

    float Remap(float source, float sourceFrom, float sourceTo, float targetFrom, float targetTo)
    {
        return targetFrom + (source - sourceFrom) * (targetTo - targetFrom) / (sourceTo - sourceFrom);
    }

    public void StartSize()
    {
        transform.localScale = startScale;
        ballStartPos = coordManager.rotateBasket.bottomBallPoint.localPosition;
        ball.transform.localPosition = ballStartPos;

    }

    public void ChangeNetSize1()
    {
        change = 1;
    
    }

}
