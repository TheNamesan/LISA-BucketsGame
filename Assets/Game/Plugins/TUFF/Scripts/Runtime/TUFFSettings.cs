using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "TUFFSettings", menuName = "TUFF/Settings/Settings Prefab", order = 99)]
    public class TUFFSettings : ScriptableObject
    {
        public static string version { get => "1.0.0"; }
        public static string interactableGizmoFilename { get => Instance.m_interactableGizmoFilename; }
        public static GameObject interactablePrefab { get => Instance.m_interactablePrefab; }
        public static GameObject enemyGraphicPrefab { get => Instance.m_enemyGraphicPrefab; }
        public static GameObject defaultTextbox { get => Instance.m_defaultTextbox; }
        public static GameObject fixedTextbox { get => Instance.m_systemTextbox; }
        // Debug
        public static bool startWithMaxItems { get => Instance.m_startWithMaxItems; }
        public static bool skillsCostNoResources { get => Instance.m_skillsCostNoResources; }
        public static bool ignoreLearnedSkills { get => Instance.m_ignoreLearnedSkills; }
        public static bool overrideUnitInitLevel { get => Instance.m_overrideUnitInitLevel; }
        public static int overrideUnitInitLevelValue { get => Instance.m_overrideUnitInitLevelValue; }
        // Battle System
        public static float critMultiplier { get => Instance.m_critMultiplier; }
        public static TPRecoveryByDamageType TPRecoveryByDamageType { get => Instance.m_TPRecoveryByDamageType; }
        public static int TPRecoveryByDamageRatio { get => Instance.m_TPRecoveryByDamageRatio; }
        public static int baseGuardDmgReduction { get => Instance.m_baseGuardDmgReduction; }
        public static int HPDangerThreshold { get => Instance.m_HPDangerThreshold; }
        public static bool showEnemyStatsByDefault { get => Instance.m_showEnemyStatsByDefault; }
        public static int baseMaxUP { get => Instance.m_baseMaxUP; }
        // Battle Types
        public static List<BattleType> elements { get => Instance.m_elements; }
        public static List<BattleType> weaponTypes { get => Instance.m_weaponTypes; }
        public static List<BattleType> armorTypes { get => Instance.m_armorTypes; }
        //Battle System UI
        public static Color HPColor { get => Instance.m_HPColor; }
        public static Color SPColor { get => Instance.m_SPColor; }
        public static Color TPColor { get => Instance.m_TPColor; }
        public static Color UPColor { get => Instance.m_UPColor; }
        public static Color HPNormalColor { get => Instance.m_HPNormalColor; }
        public static Color HPDangerColor { get => Instance.m_HPDangerColor; }
        public static Color HPKOColor { get => Instance.m_HPKOColor; }
        public static Color aliveGraphicColor { get => Instance.m_aliveGraphicColor; }
        public static Color KOGraphicColor { get => Instance.m_KOGraphicColor; }
        public static Color HPDamageTextColor { get => Instance.m_HPDamageTextColor; }
        public static Color HPDamageSpecialTextColor { get => Instance.m_HPDamageSpecialTextColor; }
        public static Color SPDamageTextColor { get => Instance.m_SPDamageTextColor; }
        public static Color HPRecoverTextColor { get => Instance.m_HPRecoverTextColor; }
        public static Color SPRecoverTextColor { get => Instance.m_SPRecoverTextColor; }
        public static Color TPRecoverTextColor { get => Instance.m_TPRecoverTextColor; }
        public static Color stateApplyTextColor { get => Instance.m_stateApplyTextColor; }
        public static Color stateRemovalTextColor { get => Instance.m_stateRemovalTextColor; }
        public static Color permanentStateApplyTextColor { get => Instance.m_permanentStateApplyTextColor; }
        public static Color weakpointTextColor { get => Instance.m_weakpointTextColor; }
        public static Color resistTextColor { get => Instance.m_resistTextColor; }
        public static Color immuneTextColor { get => Instance.m_immuneTextColor; }
        public static Color positiveColor { get => Instance.m_positiveColor; }
        public static Color negativeColor { get => Instance.m_negativeColor; }
        public static Sprite magsIcon { get => Instance.m_magsIcon; }
        //Battle System Prefab
        public static GameObject hitDisplayGroup { get => Instance.m_hitDisplayGroup; }
        public static GameObject enemyHPBar { get => Instance.m_enemyHPBar; }
        //Battle System Animations
        public static float critPauseTimer { get => Instance.m_critPauseTimer; }
        public static BattleAnimation enemyKOAnimation { get => Instance.m_enemyKOAnimation; }
        //Audio Prefabs
        public static Voicebank defaultVoicebank { get => Instance.m_defaultVoicebank; }
        //BGM
        public static BGMPlayData gameOverBGM { get => Instance.m_gameOverBGM; }
        //SFX
        public static SFX highlightSFX { get => Instance.m_highlightSFX; }
        public static SFX selectSFX { get => Instance.m_selectSFX; }
        public static SFX cancelSFX { get => Instance.m_cancelSFX; }
        public static SFX disabledSFX { get => Instance.m_disabledSFX; }
        public static SFX saveSFX { get => Instance.m_saveSFX; }
        public static SFX loadSFX { get => Instance.m_loadSFX; }
        public static SFX battleStartSFX { get => Instance.m_battleStartSFX; }
        public static AudioClip battleVictorySFX { get => Instance.m_battleVictorySFX; }
        public static SFX unitDamageSFX { get => Instance.m_unitDamageSFX; }
        public static SFX unitKOSFX { get => Instance.m_unitKOSFX; }
        public static SFX enemyDamageSFX { get => Instance.m_enemyDamageSFX; }
        public static SFX enemyKOSFX { get => Instance.m_enemyKOSFX; }
        public static SFX critDamageSFX { get => Instance.m_critDamageSFX; }
        public static SFX recoverySFX { get => Instance.m_recoverySFX; }
        public static SFX missSFX { get => Instance.m_missSFX; }
        public static SFX EXPTickSFX { get => Instance.m_EXPTickSFX; }
        public static SFX levelUpSFX { get => Instance.m_levelUpSFX; }
        public static SFX shopSFX { get => Instance.m_shopSFX; }
        public static SFX useItemSFX { get => Instance.m_useItemSFX; }
        public static SFX equipSFX { get => Instance.m_equipSFX; }
        //New Game Load
        public static string startingSceneName { get => Instance.m_startingSceneName; }
        public static Vector2 startingScenePosition { get => Instance.m_startingScenePosition; }
        public static FaceDirections startingSceneFacing { get => Instance.m_startingSceneFacing; }
        //Localization Table Colleciton Names
        public static string unitTable { get => Instance.m_unitTable; }
        public static string jobTable { get => Instance.m_jobTable; }
        public static string skillTable { get => Instance.m_skillTable; }
        public static string commandTable { get => Instance.m_commandTable; }
        public static string itemTable { get => Instance.m_itemTable; }
        public static string keyItemTable { get => Instance.m_keyitemTable; }
        public static string weaponTable { get => Instance.m_weaponTable; }
        public static string armorTable { get => Instance.m_armorTable; }
        public static string enemyTable { get => Instance.m_enemyTable; }
        public static string stateTable { get => Instance.m_stateTable; }
        public static string termsTable { get => Instance.m_termsTable; }
        public static string dialogueTable { get => Instance.m_dialogueTable; }
        // UI
        public static string itemsText { get => ParseText(Instance.m_itemsTermKey); }
        public static string weaponsText { get => ParseText(Instance.m_weaponsTermKey); }
        public static string armorsText { get => ParseText(Instance.m_armorsTermKey); }
        public static string keyItemsText { get => ParseText(Instance.m_keyItemsTermKey); }
        public static string buyText { get => ParseText(Instance.m_buyTermKey); }
        public static string sellText { get => ParseText(Instance.m_sellTermKey); }
        public static string possessionText { get => ParseText(Instance.m_possessionTermKey); }
        public static string quantityText { get => ParseText(Instance.m_quantityTermKey); }
        public static string acceptText { get => ParseText(Instance.m_acceptTermKey); }
        public static string cancelText { get => ParseText(Instance.m_cancelTermKey); }
        // Terms
        public static string levelText { get => ParseText(Instance.m_levelTermKey); }
        public static string levelShortText { get => ParseText(Instance.m_levelShortTermKey); }
        public static string HPText { get => ParseText(Instance.m_HPTermKey); }
        public static string HPShortText { get => ParseText(Instance.m_HPShortTermKey); }
        public static string SPText { get => ParseText(Instance.m_SPTermKey); }
        public static string SPShortText { get => ParseText(Instance.m_SPShortTermKey); }
        public static string TPText { get => ParseText(Instance.m_TPTermKey); }
        public static string TPShortText { get => ParseText(Instance.m_TPShortTermKey); }
        public static string maxHPShortText { get => ParseText(Instance.m_MaxHPShortTermKey); }
        public static string maxSPShortText { get => ParseText(Instance.m_MaxSPShortTermKey); }
        public static string maxTPShortText { get => ParseText(Instance.m_MaxTPShortTermKey); }
        public static string ATKShortText { get => ParseText(Instance.m_ATKShortTermKey); }
        public static string DEFShortText { get => ParseText(Instance.m_DEFShortTermKey); }
        public static string SATKShortText { get => ParseText(Instance.m_SATKShortTermKey); }
        public static string SDEFShortText { get => ParseText(Instance.m_SDEFShortTermKey); }
        public static string AGIShortText { get => ParseText(Instance.m_AGIShortTermKey); }
        public static string LUKShortText { get => ParseText(Instance.m_LUKShortTermKey); }
        public static string currencyText { get => ParseText(Instance.m_currencyTermKey); }
        public static string currencyShortText { get => ParseText(Instance.m_currencyShortTermKey); }
        public static string weaponText { get => ParseText(Instance.m_weaponTermKey); }
        public static string headText { get => ParseText(Instance.m_headTermKey); }
        public static string bodyText { get => ParseText(Instance.m_bodyTermKey); }
        public static string accessoryText { get => ParseText(Instance.m_accessoryTermKey); }
        public static string missText { get => ParseText(Instance.m_missTermKey); }
        public static string weakpointText { get => ParseText(Instance.m_weakpointTermKey); }
        public static string resistText { get => ParseText(Instance.m_resistTermKey); }
        public static string immuneText { get => ParseText(Instance.m_immuneTermKey); }
        public static string expText { get => ParseText(Instance.m_expTermKey); }
        public static string levelUpText { get => ParseText(Instance.m_levelUpTermKey); }
        public static string victoryMessageText { get => ParseText(Instance.m_victoryMessageTermKey); }
        public static string levelUpMessageText { get => ParseText(Instance.m_levelUpMessageTermKey); }
        public static string newSkillsText { get => ParseText(Instance.m_newSkillsTermKey); }

        [Header("Editor")]
        [Tooltip("Name of the texture for the gizmo rendered for Interactables. Gizmo icons must be located in Assets/Gizmos.")]
        [SerializeField] private string m_interactableGizmoFilename = "Interactable.png";

        [Header("TUFF Prefabs")]
        [SerializeField] private GameObject m_interactablePrefab;
        [SerializeField] private GameObject m_enemyGraphicPrefab;

        [Header("Textbox Prefabs")]
        [SerializeField]
        private GameObject m_defaultTextbox;
        [SerializeField]
        private GameObject m_systemTextbox;

        [Header("Debug")]
        [Tooltip("If true, player will start with the maximum amount of items in the inventory. Debug options only work when running the game through the editor.")]
        [SerializeField] private bool m_startWithMaxItems = false;
        [Tooltip("If true, skills require no SP/TP or other resources to use.")]
        [SerializeField] private bool m_skillsCostNoResources = false;
        [Tooltip("If true, all party members can use any skill from a command regardless of if the skill is known or not.")]
        [SerializeField] private bool m_ignoreLearnedSkills = false;
        [SerializeField] private bool m_overrideUnitInitLevel = false;
        [SerializeField] private int m_overrideUnitInitLevelValue = 1;

        [Header("Battle System")]
        [Tooltip("Default: 3")]
        [SerializeField] private float m_critMultiplier = 3;
        [Tooltip("Determines how users will recover TP when receiving damage.\n\n" +
            "Portion of Damage: The user will recover TP based on a percentage of the damage received (Recommended ratio: 1%) (Ex. A user who receives 300 damage will recover 3 TP at 1% ratio).\n\n" +
            "Portion of HP: The user will recover TP based on the HP lost relative to their Max HP (Recommended ratio: 50%) (Ex. A user with 100 Max TP will recover 50 TP when they lose all their HP at 50% ratio).")]
        [SerializeField] private TPRecoveryByDamageType m_TPRecoveryByDamageType = TPRecoveryByDamageType.PortionOfDamage;
        [Tooltip("Percentage of the TP to recover based on the TP Recovery By Damage Type")]
        [SerializeField] [Range(0, 100)] private int m_TPRecoveryByDamageRatio = 1;

        [Tooltip("Base percentage of damage reduced when guarding. If value is 100%, guarding will always negate all damage regardless of Guard Potency. Default: 50%")]
        [SerializeField] [Range(0, 100)] private int m_baseGuardDmgReduction = 50;
        [Tooltip("Percentage at which HP is considered low. Default: 25%")]
        [SerializeField] [Range(0, 100)] private int m_HPDangerThreshold = 25;
        [Tooltip("If true, the enemy's stats will be revealed (HP Bar and States) by default.")]
        [SerializeField] private bool m_showEnemyStatsByDefault = false;
        [Tooltip("Base value for UP value.")]
        [SerializeField] private int m_baseMaxUP = 300;
        
        // Battle Types
        [Tooltip("List of special characteristics given to attacks. Users can have weaknesses or resistance to a certain element, multiplying the damage dealt.")]
        [SerializeField] private List<BattleType> m_elements = new List<BattleType>();
        [Tooltip("List of Weapon types. Units can only equip Weapons with the Weapon Types they are compatible with.")]
        [SerializeField] private List<BattleType> m_weaponTypes = new List<BattleType>();
        [Tooltip("List of Armor types. Units can only equip Armors with the Armor Types they are compatible with.")]
        [SerializeField] private List<BattleType> m_armorTypes = new List<BattleType>();

        [Header("Battle System UI")]
        [SerializeField] private Color m_HPColor;
        [SerializeField] private Color m_SPColor;
        [SerializeField] private Color m_TPColor;
        [SerializeField] private Color m_UPColor;
        [Tooltip("HP and Name color text when HP is high.")]
        [SerializeField] private Color m_HPNormalColor = Color.white;
        [Tooltip("HP and Name color text when HP is low.")]
        [SerializeField] private Color m_HPDangerColor = Color.yellow;
        [Tooltip("HP and Name color text when KO'd.")]
        [SerializeField] private Color m_HPKOColor = Color.red;
        [Tooltip("Unit graphic's tint when HP is above 0.")]
        [SerializeField] private Color m_aliveGraphicColor = Color.white;
        [Tooltip("Unit graphic's tint when KO'd.")]
        [SerializeField] private Color m_KOGraphicColor = Color.gray;

        [SerializeField] private Color m_HPDamageTextColor;
        [SerializeField] private Color m_HPDamageSpecialTextColor;
        [SerializeField] private Color m_SPDamageTextColor;
        [SerializeField] private Color m_HPRecoverTextColor;
        [SerializeField] private Color m_SPRecoverTextColor;
        [SerializeField] private Color m_TPRecoverTextColor;
        [SerializeField] private Color m_stateApplyTextColor;
        [SerializeField] private Color m_stateRemovalTextColor;
        [SerializeField] private Color m_permanentStateApplyTextColor;
        [SerializeField] private Color m_weakpointTextColor;
        [SerializeField] private Color m_resistTextColor;
        [SerializeField] private Color m_immuneTextColor;
        [SerializeField] private Color m_positiveColor;
        [SerializeField] private Color m_negativeColor;
        [SerializeField] private Sprite m_magsIcon;

        [Header("Battle System Prefabs")]
        [SerializeField] private GameObject m_hitDisplayGroup;
        [SerializeField] private GameObject m_enemyHPBar;

        [Header("Battle System Animations")]
        [SerializeField] private float m_critPauseTimer = 0.5f;
        [SerializeField] private BattleAnimation m_enemyKOAnimation;

        [Header("Audio Prefabs")]
        [SerializeField] private Voicebank m_defaultVoicebank;

        [Header("BGM")]
        [SerializeField] private BGMPlayData m_gameOverBGM = new BGMPlayData();

        [Header("SFX")]
        [SerializeField] private SFX m_highlightSFX;
        [SerializeField] private SFX m_selectSFX;
        [SerializeField] private SFX m_cancelSFX;
        [SerializeField] private SFX m_disabledSFX;
        [SerializeField] private SFX m_saveSFX;
        [SerializeField] private SFX m_loadSFX;
        [SerializeField] private SFX m_battleStartSFX;
        [SerializeField] private AudioClip m_battleVictorySFX;
        [SerializeField] private SFX m_unitDamageSFX;
        [SerializeField] private SFX m_unitKOSFX;
        [SerializeField] private SFX m_enemyDamageSFX;
        [SerializeField] private SFX m_enemyKOSFX;
        [SerializeField] private SFX m_critDamageSFX;
        [SerializeField] private SFX m_recoverySFX;
        [SerializeField] private SFX m_missSFX;
        [SerializeField] private SFX m_EXPTickSFX;
        [SerializeField] private SFX m_levelUpSFX;
        [SerializeField] private SFX m_shopSFX;
        [SerializeField] private SFX m_useItemSFX;
        [SerializeField] private SFX m_equipSFX;

        [Header("New Game Load")]
        [Tooltip("The Scene to load on New Game.")]
        [SerializeField] private string m_startingSceneName = "";
        [Tooltip("The position on the Scene to place the Character on New Game.")]
        [SerializeField] private Vector2 m_startingScenePosition = Vector2.zero;
        [Tooltip("The Character's facing on New Game.")]
        [SerializeField] private FaceDirections m_startingSceneFacing = FaceDirections.East;

        [Header("Localization Table Collection Names")]
        [SerializeField] private string m_unitTable = "Unit";
        [SerializeField] private string m_jobTable = "Job";
        [SerializeField] private string m_skillTable = "Skill";
        [SerializeField] private string m_commandTable = "Command";
        [SerializeField] private string m_itemTable = "Item";
        [SerializeField] private string m_keyitemTable = "KeyItem";
        [SerializeField] private string m_weaponTable = "Weapon";
        [SerializeField] private string m_armorTable = "Armor";
        [SerializeField] private string m_enemyTable = "Enemy";
        [SerializeField] private string m_stateTable = "State";
        [SerializeField] private string m_termsTable = "Terms";
        [SerializeField] private string m_dialogueTable = "Dialogue";

        public string m_itemsTermKey = "";
        public string m_weaponsTermKey = "";
        public string m_armorsTermKey = "";
        public string m_keyItemsTermKey = "";
        public string m_buyTermKey = "";
        public string m_sellTermKey = "";
        public string m_possessionTermKey = "Possession";
        public string m_quantityTermKey = "Quantity";
        public string m_acceptTermKey = "";
        public string m_cancelTermKey = "";

        public string m_levelTermKey = "";
        public string m_levelShortTermKey = "";
        public string m_HPTermKey = "";
        public string m_HPShortTermKey = "";
        public string m_SPTermKey = "";
        public string m_SPShortTermKey = "";
        public string m_TPTermKey = "";
        public string m_TPShortTermKey = "";
        public string m_MaxHPShortTermKey = "";
        public string m_MaxSPShortTermKey = "";
        public string m_MaxTPShortTermKey = "";
        public string m_ATKShortTermKey = "";
        public string m_DEFShortTermKey = "";
        public string m_SATKShortTermKey = "";
        public string m_SDEFShortTermKey = "";
        public string m_AGIShortTermKey = "";
        public string m_LUKShortTermKey = "";
        public string m_currencyTermKey = "Magazines";
        public string m_currencyShortTermKey = "Mags";

        public string m_weaponTermKey = "";
        public string m_headTermKey = "";
        public string m_bodyTermKey = "";
        public string m_accessoryTermKey = "";

        public string m_missTermKey = "";
        public string m_weakpointTermKey = "";
        public string m_resistTermKey = "";
        public string m_immuneTermKey = "";

        public string m_levelUpTermKey = "";
        public string m_expTermKey = "";
        public string m_victoryMessageTermKey = "";
        public string m_levelUpMessageTermKey = "";
        public string m_newSkillsTermKey = "";

        public static bool DebugStartWithMaxItems()
        {
            return Application.isEditor && startWithMaxItems;
        }
        public static bool DebugSkillsCostNoResources()
        {
            return Application.isEditor && skillsCostNoResources;
        }
        public static bool DebugIgnoreLearnedSkills()
        {
            return Application.isEditor && ignoreLearnedSkills;
        }
        public static bool DebugOverrideUnitInitLevel()
        {
            return Application.isEditor && overrideUnitInitLevel;
        }
        public static TUFFSettings Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = Resources.Load<TUFFSettings>("TUFFSettings");
                
                return m_instance;
            }
        }
        private static TUFFSettings m_instance = null;
        private static string ParseText(string text) => TUFFTextParser.ParseText(text);
    }
}