using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPlacement : MonoBehaviour
{
    private Transform pivotPoint; // The pivotPoint object to rotate around
    private bool isPlaced = false;
    [HideInInspector]
    public Transform standPoint;
    [HideInInspector]
    public bool IsNearStand = false;
    [HideInInspector]
    public bool canPlayerPickup = false;
    public SpriteRenderer rangeImage;
    [HideInInspector]
    public Stand stand;
    private void Start()
    {
        pivotPoint = transform.parent;
    }
    void FixedUpdate()
    {
        if (pivotPoint == null)
        { return; }

        if (!isPlaced)
        {
            // Get the mouse position in world space
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            // Calculate the direction from the pivotPoint to the mouse position
            Vector3 direction = mousePosition - pivotPoint.position;

            // Calculate the angle between the direction and the pivotPoint
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply the rotation to the child object
            transform.position = pivotPoint.position + direction.normalized * Vector3.Distance(transform.position, pivotPoint.position);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isPlaced && IsNearStand&& !stand.isOccupied&& GameManager.instance.isMirrorInHand)
            {
                rangeImage.enabled = false;
                transform.parent = standPoint.parent;
                this.isPlaced = true;
                transform.position = standPoint.position;
                stand.isOccupied= true;
                GameManager.instance.MirrorInHand.layer = 6;
                GameManager.instance.isMirrorInHand = false;
                GameManager.instance.MirrorInHand = null;
                GameManager.instance.mirrorCount--;
            }
            else if (isPlaced && canPlayerPickup&& stand.isOccupied&& !GameManager.instance.isMirrorInHand)
            {
                rangeImage.enabled = true;
                transform.parent = pivotPoint;
                GameManager.instance.isMirrorInHand = true;
                GameManager.instance.MirrorInHand = this.gameObject;
                GameManager.instance.MirrorInHand.layer = 0;
                GameManager.instance.mirrorCount++;
                stand.isOccupied = false;
                // Calculate the direction from the pivotPoint to the mirror's current position
                Vector3 direction = (transform.position - pivotPoint.position).normalized;

                // Set the new position to be 0.75 units away from the pivotPoint in the calculated direction
                transform.position = pivotPoint.position + direction * 3f;

               this.isPlaced = false;
            }
        }
    }
}
