using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Database/Unit", order = 0)]
    public class Unit : DatabaseElement
    {
        [Header("General Settings")]
        [Tooltip("Name localization key from the Unit Table Collection.")]
        public string nameKey = "Name";
        [Tooltip("Full localization key from the Unit Table Collection.")]
        public string fullNameKey = "Full Name";
        [Tooltip("Reference to the Unit's Job when initialized.")]
        public Job initialJob;
        [Tooltip("Unit's level when initialized.")]
        [Range(1, 100)] public int initialLevel = 1;

        [Header("Graphics")]
        [SerializeField] public Sprite defaultMenuPortrait;
        [Tooltip("The default sprite to show for Battle menus.")]
        [SerializeField] public Sprite defaultFaceGraphic;

        [Header("Initial Equipment")]
        public Weapon primaryWeapon;
        public Weapon secondaryWeapon;
        public Armor head;
        public Armor body;
        public Armor primaryAccessory;
        public Armor secondaryAccessory;

        [Header("Features")]
        [Tooltip("The changes applied to the affected user.")]
        public List<Feature> features = new List<Feature>();

        [Header("Quotes")]
        public CharacterQuotes winQuotes = new CharacterQuotes();
        public CharacterQuotes levelUpQuotes = new CharacterQuotes();
        public CharacterQuotes dropsQuotes = new CharacterQuotes();

        public override string GetName()
        {
            return TUFFTextParser.ParseText(nameKey);
        }
        public string GetFullName()
        {
            return TUFFTextParser.ParseText(nameKey);
        }
    }
}
