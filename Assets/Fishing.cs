using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class Fishing : MonoBehaviour
{
    public bool isEquipped;
    public bool isFishingAvailable;

    public bool isCasted;
    public bool isPulling;

    Animator animator;
    public GameObject baitPrefab;
    public GameObject endof_of_rope;  // --- > IF USING ROPE
    public GameObject start_of_rope;   // --- > IF USING ROPE   
    public GameObject start_of_rod;    // --- > IF USING ROPE   

    Transform baitPosition;

    private void Start()
    {
        animator = GetComponent<Animator>();
        isEquipped = true;
    }



    void Update()
    {
        if (isEquipped)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {

                if (hit.collider.CompareTag("FishingArea"))
                {
                    isFishingAvailable = true;

                    if (Input.GetMouseButtonDown(0) && !isCasted && !isPulling)
                    {
                        StartCoroutine(CastRod(hit.point));
                    }
                }
                else
                {
                    isFishingAvailable = false;
                }

            }
            else
            {
                isFishingAvailable = false;
            }
        }

        // --- > IF USING ROPE < --- //
        if (isCasted || isPulling)
        {
            if (start_of_rope != null && start_of_rod != null && endof_of_rope != null)
            {
                start_of_rope.transform.position = start_of_rod.transform.position;

                if (baitPosition != null)
                {
                    endof_of_rope.transform.position = baitPosition.position;
                }
            }
            else
            {
                Debug.Log("MISSING ROPE REFERENCES");
            }
        }

        if (isCasted && Input.GetMouseButtonDown(1))
        {
            PullRod();
        }
    }


    IEnumerator CastRod(Vector3 targetPosition)
    {
        isCasted = true;
        animator.SetTrigger("Cast");

        // Create a delay between the animation and when the bait appears in the water
        yield return new WaitForSeconds(1f);

        GameObject instantiatedBait = Instantiate(baitPrefab);
        instantiatedBait.transform.position = targetPosition;

        baitPosition = instantiatedBait.transform;

        // ---- > Start Fish Bite Logic
    }

    private void PullRod()
    {
        animator.SetTrigger("Pull");
        isCasted = false;
        isPulling = true;

        // ---- > Start Minigame Logic
    }
}