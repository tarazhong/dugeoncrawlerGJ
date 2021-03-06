using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
#pragma warning disable 0649
    [HideInInspector] public bool CanMove;
    private IEnumerator moveCoroutine;
    private GameObject hitFwrdObject;
    public float lookSpeed = 0.05f;
    private Vector2 rotation = Vector2.zero;

    public static PlayerController instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        CanMove = true;
    }

    private void FixedUpdate()
    {
        if (CanMove)
        {
            float movmentLeftRight = Input.GetAxis("Horizontal");
            float movmentFowrdBackWard = Input.GetAxis("Vertical");
            float movmentRotate = Input.GetAxis("Rotate");

            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            bool canMoveForward = true;
            bool canMoveBackward = true;
            Vector3 right = transform.TransformDirection(Vector3.right);
            bool canMoveRight = true;
            bool canMoveLeft = true;
            RaycastHit forwardHit;

            if (Physics.Raycast(transform.position, fwd, out forwardHit, 1))
            {
                canMoveForward = false;
                if (hitFwrdObject != forwardHit.collider.gameObject)
                {
                    GameManager.instance.CheckInteraction(forwardHit.collider.gameObject);
                    hitFwrdObject = forwardHit.collider.gameObject;
                }
            }
            else
            {
                GameManager.instance.NoInteraction();
                hitFwrdObject = null;
            }

            if (Physics.Raycast(transform.position, -fwd, 1))
                canMoveBackward = false;

            if (Physics.Raycast(transform.position, right, 1))
                canMoveRight = false;
            if (Physics.Raycast(transform.position, -right, 1))
                canMoveLeft = false;

            if (movmentFowrdBackWard > 0 && canMoveForward)
            {
                moveCoroutine = Move(1, 0);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentFowrdBackWard < 0 && canMoveBackward)
            {
                moveCoroutine = Move(-1, 0);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentLeftRight > 0 && canMoveRight)
            {
                moveCoroutine = Move(0,1);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentLeftRight < 0 && canMoveLeft)
            {
                moveCoroutine = Move(0, -1);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentRotate > 0)
            {
                moveCoroutine = Rotate(1);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentRotate < 0)
            {
                moveCoroutine = Rotate(-1);
                StartCoroutine(moveCoroutine);
            }

            //rotation.y += Input.GetAxis("Mouse X");
            //rotation.x += -Input.GetAxis("Mouse Y");
            //rotation.x = Mathf.Clamp(rotation.x, -10f, 10f);
            //rotation.y = Mathf.Clamp(rotation.y, -7, 7f);
            //Camera.main.transform.localRotation = Quaternion.Euler(rotation.x * lookSpeed, rotation.y * lookSpeed, 0);
        }
    }

    private IEnumerator Move(int _x,int _z)
    {
        CanMove = false;
        float t = 0.0f;
        // animate the position of the game object...
       
        Vector3 startPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 finalPos = startPos + transform.forward * _x + transform.right * _z;
        finalPos.x = (int)Mathf.Round(finalPos.x); // prevent float approx
        finalPos.z = (int)Mathf.Round(finalPos.z);
        while (t <= 1.0f)
        {
            // .. and increase the t interpolater
            t += Time.deltaTime;
            float x = Mathf.Lerp(startPos.x, finalPos.x, t);
            float z = Mathf.Lerp(startPos.z, finalPos.z, t);
            transform.position = new Vector3(x, 0, z);
            yield return new WaitForEndOfFrame();
        }
        if (t > 1.0f)
        {
            transform.position = finalPos;
        }
        CanMove = true;
    }
    private IEnumerator Rotate(int _x)
    {
        CanMove = false;
        float t = 0.0f;
        // animate the position of the game object...
        Quaternion startRot = transform.rotation;
        Quaternion finalRot = startRot * Quaternion.Euler(0, 90 * _x, 0);
        while (t <= 1.0f)
        {
            // .. and increase the t interpolater
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, finalRot, t);
            yield return new WaitForEndOfFrame();
        }
        if (t > 1.0f)
        {
            transform.rotation = finalRot;
        }
        CanMove = true;
    }
}
