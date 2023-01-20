using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Trajectory : MonoBehaviour
{
    [SerializeField] GameObject dot;
    [SerializeField] int amount;
    [SerializeField] float force;
    List<GameObject> dots = new List<GameObject>();
    [SerializeField] float maxDif = 0.9f;
    [SerializeField] float speed = 2f;
    [SerializeField] float m;
    Vector3 lastPoint;
    bool firstPoint = false;
    Vector3 pointBetween;
    coordManager coordManager;
    float invisible = 0f;
    float completeVisible = 0.8f;
    float bottomThreshold = 0.4f;
    float topThreshold = 0.6f;




    private void Start()
    {
        coordManager=FindObjectOfType<coordManager>();

        for (int i = 0; i < amount; i++)
        {
           GameObject newDot=Instantiate(dot, transform);
           dots.Add(newDot);
           newDot.SetActive(false);

        }
    }

    public void DeleteTrajectory()
    {
        for (int i = 0; i < amount; i++)
        {
            dots[i].SetActive(false);
        }
    }

    public void TrajectoryColor(float val)
    {
        if (val < bottomThreshold)
        {
            val = invisible;
        }
        else if (val > topThreshold)
        {
            val = completeVisible;
        }
        else
        {
            val = Remap(val, bottomThreshold, topThreshold, invisible, completeVisible);
        }

        Repaint(val);
    }

    private void Repaint(float val)
    {
        for (int i = 0; i < amount; i++)
        {
            Color color = dots[i].GetComponent<SpriteRenderer>().color;
            dots[i].GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, val);

        }
    }

    float Remap(float source, float sourceFrom, float sourceTo, float targetFrom, float targetTo)
    {
        return targetFrom + (source - sourceFrom) * (targetTo - targetFrom) / (sourceTo - sourceFrom);
    }



    public void CreateTrajectory(Vector3 difference, Vector3 pos)
    {
        firstPoint = false;

        for (int i = 0; i < amount; i++)
        {
            dots[i].transform.position = pointPosition((i+1) * m
                , difference, pos);
            dots[i].SetActive(true);
        }
    }
    
    Vector2 pointPosition(float t, Vector3 difference, Vector2 position)
    {
        Vector2 dir = new Vector2(-difference.x, -difference.y);
        Vector2 currentPointPos = (Vector2)transform.position + (dir.normalized * difference.magnitude * speed * t) + 0.5f * Physics2D.gravity * (t * t);
        currentPointPos = Reflect(currentPointPos);
        return currentPointPos;
    }

    private Vector2 Reflect(Vector2 currentPointPos)
    {
        if (currentPointPos.x < coordManager.bottomLeftCorner.x)
        {
            currentPointPos = CalculateReflection(currentPointPos, coordManager.bottomLeftCorner.x, -Vector3.right);

        }
        else if (currentPointPos.x > coordManager.topRightCorner.x)
        {
            currentPointPos = CalculateReflection(currentPointPos, coordManager.topRightCorner.x, Vector3.right);

        }
        else
        {
            lastPoint = currentPointPos;
        }

        return currentPointPos;
    }

    private Vector2 CalculateReflection(Vector2 currentPointPos, float screenBoardX, Vector3 normal)
    {
        if (firstPoint == false)
        {
            float yPoint = IntersactionCalculation(currentPointPos.x, currentPointPos.y, lastPoint.x, lastPoint.y, screenBoardX, coordManager.bottomLeftCorner.y, screenBoardX, coordManager.topRightCorner.y);
            pointBetween = new Vector3(screenBoardX, yPoint, 0);
            firstPoint = true;
        }
        Vector3 direction = currentPointPos - new Vector2(pointBetween.x, pointBetween.y);
        currentPointPos = pointBetween + Vector3.Reflect(direction, normal);
        return currentPointPos;
    }


    private float IntersactionCalculation(float x1, float y1, float x2, float y2, float x3, float y3,float x4, float y4)
    {
        float up = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
        return up;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(lastPoint, lastPoint-Vector3.right*3f);
    }

    private void OnDestroy()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
 