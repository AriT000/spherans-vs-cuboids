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
    }
}
