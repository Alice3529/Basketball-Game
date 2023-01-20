using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coordManager : MonoBehaviour
{
    public Transform basket;
    internal Vector2 difference;
    [SerializeField] Vector2 xBoards;
    [SerializeField] Vector2 yBoards;
    [SerializeField] float speed;
    [SerializeField] Transform ballPoint;
    [SerializeField] GameObject basketPrefab;
    internal Vector2 bottomLeftCorner;
    internal Vector2 topRightCorner;
    Vector2 screenCenter;
    float fallOffset;
    [SerializeField] bool actions = false;
    [SerializeField] float bottomLine = 14f;
    public bool canControl = true;
    Vector3 startPos;
    ball1 ball;
    [SerializeField] Transform rightBound;
    [SerializeField] Transform leftBound;
    Trajectory trajectory;
    Camera camera;
    internal rotateBasket rotateBasket;
    internal net net;
    [SerializeField] float wallOffset=0.1f;
    public Transform newBasket;
    public float cameraNotMove = 0.55f;
    public float ballNotMove = 0.4f;


    private void Start()
    {
        Application.targetFrameRate = 60;
        camera = Camera.main;
        ball = FindObjectOfType<ball1>();
        ChangeCorners();
        SetColliders();
        SetTrajectory();
        
    }

    public void SetTrajectory()
    {
        trajectory = basket.GetComponentInChildren<Trajectory>();
        rotateBasket = basket.gameObject.GetComponent<rotateBasket>();
        net = basket.GetComponentInChildren<net>();

    }
    private void SetColliders()
    {
        Vector2 rightBoundSize = rightBound.GetComponent<BoxCollider2D>().bounds.size / 2;
        rightBound.transform.position = new Vector2(topRightCorner.x + rightBoundSize.x, screenCenter.y);
        Vector2 leftBoundSize = leftBound.GetComponent<BoxCollider2D>().bounds.size / 2;
        leftBound.transform.position = new Vector2(bottomLeftCorner.x - leftBoundSize.x, screenCenter.y);
    }

    public void ChangeCorners()
    {
        bottomLeftCorner = camera.ScreenToWorldPoint(Vector2.zero);
        topRightCorner = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        screenCenter = camera.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
    }

    public float GetBottom()
    {
        return bottomLeftCorner.y + fallOffset;
    }

    void Update()
    {
        if (canControl==false) { return; }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startPos = Camera.main.ScreenToWorldPoint(touch.position);
                DetectNavigationBar(touch);
            }


            if (touch.phase == TouchPhase.Moved && actions==true)
            {
                CalculateDifference(touch);
                Actions();

            }

            if (touch.phase == TouchPhase.Ended && actions==true)
            {
                EndActions();
            }

        }
    }

    private void EndActions()
    {
        if (ball.inBasket == true)
        {
            net.StartSize();
            ball.Shoot(difference);
            difference = Vector2.zero;

        }
        trajectory.DeleteTrajectory();
    }

    private void Actions()
    {
        rotateBasket.Rotate(difference);
        net.ChangeNetSize(difference.magnitude);
        trajectory.CreateTrajectory(difference, basket.position);
        trajectory.TrajectoryColor(difference.magnitude);
    }

    private void CalculateDifference(Touch touch)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
        difference = pos - startPos;
        difference = new Vector3(Mathf.Clamp(difference.x, xBoards.x, xBoards.y), Mathf.Clamp(difference.y, yBoards.x, yBoards.y), 0f);
    }

    private void DetectNavigationBar(Touch touch)
    {
        actions = (touch.position.y <= Screen.height / bottomLine) ? false : true; 

    }

    public void CreateNewBasket()
    {
        float xPos, yPos;
        CalculateNewBasketPos(out xPos, out yPos);
        newBasket = Instantiate(basket, new Vector2(xPos, yPos), Quaternion.identity);
        newBasket.name = "Basket";
        rotateBasket.DisactiveBasket();
    }

    private void CalculateNewBasketPos(out float xPos, out float yPos)
    {
        if (basket.position.x < screenCenter.x)
        {
            xPos = Random.Range(screenCenter.x + basket.transform.localScale.x, topRightCorner.x - basket.transform.localScale.x);
            float xDistance = topRightCorner.x - (xPos + basket.transform.GetChild(0).transform.localScale.x / 2); // distance between basket edge and wall
            float ballDiameter = ball.transform.localScale.y * 2;
            if (xDistance < ballDiameter + wallOffset)
            {
                xPos = topRightCorner.x - basket.transform.GetChild(0).transform.localScale.x / 2 - ballDiameter - wallOffset;
            }
        }
        else
        {
            xPos = Random.Range(bottomLeftCorner.x + basket.transform.localScale.x, screenCenter.x - basket.transform.localScale.x);
            float xDistance = xPos-basket.transform.GetChild(0).transform.localScale.x / 2 - bottomLeftCorner.x; // distance between basket edge and wall
            float ballDiameter = ball.transform.localScale.y * 2;
            if (xDistance<ballDiameter + wallOffset)
            {
                xPos = basket.transform.GetChild(0).transform.localScale.x / 2 + ballDiameter + wallOffset + bottomLeftCorner.x;
            }
        }

        yPos = Random.Range(basket.transform.position.y + ((topRightCorner.y - bottomLeftCorner.y) / 6), basket.transform.position.y + ((topRightCorner.y - bottomLeftCorner.y) / 5));
    }
}
