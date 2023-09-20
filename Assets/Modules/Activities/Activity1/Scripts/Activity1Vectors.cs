using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Activity1Vectors : MonoBehaviour
{
    public DraggableVector velocity;
    public DraggableVector centrifugalForce;
    public DraggableVector coriolisForce;

    [Header("UI")]
    public Button fireButton;
    public Button checkButton;
    public Button resetButton;
    public Activity1OmegaSlider omegaSlider;
    public RectTransform winPanel;
    public RectTransform losePanel;

    [Header("Effects")]
    public ParticleSystem particles;
    public SoundEffect successEffect;
    public SoundEffect tryAgainEffect;
    private AudioSource audioSource;
    private bool soundIsOn = true;

    private Vector3 initialVelocityPosition;
    private Vector3 initialVelocityComponents;

    private Vector3 initialCentrifugalPosition;
    private Vector3 initialCentrifugalComponents;

    private Vector3 initialCoriolisPosition;
    private Vector3 initialCoriolisComponents;

    private bool truthIsKnown;
    private Vector3 velocityDirection;
    private Vector3 centrifugalDirection;
    private Vector3 coriolisDirection;

    public static event Action OnAllVectorsCorrect;
    public static event Action OnReset;

    private void Awake()
    {
        if (velocity)
        {
            initialVelocityPosition = velocity.transform.position;
            initialVelocityComponents = velocity.components;
        }
        if (centrifugalForce)
        {
            initialCentrifugalPosition = centrifugalForce.transform.position;
            initialCentrifugalComponents = centrifugalForce.components;
        }
        if (coriolisForce)
        {
            initialCoriolisPosition = coriolisForce.transform.position;
            initialCoriolisComponents = coriolisForce.components;
        }

        if (winPanel)
        {
            // winPanel.gameObject.SetActive(false);
            if (winPanel.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        if (losePanel)
        {
            // losePanel.gameObject.SetActive(false);
            if (losePanel.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        if (particles) particles.gameObject.SetActive(false);

        TryGetComponent(out audioSource);
    }

    private void OnEnable()
    {
        GunFrameSimulation.OnPause += HandleGunFrameSimulationPaused;
    }

    private void OnDisable()
    {
        GunFrameSimulation.OnPause -= HandleGunFrameSimulationPaused;
    }

    public void Reset()
    {
        ResetVector(velocity, initialVelocityPosition, initialVelocityComponents);
        ResetVector(centrifugalForce, initialCentrifugalPosition, initialCentrifugalComponents);
        ResetVector(coriolisForce, initialCoriolisPosition, initialCoriolisComponents);

        SetAllVectorsInteractable();

        if (fireButton) SetInteractability(fireButton, true);
        if (checkButton) SetInteractability(checkButton, false);
        if (resetButton)
        {
            if (resetButton.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
        if (omegaSlider) SetInteractability(omegaSlider, true);

        truthIsKnown = false;

        OnReset?.Invoke();
    }

    public void ResetFromWin()
    {
        ResetVector(velocity, initialVelocityPosition, initialVelocityComponents);
        ResetVector(centrifugalForce, initialCentrifugalPosition, initialCentrifugalComponents);
        ResetVector(coriolisForce, initialCoriolisPosition, initialCoriolisComponents);

        SetAllVectorsInteractable();

        if (fireButton) SetInteractability(fireButton, true);
        if (checkButton) SetInteractability(checkButton, false);
        if (resetButton)
        {
            // resetButton.gameObject.SetActive(false);
            if (resetButton.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
        // if (resetButton) SetInteractability(resetButton, false);
        if (omegaSlider) SetInteractability(omegaSlider, true);

        truthIsKnown = false;

        OnAllVectorsCorrect?.Invoke();
    }

    private void SetInteractability(Button button, bool interactable)
    {
        button.interactable = interactable;
        if (button.TryGetComponent(out CursorHoverUI cursor)) cursor.enabled = interactable;
    }

    private void SetInteractability(Activity1OmegaSlider slider, bool interactable)
    {
        slider.interactable = interactable;
        if (slider.TryGetComponent(out CursorHoverUI cursor)) cursor.enabled = interactable;
    }

    private void CheckForWin(bool velocityCorrect, bool centrifugalCorrect, bool coriolisCorrect)
    {
        bool allCorrect = velocityCorrect & centrifugalCorrect & coriolisCorrect;

        if (!velocityCorrect)
        {
            ResetVector(velocity, initialVelocityPosition, initialVelocityComponents);
        }
        if (!centrifugalCorrect)
        {
            ResetVector(centrifugalForce, initialCentrifugalPosition, initialCentrifugalComponents);
        }
        if (!coriolisCorrect)
        {
            ResetVector(coriolisForce, initialCoriolisPosition, initialCoriolisComponents);
        }

        if (winPanel)
        {
            // winPanel.gameObject.SetActive(allCorrect);
            if (winPanel.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.alpha = allCorrect ? 1 : 0;
                canvasGroup.interactable = allCorrect;
                canvasGroup.blocksRaycasts = allCorrect;
            }
        }

        if (losePanel)
        {
            // losePanel.gameObject.SetActive(!allCorrect);
            if (losePanel.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.alpha = allCorrect ? 0 : 1;
                canvasGroup.interactable = !allCorrect;
                canvasGroup.blocksRaycasts = !allCorrect;
            }
        }

        if (particles) particles.gameObject.SetActive(allCorrect);

        if (audioSource)
        {
            if (allCorrect)
            {
                if (successEffect && soundIsOn) successEffect.Play(audioSource);
            }
            else
            {
                if (tryAgainEffect && soundIsOn) tryAgainEffect.Play(audioSource);
            }
        }
    }

    private void ResetVector(DraggableVector vector, Vector3 initialPosition, Vector3 initialComponents)
    {
        if (vector)
        {
            vector.transform.position = initialPosition;
            vector.components = initialComponents;
            vector.Redraw();

            if (!truthIsKnown)
            {
                vector.useStickyPoint = false;
                vector.useStickyDirections = false;
                vector.stickyDirections = new List<Vector3>();
            }
        }
    }

    public void HandleGunFrameSimulationPaused(Vector3 simPosition, Vector3 bulletPosition, Vector3 bulletVelocity, Vector3 omega)
    {
        velocityDirection = bulletVelocity.normalized;
        centrifugalDirection = (bulletPosition - simPosition).normalized;
        // centrifugalDirection = bulletPosition.normalized;
        coriolisDirection = Vector3.Cross(omega, bulletVelocity).normalized;  // Unity is left-handed
        List<Vector3> stickyDirections = new List<Vector3>
        {
            velocityDirection,
            -velocityDirection,
            centrifugalDirection,
            -centrifugalDirection,
            coriolisDirection,
            -coriolisDirection
        };

        AddStickiness(velocity, bulletPosition, stickyDirections);
        AddStickiness(centrifugalForce, bulletPosition, stickyDirections);
        AddStickiness(coriolisForce, bulletPosition, stickyDirections);

        truthIsKnown = true;
    }

    private void AddStickiness(DraggableVector vector, Vector3 point, List<Vector3> directions)
    {
        if (vector)
        {
            vector.useStickyPoint = true;
            vector.stickyPoint = point;

            vector.useStickyDirections = true;
            vector.stickyDirections = directions;
        }
    }

    public void CheckAnswers()
    {
        if (!truthIsKnown)
        {
            Debug.LogWarning("Activity1Vectors > we don't yet know the truth");
            return;
        }

        velocity.SetInteractable(false);
        centrifugalForce.SetInteractable(false);
        coriolisForce.SetInteractable(false);

        bool velocityCorrect = Vector3.Angle(velocityDirection, velocity.components) == 0;

        bool omegaIsZero = coriolisDirection.magnitude == 0;
        bool centrifugalCorrect;
        bool coriolisCorrect;
        if (omegaIsZero)
        {
            centrifugalCorrect = centrifugalForce.transform.position == initialCentrifugalPosition;
            coriolisCorrect = coriolisForce.transform.position == initialCoriolisPosition;
        }
        else
        {
            centrifugalCorrect = Vector3.Angle(centrifugalDirection, centrifugalForce.components) == 0;
            coriolisCorrect = Vector3.Angle(coriolisDirection, coriolisForce.components) == 0;
        }

        CheckForWin(velocityCorrect, centrifugalCorrect, coriolisCorrect);
    }

    public void SetAllVectorsInteractable()
    {
        velocity.MakeInteractable();
        centrifugalForce.MakeInteractable();
        coriolisForce.MakeInteractable();
    }

    public void SetSoundOn(bool isOn)
    {
        soundIsOn = isOn;
    }
}
