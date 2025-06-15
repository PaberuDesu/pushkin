using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoOpener : MonoBehaviour
{
    [SerializeField] private ImageCarousel carousel;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text info;
    [SerializeField] private WhiteAndBlackLists whiteBlackLists;
    private RectTransform VisualMover;
    private Coroutine moveCoroutine;
    private const float moveOffset = 0.125f;
    [HideInInspector]
    public bool IsShown { get; private set; } = false;
    private const float clickTimeTreshold = 0.2f;
    public const float duration = 0.5f;
    private float clickStartTimeMarker;

    private void Start()
    {
        VisualMover = transform as RectTransform;
        VisualMover.anchoredPosition = Vector2.right * TargetX(IsShown);
        foreach (Place place in Graph.Places)
            place.markerRect.GetComponent<Button>().onClick.AddListener(() => { ShowInfo(place); });
    }

    private float TargetX(bool isShownNow) =>
        moveOffset * VisualMover.rect.width * (isShownNow ? -1 : 1);

    public void OnPointerDownOnMap() => clickStartTimeMarker = Time.realtimeSinceStartup;

    public void OnPointerUpFromMap()
    {
        if (Time.realtimeSinceStartup - clickStartTimeMarker < clickTimeTreshold)
            ShowInfo(null);
    }

    public void ShowInfo(Place place)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        if (IsShown != (place != null))
            IsShown = !IsShown;

        if (IsShown && place != null)
        {
            carousel.Initialize(place.Images);
            title.text = place.Name;
            info.text = place.Description;
            whiteBlackLists.Initialize(place);
        }

        moveCoroutine = StartCoroutine(SmoothMove(TargetX(IsShown)));
    }

    private IEnumerator SmoothMove(float targetX)
    {
        Vector2 startPos = VisualMover.anchoredPosition;
        Vector2 endPos = Vector2.right * targetX;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            VisualMover.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        VisualMover.anchoredPosition = endPos;
        moveCoroutine = null;
    }

    private void OnDestroy() => StopAllCoroutines();
}
