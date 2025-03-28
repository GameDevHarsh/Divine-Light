using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerVisibility : MonoBehaviour
{
    public SpriteRenderer playerSprite;
    public LayerMask wallLayer;
    // Start is called before the first frame update

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            playerSprite.sortingOrder = 0;
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            playerSprite.sortingOrder = 1 ;
        }
    }
}
