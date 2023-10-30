using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class ParameterAnimatorProperty<P> : AnimatorProperty where P : AnimatorProperty
    {
        [SerializeField]
        private List<P> parameteredProperties;

        public int GetNumberParameteredProperties()
        {
            return this.parameteredProperties == null ? 0 : this.parameteredProperties.Count;
        }

        public P GetParameteredProperty(int propertyIndex)
        {
            return this.parameteredProperties[propertyIndex];
        }

        public void SetParameteredProperty(int propertyIndex, P parameteredProperty)
        {
            if (this.parameteredProperties[propertyIndex] != parameteredProperty)
            {
                FrigidEdit.RecordChanges(this);
                this.parameteredProperties[propertyIndex] = parameteredProperty;
            }
        }

        public virtual void AddParameteredPropertyAt(int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.parameteredProperties.Insert(propertyIndex, null);
        }

        public virtual void RemoveParameteredPropertyAt(int propertyIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.parameteredProperties.RemoveAt(propertyIndex);
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.parameteredProperties = new List<P>();
            base.Created();
        }
    }
}
