using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(TUFFSettings))]
    public class TUFFSettingsEditor : Editor
    {
        private static Texture2D tuffLogo = null;
        private static Texture2D tuffText = null;
        public override void OnInspectorGUI()
        {
            var settings = target as TUFFSettings;
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_interactableGizmoFilename"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_interactablePrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_overworldCharacterPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_enemyGraphicPrefab"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_defaultTextbox"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_systemTextbox"));

            // Debug
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_startWithMaxItems"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_skillsCostNoResources"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ignoreLearnedSkills"));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_overrideUnitInitLevel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_overrideUnitInitLevelValue"));
            EditorGUILayout.EndHorizontal();

            // Player Data
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxSaveFileSlots"));

            // Battle System
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_critMultiplier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TPRecoveryByDamageType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TPRecoveryByDamageRatio"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_baseGuardDmgReduction"), new GUIContent("Base Guard Damage Reduction (%)"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HPDangerThreshold"), new GUIContent("HP Danger Threshold (%)"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_showEnemyStatsByDefault"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_baseMaxUP"));

            EditorGUILayout.LabelField("Battle Types", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_elements"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_weaponTypes"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_armorTypes"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HPColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SPColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TPColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_UPColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HPNormalColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HPDangerColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HPKOColor"), new GUIContent("HP KO Color"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_aliveGraphicColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_KOGraphicColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HPDamageTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HPDamageSpecialTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SPDamageTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HPRecoverTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SPRecoverTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TPRecoverTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_stateApplyTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_stateRemovalTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_permanentStateApplyTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_weakpointTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_resistTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_immuneTextColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_positiveColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_negativeColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_magsIcon"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_hitDisplayGroup"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_enemyHPBar"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_critPauseTimer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_enemyKOAnimation"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_defaultVoicebank"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_gameOverBGM"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_highlightSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_selectSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_cancelSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_disabledSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_equipSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_saveSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_loadSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_battleStartSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_escapeSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_battleVictorySFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_unitDamageSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_unitKOSFX"), new GUIContent("Unit KO SFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_enemyDamageSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_enemyKOSFX"), new GUIContent("Enemy KO SFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_critDamageSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_recoverySFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_missSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EXPTickSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_levelUpSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_shopSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_useItemSFX"));
            

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_startingSceneName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_startingScenePosition"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_startingSceneFacing"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_unitTable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_jobTable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_skillTable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_commandTable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_itemTable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_keyitemTable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_weaponTable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_armorTable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_enemyTable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_stateTable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_termsTable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_dialogueTable"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Terms", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("UI", EditorStyles.boldLabel);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_itemsTermKey", TUFFSettings.itemsText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_weaponsTermKey", TUFFSettings.weaponsText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_armorsTermKey", TUFFSettings.armorsText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_keyItemsTermKey", TUFFSettings.keyItemsText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_buyTermKey", TUFFSettings.buyText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_sellTermKey", TUFFSettings.sellText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_possessionTermKey", TUFFSettings.possessionText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_quantityTermKey", TUFFSettings.quantityText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_acceptTermKey", TUFFSettings.acceptText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_cancelTermKey", TUFFSettings.cancelText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_saveFilePromptTermKey", TUFFSettings.saveFilePromptText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_loadFilePromptTermKey", TUFFSettings.loadFilePromptText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_currentExpTermKey", TUFFSettings.currentExpText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_toNextLevelTermKey", TUFFSettings.toNextLevelText);

            EditorGUILayout.LabelField("Character Bio", EditorStyles.boldLabel);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_fightingArtTermKey", TUFFSettings.fightingArtText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_pastOccupationTermKey", TUFFSettings.pastOccupationText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_likesTermKey", TUFFSettings.likesText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_favoriteFoodTermKey", TUFFSettings.favoriteFoodText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_mostHatedThingTermKey", TUFFSettings.mostHatedThingText);

            EditorGUILayout.LabelField("Basic Status", EditorStyles.boldLabel);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_levelTermKey", TUFFSettings.levelText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_levelShortTermKey", TUFFSettings.levelShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_HPTermKey", TUFFSettings.HPText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_HPShortTermKey", TUFFSettings.HPShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_SPTermKey", TUFFSettings.SPText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_SPShortTermKey", TUFFSettings.SPShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_TPTermKey", TUFFSettings.TPText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_TPShortTermKey", TUFFSettings.TPShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_MaxHPShortTermKey", TUFFSettings.maxHPShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_MaxSPShortTermKey", TUFFSettings.maxSPShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_MaxTPShortTermKey", TUFFSettings.maxTPShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_ATKShortTermKey", TUFFSettings.ATKShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_DEFShortTermKey", TUFFSettings.DEFShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_SATKShortTermKey", TUFFSettings.SATKShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_SDEFShortTermKey", TUFFSettings.SDEFShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_AGIShortTermKey", TUFFSettings.AGIShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_LUKShortTermKey", TUFFSettings.LUKShortText);

            EditorGUILayout.LabelField("Extra Rate", EditorStyles.boldLabel);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_hitRateTermKey", TUFFSettings.hitRateText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_hitRateShortTermKey", TUFFSettings.hitRateShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_evasionRateTermKey", TUFFSettings.evasionRateText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_evasionRateShortTermKey", TUFFSettings.evasionRateShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_criticalRateTermKey", TUFFSettings.criticalRateText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_criticalRateShortTermKey", TUFFSettings.criticalRateShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_criticalEvasionRateTermKey", TUFFSettings.criticalEvasionRateText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_criticalEvasionRateShortTermKey", TUFFSettings.criticalEvasionRateShortText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_targetRateTermKey", TUFFSettings.targetRateText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_targetRateShortTermKey", TUFFSettings.targetRateShortText);

            EditorGUILayout.LabelField("Currency", EditorStyles.boldLabel);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_currencyTermKey", TUFFSettings.currencyText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_currencyShortTermKey", TUFFSettings.currencyShortText);

            EditorGUILayout.LabelField("Equip Types", EditorStyles.boldLabel);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_weaponTermKey", TUFFSettings.weaponText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_headTermKey", TUFFSettings.headText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_bodyTermKey", TUFFSettings.bodyText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_accessoryTermKey", TUFFSettings.accessoryText);

            EditorGUILayout.LabelField("Battle Messages", EditorStyles.boldLabel);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_missTermKey", TUFFSettings.missText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_weakpointTermKey", TUFFSettings.weakpointText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_resistTermKey", TUFFSettings.resistText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_immuneTermKey", TUFFSettings.immuneText);

            EditorGUILayout.LabelField("Victory Messages", EditorStyles.boldLabel);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_expTermKey", TUFFSettings.expText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_levelUpTermKey", TUFFSettings.levelUpText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_victoryMessageTermKey", TUFFSettings.victoryMessageText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_levelUpMessageTermKey", TUFFSettings.levelUpMessageText);
            DrawTermFieldAndLocalizedPreview(serializedObject, "m_newSkillsTermKey", TUFFSettings.newSkillsText);

            serializedObject.ApplyModifiedProperties();
        }

        protected static void DrawTermFieldAndLocalizedPreview(SerializedObject serializedObject, string fieldName, string parsedText)
        {
            EditorGUILayout.BeginHorizontal();
            var prop = serializedObject.FindProperty(fieldName);
            EditorGUILayout.PropertyField(prop);
            EditorGUILayout.LabelField(parsedText);
            EditorGUILayout.EndHorizontal();
        }

        public static void GetLogoTextures()
        {
            if (tuffLogo != null && tuffText != null) return;
            var guid = AssetDatabase.FindAssets("TUFFLogo t:Texture2D");
            if(guid.Length != 0)
            {
                tuffLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid[0]));
            }
            guid = AssetDatabase.FindAssets("TUFFText t:Texture2D");
            if (guid.Length != 0)
            {
                tuffText = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid[0]));
            }
        }

        public static void MainSettingsPanel()
        {
            GUIStyle wrapStyle = new GUIStyle(GUI.skin.GetStyle("label"))
            {
                wordWrap = true,
                richText = true
            };
            EditorGUILayout.BeginHorizontal();
            GetLogoTextures();
            
            Vector2 logoOffset = new Vector2(10, 52);
            Vector2 logoSize = new Vector2(160, 160);
            EditorGUILayout.Space(logoSize.x);
            GUI.DrawTexture(new Rect(logoOffset, logoSize), tuffLogo);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            Vector2 textOffset = new Vector2(logoOffset.x + logoSize.x, 10);
            Vector2 textSize = new Vector2(226, 104);
            EditorGUILayout.Space(textSize.y);
            GUI.DrawTexture(new Rect(textOffset, textSize), tuffText);
            EditorGUILayout.LabelField($"<b>Welcome to the The Unity Foolish Framework!</b>", wrapStyle);
            EditorGUILayout.LabelField($"<b>Version {TUFFSettings.version}</b>", wrapStyle);
            EditorGUILayout.Space();
            string description =
                "This framework was designed to produce LISA inspired games, based on RPG Maker VX Ace and the Yanfly Engine Plugins.\n" +
                "Originally created for the LISA: The Fool fangame by LimeTime. Developed by TheNamesan.\n\n" +
                "LISA: The Painful RPG and the LISA series are property of Austin Jorgensen."
                ;
            EditorGUILayout.LabelField(description, wrapStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"CREDITS: TheNamesan, Omega, baba-s, Dylan Engelman, Noisemaker, The LISA: The Fool Dev Team", wrapStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}
