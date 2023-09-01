#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FrigidBlackwaters
{
    public abstract class FrigidEditorWindow : EditorWindow
    {
        public static void Show<W>() where W : FrigidEditorWindow
        {
            W window = GetWindow<W>();
            window.titleContent = new GUIContent(window.Title);
            window.Show();
        }

        protected abstract string Title
        {
            get;
        }

        protected virtual void Opened() { }

        protected virtual void Closed() { }

        protected abstract void Draw();

        private void Awake()
        {
            this.Opened();
        }

        private void OnDestroy()
        {
            this.Closed();
        }

        private void Update()
        {
            if (this.hasFocus)
            {
                this.Repaint();
            }
        }

        private void OnGUI() 
        {
            using (new FrigidEdit.EditingScope())
            {
                this.Draw();
            }
            if (GUI.Button(new Rect(Vector2.zero, this.position.size), "", new GUIStyle()))
            {
                GUI.FocusControl(null);
            }
        }
    }
}
#endif
