#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public abstract class FrigidPopup : PopupWindowContent
{
    private Vector2 size;

    public static void Show(Rect activatorRect, FrigidPopup popup)
    {
        popup.size = new Vector2(Screen.width / 2, Screen.height / 2);
        PopupWindow.Show(activatorRect, popup);
    }

    public override Vector2 GetWindowSize()
    {
        return this.size;
    }

    public override void OnGUI(Rect rect)
    {
        this.Draw();
        if (GUI.Button(new Rect(Vector2.zero, this.editorWindow.position.size), "", new GUIStyle()))
        {
            GUI.FocusControl(null);
        }
    }

    public override void OnOpen()
    {
        this.Opened();
        base.OnOpen();
    }

    public override void OnClose()
    {
        this.Closed();
        base.OnClose();
    }

    protected virtual void Opened() { }

    protected virtual void Closed() { }

    protected abstract void Draw();
}
#endif
