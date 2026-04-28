using UnityEngine;
using UnityEngine.UI;

public class ImageScroller : MonoBehaviour
{


    private RawImage img;
    [SerializeField] private float x;
    [SerializeField] private float y;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        img = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
         img.uvRect = new Rect(img.uvRect.position + new Vector2(x,y) * Time.deltaTime, img.uvRect.size);
        //reset the position to avoid overflow
         if(img.uvRect.x > 1)
        {
            img.uvRect = new Rect(new Vector2(0,0), img.uvRect.size);
        }
    }
}
