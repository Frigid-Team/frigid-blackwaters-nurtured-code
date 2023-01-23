using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public class FPSDisplay : FrigidMonoBehaviour
    {
		private float deltaTime;

        protected override void Start()
        {
			base.Start();
			this.deltaTime = 0.0f;
        }

        protected override void Update()
		{
			base.Update();
			this.deltaTime += (Time.unscaledDeltaTime - this.deltaTime) * 0.1f;
		}

		private void OnGUI()
		{
			int w = Screen.width, h = Screen.height;

			GUIStyle style = new GUIStyle();

			Rect rect = new Rect(0, 0, w, h * 2 / 100);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = h * 2 / 100;
			style.normal.textColor = Color.white;
			float msec = this.deltaTime * 1000.0f;
			float fps = 1.0f / this.deltaTime;
			string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
			GUI.Label(rect, text, style);
		}
	}
}
