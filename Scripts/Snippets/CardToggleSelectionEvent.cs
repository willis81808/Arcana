using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardToggleSelectionEvent : MonoBehaviour
{
    private CardVisuals cardVisuals;

    public UnityEvent onCardSelected, onCardDeselected;

    void Awake()
    {
        cardVisuals = GetComponentInParent<CardVisuals>();
        cardVisuals.toggleSelectionAction += HandleVisualToggle;
    }
    
    void OnDestroy()
    {
        cardVisuals.toggleSelectionAction -= HandleVisualToggle;
    }

    private void HandleVisualToggle(bool active)
    {
        if (active)
        {
            onCardSelected?.Invoke();
        }
        else
        {
            onCardDeselected?.Invoke();
        }
    }
}
