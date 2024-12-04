using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShovelScript : MonoBehaviour, IPointerClickHandler
{
    public GameObject eventSystem;
    public int price = 0;
    [SerializeField] private KeyCode shortcutKey = KeyCode.W;
    public GameObject shovelAnimationObject;

    [HideInInspector] public bool availableToClick;

    private bool isFollowingMouse = false;
    private CanvasGroup canvasGroup;
    private StatsScript money;

    private Vector3 initialPosition; // Store the initial position of the fertilizer

    private void Start()
    {
        availableToClick = true;
        canvasGroup = GetComponent<CanvasGroup>();
        money = eventSystem.GetComponent<StatsScript>();
        initialPosition = transform.position; // Save the initial position
    }

    private void Update()
    {
        if(LevelProperties.Instance.isCarryingObject && LevelProperties.Instance.objectCarried == gameObject && Input.GetKeyDown(shortcutKey))
        {
            ReturnToPosition();
        } else if (Input.GetKeyDown(shortcutKey) && !LevelProperties.Instance.isCarryingObject && availableToClick)
        {
            Debug.Log("Check");
            AttemptPickup();
        }
        if (isFollowingMouse)
        {
            FollowMouse();
        }

        if (availableToClick)
        {
            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
    }

    private void FollowMouse()
    {
        // Make the fertilizer follow the mouse position
        transform.position = Input.mousePosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (!LevelProperties.Instance.isCarryingObject && !isFollowingMouse)
        {
            AttemptPickup();

        }
        else
        {
            DropShovel();

        }
    }

    private void AttemptPickup()
    {
        if (money.moneyAvailable >= price)
        {
            isFollowingMouse = true;
            LevelProperties.Instance.isCarryingObject = true;
            LevelProperties.Instance.objectCarried = gameObject;
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = true; // Allow interaction during dragging
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    private void DropShovel()
    {
        Debug.Log(initialPosition);
        // Convert mouse position to world position
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Perform a 2D raycast
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            // Check if the hit object has the PlotScript component
            PlotScript plot = hit.collider.GetComponent<PlotScript>();
            if (plot != null && plot.hasPlant && !plot.GetComponent<PlotScript>().toolActive)
            {

                LevelProperties.Instance.isCarryingObject = false;

                // Reset dragging state
                isFollowingMouse = false;
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;

                GameObject animation = Instantiate(shovelAnimationObject, plot.gameObject.transform);
                StartCoroutine(animation.GetComponent<ToolManualAnimate>().Animate());
                
                availableToClick = false;
            } else
            {
                ReturnToPosition();
            }

        } else
        {
            ReturnToPosition();
        }

        // Reset the fertilizer to its original position if not dropped on a valid plot
        ReturnToPosition();

    }

    public void ReturnToPosition()
    {
        transform.position = initialPosition;
        isFollowingMouse = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        LevelProperties.Instance.isCarryingObject = false;
        LevelProperties.Instance.objectCarried = null;
        availableToClick = true;
    }

}