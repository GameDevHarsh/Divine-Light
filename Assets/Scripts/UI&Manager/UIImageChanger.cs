using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIImageChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite hoverSprite; // The sprite to switch to on hover
    private Sprite originalSprite; // The original sprite of the UI Image
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            originalSprite = image.sprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null && hoverSprite != null)
        {
            image.sprite = hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null)
        {
            image.sprite = originalSprite;
        }
    }
}
