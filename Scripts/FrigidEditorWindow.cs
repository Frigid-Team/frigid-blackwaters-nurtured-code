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
            Opened();
        }

        private void OnDestroy()
        {
            Closed();
        }

        private void Update()
        {
            if (this.hasFocus)
            {
                Repaint();
            }
        }

        private void OnGUI() 
        {
            using (new FrigidEditMode.EditingScope())
            {
                Draw();
            }
        }
    }
}
#endif
