using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class rotateBasket : MonoBehaviour
{
    [SerializeField] GameObject trajectory;
    public Transform bottomBallPoint;
    public Transform ballPos;


    public void Rotate(Vector3 difference)
    {
        float angle = Mathf.Atan2(-difference.y, -difference.x) * Mathf.Rad2Deg;
        Debug.Log(angle);
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    public void DisactiveBasket()
    {
        if (this.gameObject != FindObjectOfType<coordManager>().basket)
        {
            trajectory.SetActive(false);
        }
    }

    public void ActiveBasket()
    {
        trajectory.SetActive(true);
    }
}
