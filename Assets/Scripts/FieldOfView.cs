using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0, 360)]
    public float angle;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    // The AI script of this unit.
    private AIUnit unit;

    private List<GameObject> objectsInView;

    private void Start()
    {
        unit = GetComponent<AIUnit>();
        objectsInView = new List<GameObject>();
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        objectsInView.Clear();
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);

        foreach (Collider2D collider in rangeChecks)
        {
            if (collider.gameObject != transform.parent.gameObject)
            {
                objectsInView.Add(collider.gameObject);
            }            
        }
    }

    public List<GameObject> GetObjectsInView()
    {
        return objectsInView;
    }

  
}