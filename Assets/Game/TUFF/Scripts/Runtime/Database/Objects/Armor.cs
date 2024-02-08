using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "Armor", menuName = "Database/Armor", order = 7)]
    public class Armor : InventoryItem, IEquipable
    {
        [Header("General Settings")]
        [Tooltip("Name localization key from the Armor Table Collection.")]
        public string nameKey = "name_key";
        [Tooltip("Description localization key from the Armor Table Collection."), TextArea(2, 2)]
        public string descriptionKey = "description_key";
        [Tooltip("The armor's type. Compatible with Units who can equip this armor type.")]
        public int armorType = 0;
        [Tooltip("The slot that this armor occupies.")]
        public EquipType equipType = EquipType.Head;

        // Stat Changes
        public int maxHP { get { return m_maxHP; } set { m_maxHP = value; } }
        [SerializeField] protected int m_maxHP = 0;
        public int maxSP { get { return m_maxSP; } set { m_maxSP = value; } }
        [SerializeField] protected int m_maxSP = 0;
        public int maxTP { get { return m_maxTP; } set { m_maxTP = value; } }
        [SerializeField] protected int m_maxTP = 0;
        public int ATK { get { return m_ATK; } set { m_ATK = value; } }
        [SerializeField] protected int m_ATK = 0;
        public int DEF { get { return m_DEF; } set { m_DEF = value; } }
        [SerializeField] protected int m_DEF = 0;
        public int SATK { get { return m_SATK; } set { m_SATK = value; } }
        [SerializeField] protected int m_SATK = 0;
        public int SDEF { get { return m_SDEF; } set { m_SDEF = value; } }
        [SerializeField] protected int m_SDEF = 0;
        public int AGI { get { return m_AGI; } set { m_AGI = value; } }
        [SerializeField] protected int m_AGI = 0;
        public int LUK { get { return m_LUK; } set { m_LUK = value; } }
        [SerializeField] protected int m_LUK = 0;

        
        public List<Feature> features { get { return m_features; } set { m_features = value; } }
        [Header("Features")]
        [Tooltip("The changes applied to the affected user.")]
        public List<Feature> m_features = new List<Feature>();

        public override string GetName()
        {
            return TUFFTextParser.ParseText(nameKey);
        }
        public override string GetDescription()
        {
            return TUFFTextParser.ParseText(descriptionKey);
        }
    }
}
