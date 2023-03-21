using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Core
{
    public class FPSDisplay : MonoBehaviour
    {
		private List<float> deltaTimesWithinLastSecond;
		private float currElapsed;
		private float lastAvgDeltaTime;

        private void Start()
        {
			this.deltaTimesWithinLastSecond = new List<float>();
			this.currElapsed = 0;
        }

        private void Update()
        {
			this.currElapsed += Time.unscaledDeltaTime;
			this.deltaTimesWithinLastSecond.Add(Time.unscaledDeltaTime);
			if (this.currElapsed > 1f)
            {
				float sum = 0f;
				foreach (float deltaTimeWithinLastSecond in this.deltaTimesWithinLastSecond)
                {
					sum += deltaTimeWithinLastSecond;
                }
				this.lastAvgDeltaTime = sum / this.deltaTimesWithinLastSecond.Count;
				this.deltaTimesWithinLastSecond.Clear();
				this.currElapsed = 0;
            }
        }

        private void OnGUI()
		{
			int w = Screen.width, h = Screen.height;

			GUIStyle style = new GUIStyle();

			Rect rect = new Rect(0, 0, w, h * 2 / 100);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = h * 2 / 100;
			style.normal.textColor = Color.white;
			float msec = Time.unscaledDeltaTime * 1000.0f;
			float fps = 1.0f / Time.unscaledDeltaTime;
			float avgFps = 1.0f / this.lastAvgDeltaTime;
			string text = string.Format("{0:0.0} ms ({1:0.} fps) {2:0.0} avg. ms ({3:0.} avg. fps)", msec, fps, this.lastAvgDeltaTime * 1000.0f, avgFps);
			GUI.Label(rect, text, style);
		}
	}
}
