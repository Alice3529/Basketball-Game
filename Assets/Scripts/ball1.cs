using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ball1 : MonoBehaviour
{
    [SerializeField] float velocity = 10f;
    coordManager coordManager;
    Rigidbody2D rigidbody2D;
    public Action<Vector3, float> camUp;
    public Action<Vector3, int> camBottom;
    public bool inBasket = false;
    [SerializeField] float rotationSpeed;
    [SerializeField] float moveSpeed;
    Vector3 cameraPos;
    [SerializeField] float upGravity = 2f;
    [SerializeField] float bottomGravity = 3.6f;
    [SerializeField] ParticleSystem smoke;
    [SerializeField] ParticleSystem fire;
    counter counter;
    bool stopMove = false;

         

    private void Start()
    {
        coordManager=FindObjectOfType<coordManager>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        cameraPos = Camera.main.transform.position;
        counter = FindObjectOfType<counter>();
 
    }

    private void Update()
    {
        if (transform.position.y < coordManager.GetBottom())
        {
            InBasket1();
            counter.ResetCounter();
        }
        if (transform.position.y <= (coordManager.rotateBasket.ballPos.position.y)  && rigidbody2D.velocity.y<0f)
        {
            camBottom.Invoke(cameraPos, 1);
        }
        if (rigidbody2D.velocity.y < 0f)
        {
            rigidbody2D.gravityScale = bottomGravity;
        }

    }

    private void FixedUpdate()
    {
        RotateBall();
        MoveBallToStartPoint();

    }

    private void MoveBallToStartPoint()
    {
        if (inBasket == true && coordManager.canControl == false)
        {
            Vector3 newPos = Vector3.MoveTowards(rigidbody2D.position, coordManager.rotateBasket.bottomBallPoint.position, Time.fixedDeltaTime * moveSpeed);
            rigidbody2D.MovePosition(newPos);
            if (Vector3.Distance(rigidbody2D.position, coordManager.rotateBasket.bottomBallPoint.position) < 0.01f && stopMove == false)
            {
                rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                coordManager.canControl = true;
                stopMove = true;
            }
        }
    }

    private void RotateBall()
    {
        if (inBasket == false)
        {
            rigidbody2D.MoveRotation(rigidbody2D.rotation + rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void Shoot(Vector3 difference)
    {
        if (difference.magnitude < coordManager.ballNotMove) { return; }
        ResetBall();
        difference = BallShoot(difference);
        CameraUp(difference);

    }

    private Vector3 BallShoot(Vector3 difference)
    {
        difference = new Vector3(-difference.x, -difference.y);
        Vector3 normalizedDir = difference.normalized;
        Vector3 force = normalizedDir * velocity * difference.magnitude;
        rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        return difference;
    }

    private void ResetBall()
    {
        stopMove = false;
        transform.parent = null;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        inBasket = false;
    }

    private void CameraUp(Vector3 difference)
    {
        Vector3 basketDifference = coordManager.newBasket.position - coordManager.basket.transform.position;
        camUp.Invoke(basketDifference, difference.magnitude);
        coordManager.canControl = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "basket"&& inBasket==false)
        {
            Transform newBasket = collision.gameObject.transform.parent;
            inBasket = true;
            if (coordManager.basket != newBasket)
            {
                CalculateCameraOffset(newBasket);
                Destroy(coordManager.basket.gameObject);
                coordManager.basket = newBasket;
                coordManager.rotateBasket.ActiveBasket();
                coordManager.CreateNewBasket();
                coordManager.SetTrajectory();
                coordManager.ChangeCorners();
                counter.UpdateCounter();
                ParticleCreator();

            }
            InBasket();
        }
    }


    private void CalculateCameraOffset(Transform newBasket)
    {
        Vector3 basketDifference = newBasket.transform.position - coordManager.basket.position;
        Vector3 idealEndPos = cameraPos + basketDifference;
        cameraPos = idealEndPos;
        Down();
    }

    public void Down()
    {
        camBottom.Invoke(cameraPos, 2);

    }

    public void InBasket()
    {
       rigidbody2D.rotation = 0;
       coordManager.basket.transform.rotation = Quaternion.identity;
       transform.rotation = Quaternion.identity;
       rigidbody2D.velocity = Vector3.zero;
       transform.SetParent(coordManager.basket.transform, true);
       rigidbody2D.gravityScale = upGravity;
       rigidbody2D.isKinematic = true;
       coordManager.net.ChangeNetSize1();

    }

    private void ParticleCreator()
    {
        StopParticles();
        int probability = UnityEngine.Random.Range(0, 11);
        if (probability % 5 == 0)
        {
            smoke.Play();
        }
        else if (probability % 7 == 0)
        {
            fire.Play();
        }
    }

    public void InBasket1()
    {
        StopParticles();
        coordManager.basket.transform.rotation= Quaternion.identity;
        transform.position = coordManager.rotateBasket.ballPos.position;
        rigidbody2D.velocity = new Vector2(0f, -0.5f);
    }

    private void StopParticles()
    {
        smoke.Stop();
        fire.Stop();
    }
}
