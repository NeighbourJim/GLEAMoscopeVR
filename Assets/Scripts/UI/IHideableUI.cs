using UnityEngine;

public interface IHideableUI
{
    CanvasGroup CanvasGroup { get; }
    Collider Collider { get; }

    void SetVisibleAndInteractableState(bool visible);
}
