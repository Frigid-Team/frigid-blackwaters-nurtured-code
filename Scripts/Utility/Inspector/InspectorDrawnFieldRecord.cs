#if UNITY_EDITOR
using System.Collections.Generic;

namespace FrigidBlackwaters.Utility
{
    public class InspectorDrawnFieldRecord
    {
        private List<InspectorFieldDrawer> drawnFieldDrawers;

        public InspectorDrawnFieldRecord(List<InspectorFieldDrawer> drawnFieldDrawers)
        {
            this.drawnFieldDrawers = drawnFieldDrawers;
        }

        public List<InspectorFieldDrawer> DrawnFieldDrawers
        {
            get
            {
                return this.drawnFieldDrawers;
            }
        }
    }
}
#endif
