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
    public Slider omegaSlider;
    public RectTransform winPanel;
    public RectTransform losePanel;

    [Header("Celebration")]
    public ParticleSystem particles;

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

        if (winPanel) winPanel.gameObject.SetActive(false);
        if (losePanel) losePanel.gameObject.SetActive(false);

        if (particles) particles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GunFrameSimulation.OnPause += HandleGunFrameSimulationPaused;
    }

    private void OnDisable()
    {
        GunFrameSimulation.OnPause -= HandleGunFrameSimulationPaused;
    }

    public void ResetFromWin()
    {
        ResetVector(velocity, initialVelocityPosition, initialVelocityComponents);
        ResetVector(centrifugalForce, initialCentrifugalPosition, initialCentrifugalComponents);
        ResetVector(coriolisForce, initialCoriolisPosition, initialCoriolisComponents);

        if (fireButton)
        {
            fireButton.interactable = true;
            if (fireButton.TryGetComponent(out CursorHoverUI cursor))
            {
                cursor.enabled = true;
            }
        }
        if (checkButton)
        {
            checkButton.interactable = false;
            if (checkButton.TryGetComponent(out CursorHoverUI cursor))
            {
                cursor.enabled = false;
            }
        }
        if (omegaSlider)
        {
            omegaSlider.interactable = true;
            if (omegaSlider.TryGetComponent(out CursorHoverUI cursor))
            {
                cursor.enabled = true;
            }
        }

        truthIsKnown = false;

        OnAllVectorsCorrect?.Invoke();
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

        if (winPanel) winPanel.gameObject.SetActive(allCorrect);
        if (losePanel) losePanel.gameObject.SetActive(!allCorrect);

        if (particles) particles.gameObject.SetActive(allCorrect);
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
        // centrifugalDirection = (bulletPosition - simPosition).normalized;
        centrifugalDirection = bulletPosition.normalized;
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

        // TODO What if Omega = 0?

        bool velocityCorrect = Vector3.Angle(velocityDirection, velocity.components) == 0;
        bool centrifugalCorrect = Vector3.Angle(centrifugalDirection, centrifugalForce.components) == 0;
        bool coriolisCorrect = Vector3.Angle(coriolisDirection, coriolisForce.components) == 0;

        CheckForWin(velocityCorrect, centrifugalCorrect, coriolisCorrect);
    }
}
