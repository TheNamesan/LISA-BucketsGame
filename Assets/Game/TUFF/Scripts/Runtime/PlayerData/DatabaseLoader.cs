using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class DatabaseLoader : MonoBehaviour
    {
        public Unit[] units;
        public Job[] jobs;
        public Skill[] skills;
        public Command[] commands;
        public Item[] items;
        public KeyItem[] keyItems;
        public Weapon[] weapons;
        public Armor[] armors;
        public State[] states;
        public BattleAnimation[] animations;
        public static string battlePath = "Database/9Battles";
        public static string animationsPath = "Database/11Animations";
        public static string termsPath = "Database/12Terms/Terms";
        public static DatabaseLoader instance { get {
                if (!GameManager.instance) return null;
                return GameManager.instance.databaseLoader;
                //if (m_instance == null)
                //{
                //    //GameManager.CheckInstance();
                //    if (GameManager.instance == null) return null;
                //    AssignInstance(GameManager.instance.GetComponentInChildren<DatabaseLoader>());
                //}

                //return m_instance;
            }
        }
        //protected static DatabaseLoader m_instance;
        public void Awake()
        {
            //if (m_instance != null)
            //{
            //    if (instance != this) Destroy(gameObject);
            //}
            //else
            //{
            //    AssignInstance(this);
            //}
            units = Resources.LoadAll<Unit>("Database/0Units");
            jobs = Resources.LoadAll<Job>("Database/1Jobs");
            skills = Resources.LoadAll<Skill>("Database/2Skills");
            commands = Resources.LoadAll<Command>("Database/3Commands");
            items = Resources.LoadAll<Item>("Database/4Items");
            keyItems = Resources.LoadAll<KeyItem>("Database/5KeyItems");
            weapons = Resources.LoadAll<Weapon>("Database/6Weapons");
            armors = Resources.LoadAll<Armor>("Database/7Armors");
            states = Resources.LoadAll<State>("Database/10States");
            animations = Resources.LoadAll<BattleAnimation>(animationsPath);
            AssignIDs();
        }
        //protected static void AssignInstance(DatabaseLoader target)
        //{
        //    if (target == null) return;
        //    m_instance = target;
        //    if (target.gameObject)
        //        DontDestroyOnLoad(target.gameObject);
        //    m_instance.units = Resources.LoadAll<Unit>("Database/0Units");
        //    m_instance.jobs = Resources.LoadAll<Job>("Database/1Jobs");
        //    m_instance.skills = Resources.LoadAll<Skill>("Database/2Skills");
        //    m_instance.commands = Resources.LoadAll<Command>("Database/3Commands");
        //    m_instance.items = Resources.LoadAll<Item>("Database/4Items");
        //    m_instance.keyItems = Resources.LoadAll<KeyItem>("Database/5KeyItems");
        //    m_instance.weapons = Resources.LoadAll<Weapon>("Database/6Weapons");
        //    m_instance.armors = Resources.LoadAll<Armor>("Database/7Armors");
        //    m_instance.states = Resources.LoadAll<State>("Database/10States");
        //    m_instance.animations = Resources.LoadAll<BattleAnimation>(animationsPath);
        //    m_instance.AssignIDs();
        //}
        protected void AssignIDs()
        {
            for (int i = 0; i < units.Length; i++) { units[i].id = i; }
            for (int i = 0; i < jobs.Length; i++) { jobs[i].id = i; }
            for (int i = 0; i < skills.Length; i++) { skills[i].id = i; }
            for (int i = 0; i < commands.Length; i++) { commands[i].id = i; }
            for (int i = 0; i < items.Length; i++) { items[i].id = i; }
            for (int i = 0; i < keyItems.Length; i++) { keyItems[i].id = i; }
            for (int i = 0; i < weapons.Length; i++) { weapons[i].id = i; }
            for (int i = 0; i < armors.Length; i++) { armors[i].id = i; }
            for (int i = 0; i < states.Length; i++) { states[i].id = i; }
        }
        public List<State> GetAllStatesOfType(StateType type)
        {
            var statesOfType = new List<State>();
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].stateType != type) continue;
                statesOfType.Add(states[i]);
            }
            return statesOfType;
        }
    }
}