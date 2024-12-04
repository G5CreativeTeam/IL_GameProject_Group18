using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManualAnimate : MonoBehaviour
{
    public Sprite[] sprites;
   
    [Header("Sprite Properties")]
    public float size = 6.5f;
    public float xPosition = 10;
    public float yPosition = 20;
    [SerializeField] private ToolType activeTool;
    [SerializeField] private GameObject toolObject;

    [Header("Animation")]
    public float animSpeed;
    public float interlude = 0.005f;


    private int index = 0;
    private float opacity = 1;
    // Start is called before the first frame update

    public IEnumerator Animate()
    {
        transform.parent.GetComponent<PlotScript>().toolActive = true;
        transform.localScale = new Vector3(size,size,size);
        transform.localPosition = new Vector3(xPosition,yPosition);
        while (index != sprites.Length) {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[index];
            yield return new WaitForSeconds(animSpeed);
            index++;
        }
        StartCoroutine(Fade());
        StopCoroutine(Animate());
    }

    IEnumerator Fade()
    {
        while (opacity >= 0)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
            opacity -= Time.deltaTime*3;
            
            yield return new WaitForSeconds(interlude);
        }
        UseTool(activeTool);
        transform.parent.GetComponent<PlotScript>().toolActive = false;
        Destroy(gameObject);
        
        
        StopCoroutine(Fade());
        
    }

    public void UseTool(ToolType tool)
    {
        switch (tool)
        {
            case ToolType.shovel:
                transform.parent.GetComponent<PlotScript>().ShovelDrop();
                //toolObject.GetComponent<ShovelScript>().ReturnToPosition();
                return;
            case ToolType.wateringCan:
                transform.parent.GetComponent<PlotScript>().WateringCanDrop();
                //toolObject.GetComponent<WateringCanScript>().ReturnToPosition();
                return;
            case ToolType.fertilizer:
                transform.parent.GetComponent<PlotScript>().FertilizerDrop();
                //toolObject.GetComponent<FertilizerScript>().ReturnToPosition();
                return;
        }
    }

    public enum ToolType
    {
        fertilizer,
        wateringCan,
        shovel
    }
}
