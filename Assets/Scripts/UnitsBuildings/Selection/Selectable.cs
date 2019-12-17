using QuickOutline;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/**
 * A kijelölendő cuccra megy
 */
public class Selectable : MonoBehaviour, IEventSystemHandler, ISelectable
{
    #region Publikus változók

    [Range(0f, 6f)]
    public float DefaultOutlineWidth;

    [Tooltip("Kijelölés kezdetekor meghívódó custom callbackek.")]
    public UnityEvent OnSelectStartEvent;
    
    [Tooltip("Kijelölés végekor meghívódó custom callbackek.")]
    public UnityEvent OnSelectEndEvent;
    
    [Tooltip("Elsődleges action kattintáskor meghívódó custom callbackek.")]
    public UnityEvent OnActionEvent;

    #endregion

    #region Privát változók

    // TODO: melyik játékos által van kijelölve?
    private bool _selected = false;

    #endregion

    #region Main

    #endregion

    #region Selection

    public void OnSelectStart ()
    {
        try
        {
            _selected = true;
        }
        finally
        {
            OnSelectStartEvent.Invoke();
        }
    }

    public void OnSelectEnd ()
    {
        try
        {
            _selected = false;
        }
        finally
        {
            OnSelectEndEvent.Invoke();
        }
    }

    public void OnAction()
    {
        OnActionEvent.Invoke();
    }
    #endregion
}