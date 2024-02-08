using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ResultsScreenHUD : MonoBehaviour
    {
        protected UIMenu currentMenu = null;
        public GameObject descriptionBox;
        public GameObject quoteBox;
        public UIMenu levelUpMenu;
        public UIMenu levelUpOverviewMenu;
        public UIMenu rewardsMenu;
        

        public bool isFinished = true;
        public virtual IEnumerator ShowResults()
        {
            isFinished = false;

            gameObject?.SetActive(true);
            descriptionBox?.SetActive(true);
            quoteBox?.SetActive(true);
            Debug.Log("Results");

            Debug.Log("Level Up");
            levelUpMenu.OpenMenu();

            currentMenu = levelUpMenu;

            while (!isFinished) { yield return null; }
            yield break;
        }
        public virtual void NextMenu()
        {
            if (currentMenu == levelUpMenu)
            {
                levelUpMenu.CloseMenu();
                currentMenu = levelUpOverviewMenu;
                levelUpOverviewMenu.OpenMenu();
                
                return;
            }
            else if (currentMenu == levelUpOverviewMenu)
            {
                levelUpOverviewMenu.CloseMenu();

                Debug.Log("Rewards");
                currentMenu = rewardsMenu;
                rewardsMenu.OpenMenu();
                
            }
            else
            {
                rewardsMenu.CloseMenu();
                EndResults();
            }
        }
        public virtual void EndResults()
        {
            descriptionBox?.SetActive(false);
            quoteBox?.SetActive(false);
            isFinished = true;
        }
    }
}

