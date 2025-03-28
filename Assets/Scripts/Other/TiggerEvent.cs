using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TiggerEvent : MonoBehaviour
{
    private bool IsPlayerInRange = false;
    private SpriteRenderer img;
    private void Start()
    {
        img=GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if(IsPlayerInRange)
        {
            transform.parent.GetComponent<MirrorControl>().RotateMirror();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IsPlayerInRange = true;
            transform.parent.GetComponent<MirrorPlacement>().canPlayerPickup = true;
        }
        if (collision.CompareTag("Stand"))
        {
            transform.parent.GetComponent<MirrorPlacement>().IsNearStand = true;
            transform.parent.GetComponent<MirrorPlacement>().standPoint = collision.transform;
            img.color = new Color(0, 1, 0, 0.25f);
        }
        if (collision.GetComponent<Stand>() != null)
        {
            transform.parent.GetComponent<MirrorPlacement>().stand = collision.GetComponent<Stand>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IsPlayerInRange = false;
            transform.parent.GetComponent<MirrorPlacement>().canPlayerPickup = false;
        }
        if (collision.CompareTag("Stand"))
        {
            transform.parent.GetComponent<MirrorPlacement>().IsNearStand = false;
            transform.parent.GetComponent<MirrorPlacement>().standPoint = null;
            img.color = new Color(1, 0, 0, 0.25f);
        }
    }
}
