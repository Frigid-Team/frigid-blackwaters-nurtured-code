#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace FrigidBlackwaters.Utility
{
    [CustomInspectorFieldDrawer(typeof(ShowIfPreviouslyShownAttribute))]
    public class ShowIfPreviouslyShownFieldDrawer : ShowIfAttributeFieldDrawer
    {
        protected override bool EvaluateWithRootObject(ShowIfAttribute showIfAttribute, SerializedObject rootObject, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            if (drawnFieldRecords.Count == 0) return false;

            InspectorDrawnFieldRecord previousDrawnFieldRecord = drawnFieldRecords[drawnFieldRecords.Count - 1];
            bool isShown = true;
            foreach (InspectorFieldDrawer drawnFieldDrawer in previousDrawnFieldRecord.DrawnFieldDrawers)
            {
                if (drawnFieldDrawer is ShowIfAttributeFieldDrawer)
                {
                    isShown &= ((ShowIfAttributeFieldDrawer)drawnFieldDrawer).IsShownInObjectInspector(rootObject, drawnFieldRecords.Take(drawnFieldRecords.Count - 1).ToList());
                    break;
                }
            }
            return isShown;
        }

        protected override bool EvaluateWithRootProperty(ShowIfAttribute showIfAttribute, SerializedProperty rootProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            if (drawnFieldRecords.Count == 0) return false;

            InspectorDrawnFieldRecord previousDrawnFieldRecord = drawnFieldRecords[drawnFieldRecords.Count - 1];
            bool isShown = true;
            foreach (InspectorFieldDrawer drawnFieldDrawer in previousDrawnFieldRecord.DrawnFieldDrawers)
            {
                if (drawnFieldDrawer is ShowIfAttributeFieldDrawer)
                {
                    isShown &= ((ShowIfAttributeFieldDrawer)drawnFieldDrawer).IsShownInPropertyInspector(rootProperty, drawnFieldRecords.Take(drawnFieldRecords.Count - 1).ToList());
                    break;
                }
            }
            return isShown;
        }
    }
}
#endif
