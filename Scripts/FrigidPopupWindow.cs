#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public abstract class FrigidPopupWindow : PopupWindowContent
{
    public static void Show(Rect activatorRect, FrigidPopupWindow popupWindow)
    {
        PopupWindow.Show(activatorRect, popupWindow);
    }

    public override void OnGUI(Rect rect)
    {
        Draw();
    }

    public override void OnOpen()
    {
        Opened();
        base.OnOpen();
    }

    public override void OnClose()
    {
        Closed();
        base.OnClose();
    }

    protected virtual void Opened() { }

    protected virtual void Closed() { }

    protected abstract void Draw();
}
#endif
