using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class LevelUpOverviewHUD : MonoBehaviour
    {
        public ResultsScreenHUD resultsScreenHUD;
        public GeneralInfoDisplay generalInfoDisplayPrefab;
        public TMP_Text descriptionText;
        public QuoteBoxHUD quoteBoxHUD;
        public Image portrait;
        public TMP_Text nameText;
        public TMP_Text expText;
        public TMP_Text newSkillsText;
        public RectTransform newSkillsContent;


        [Header("Element References")]
        public StatChangeElement levelElement;
        public StatChangeElement maxHPElement;
        public StatChangeElement maxSPElement;
        public StatChangeElement maxTPElement;
        public StatChangeElement ATKElement;
        public StatChangeElement DEFElement;
        public StatChangeElement SATKElement;
        public StatChangeElement SDEFElement;
        public StatChangeElement AGIElement;
        public StatChangeElement LUKElement;

        protected List<PartyMember> leveledUpMembers = new List<PartyMember>();
        protected List<GeneralInfoDisplay> newSkillsElements = new List<GeneralInfoDisplay>();
        protected int currentIndex = 0;
        public void Initiate()
        {
            currentIndex = 0;
            leveledUpMembers.Clear();
            GetLeveledUpPartyMembers();
            if (leveledUpMembers.Count <= 0)
            {
                resultsScreenHUD.NextMenu();
                return;
            }
            ApplyLabels();
            ShowOverview(currentIndex);
        }

        public void Update()
        {
            if (UIController.instance.actionButtonDown)
            {
                currentIndex += 1;
                if (currentIndex >= leveledUpMembers.Count) resultsScreenHUD.NextMenu();
                else ShowOverview(currentIndex);
            }
        }

        protected void ShowOverview(int idx)
        {
            var member = leveledUpMembers[idx];
            AudioManager.instance.PlaySFX(TUFFSettings.levelUpSFX);
            descriptionText.text = $"{member.GetName()}{TUFFSettings.levelUpMessageText}";
            SetQuote(member);
            portrait.sprite = member.GetPortraitSprite();
            nameText.text = member.GetName();
            expText.text = $"+{LISAUtility.IntToString(BattleManager.instance.expCollected)}{TUFFSettings.expText}";
            UpdateElementsInfo(member);
            UpdateNewSkills(member);
        }

        protected virtual void UpdateElementsInfo(PartyMember member)
        {
            if (member == null) return;
            var job = member.GetJob();
            if (job == null) return;
            int prevLv = member.prevLevel;
            levelElement.UpdateInfo(prevLv, member.level);
            maxHPElement.UpdateInfo(job.LevelToStat(prevLv, LevelToStatType.MaxHP), member.GetBaseMaxHP());
            if (job.usesSP)
            {
                maxSPElement.gameObject.SetActive(true);
                maxSPElement.UpdateInfo(job.LevelToStat(prevLv, LevelToStatType.MaxSP), member.GetBaseMaxSP());
            }
            else maxSPElement.gameObject.SetActive(false);
            if (job.usesTP)
            {
                maxTPElement.gameObject.SetActive(true);
                maxTPElement.UpdateInfo(job.LevelToStat(prevLv, LevelToStatType.MaxTP), member.GetBaseMaxTP()); 
            }
            else maxTPElement.gameObject.SetActive(false);
            ATKElement.UpdateInfo(job.LevelToStat(prevLv, LevelToStatType.ATK), member.GetBaseATK());
            DEFElement.UpdateInfo(job.LevelToStat(prevLv, LevelToStatType.DEF), member.GetBaseDEF());
            SATKElement.UpdateInfo(job.LevelToStat(prevLv, LevelToStatType.SATK), member.GetBaseSATK());
            SDEFElement.UpdateInfo(job.LevelToStat(prevLv, LevelToStatType.SDEF), member.GetBaseSDEF());
            AGIElement.UpdateInfo(job.LevelToStat(prevLv, LevelToStatType.AGI), member.GetBaseAGI());
            LUKElement.UpdateInfo(job.LevelToStat(prevLv, LevelToStatType.LUK), member.GetBaseLUK());
        }

        protected void UpdateNewSkills(PartyMember member)
        {
            ResetNewSkillsContent();
            if (member == null) return;
            if (member.level < member.prevLevel)
            {
                newSkillsText.gameObject.SetActive(false);
                return;
            }
            //var commands = member.GetCommands();
            //var validSkillsDirectory = new Dictionary<int, List<SkillsLearned>>(); // (Level, list of skills learned at level)
            var validSkills = member.job.GetSkillsToLearnAtLevel(member.level, member.prevLevel + 1);
            //foreach (Command command in commands)
            //{
            //    var skillsLearned = command.skills;
            //    for (int i = 0; i < skillsLearned.Count; i++)
            //    {
            //        var skillLearned = skillsLearned[i];
            //        if (skillLearned.learnType != LearnType.Level) continue;
            //        int level = skillLearned.levelLearnedAt;
            //        if (validSkillsDirectory.ContainsKey(level)) validSkillsDirectory[level].Add(skillLearned);
            //        else
            //        { validSkillsDirectory.Add(level, new List<SkillsLearned>() { skillLearned }); }
            //    }
            //}
            //for (int i = member.prevLevel + 1; i <= member.level; i++)
            //{
            //    if (!validSkillsDirectory.ContainsKey(i)) continue;
            //    var skillsLearnedAtLevel = validSkillsDirectory[i];
            //    foreach(SkillsLearned skill in skillsLearnedAtLevel)
            //    {
            //        var newSkill = Instantiate(generalInfoDisplayPrefab, newSkillsContent);
            //        newSkill.DisplayInfo(skill.skill.icon, skill.skill.GetName());
            //        newSkillsElements.Add(newSkill);
            //    }
            //}
            for (int i = 0; i < validSkills.Count; i++)
            {
                var skill = validSkills[i];
                var newSkill = Instantiate(generalInfoDisplayPrefab, newSkillsContent);
                newSkill.DisplayInfo(skill.icon, skill.GetName());
                newSkillsElements.Add(newSkill);
            }
            if (newSkillsElements.Count <= 0) newSkillsText.gameObject.SetActive(false);
            else newSkillsText.gameObject.SetActive(true);
        }

        private void ResetNewSkillsContent()
        {
            foreach (Transform child in newSkillsContent) // Clean new skills content
            {
                Destroy(child.gameObject);
            }
            newSkillsElements.Clear();
        }

        protected void GetLeveledUpPartyMembers()
        {
            var partyMembers = PlayerData.instance.GetAllPartyMembers();
            for (int i = 0; i < partyMembers.Count; i++)
            {
                if (partyMembers[i].prevLevel != partyMembers[i].level) // If a party member leveled up
                {
                    leveledUpMembers.Add(partyMembers[i]);
                }
            }
        }

        protected void ApplyLabels()
        {
            levelElement.UpdateLabel(TUFFSettings.levelShortText);
            maxHPElement.UpdateLabel(TUFFSettings.maxHPShortText);
            maxSPElement.UpdateLabel(TUFFSettings.maxSPShortText);
            maxTPElement.UpdateLabel(TUFFSettings.maxTPShortText);
            ATKElement.UpdateLabel(TUFFSettings.ATKShortText);
            DEFElement.UpdateLabel(TUFFSettings.DEFShortText);
            SATKElement.UpdateLabel(TUFFSettings.SATKShortText);
            SDEFElement.UpdateLabel(TUFFSettings.SDEFShortText);
            AGIElement.UpdateLabel(TUFFSettings.AGIShortText);
            LUKElement.UpdateLabel(TUFFSettings.LUKShortText);
            newSkillsText.text = TUFFSettings.newSkillsText;
        }
        public void SetQuote(PartyMember member)
        {
            quoteBoxHUD.DisplayQuote(member.GetGraphic(), member.GetName(), member.GetRandomLevelUpQuote());
        }
    }
}

