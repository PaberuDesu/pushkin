using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AppCloseController : MonoBehaviour
{
    [SerializeField] private Animator escAnimator;
    [SerializeField] private InfoOpener info;
    [SerializeField] private SettingsManager settings;

    private const float holdDuration = 1f;
    private float holdTime = 0f;
    private bool isHolding = false;
    private Coroutine escCoroutine;
    private Keyboard keyboard;

    private void Start() => keyboard = Keyboard.current;

    private void Update() => HandleEscapeInput();

    private void HandleEscapeInput()
    {
        if (keyboard == null) return;

        if (keyboard.escapeKey.wasPressedThisFrame)
            StartHolding();

        if (keyboard.escapeKey.wasReleasedThisFrame)
            CancelHolding();

        if (isHolding && keyboard.escapeKey.isPressed)
        {
            holdTime += Time.deltaTime;
            if (holdTime >= holdDuration)
                QuitApplication();
        }
    }

    private void StartHolding()
    {
        float animDuration = 0;
        if (info.IsShown)
        {
            info.ShowInfo(null);
            animDuration = InfoOpener.duration;
        }
        else if (SettingsManager.IsShown)
        {
            settings.ChangeSettingsWindowState(false);
            animDuration = settings.Duration;
        }

        escCoroutine = StartCoroutine(StartEscaping(animDuration));
    }

    private IEnumerator StartEscaping(float animDuration)
    {
        yield return new WaitForSeconds(animDuration);
        isHolding = true;
        if (isHolding) escAnimator.SetBool("Escaping", true);
    }

    private void CancelHolding()
    {
        if (escCoroutine != null) StopCoroutine(escCoroutine);
        isHolding = false;
        holdTime = 0f;
        escAnimator.SetBool("Escaping", false);
    }

    private void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private void OnDestroy() => StopAllCoroutines();
}