using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class InventoryItem : DatabaseElement 
    {
        [SerializeField] protected Sprite m_icon;
        public Sprite icon
        {
            get { return m_icon; }
            set { m_icon = value; }
        }
        [Tooltip("Item's buy cost at shops.")]
        public int price = 0;
        public virtual void ConsumeItemFromMenu(Targetable user) { }
    }
}

