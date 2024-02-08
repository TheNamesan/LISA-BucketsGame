using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public class PopulateTest : MonoBehaviour
    {
        [SerializeField]
        GameObject toSpawn;
        [SerializeField]
        int spawnThisMany;
        string itemName;
        // Start is called before the first frame update
        void Start()
        {
            Populate(); 
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void Populate()
        {
            /*GameObject obj;
            for (int i = 0; i < spawnThisMany; i++)
            {
                obj = Instantiate(toSpawn, transform);
                obj.GetComponent<ItemButtonController>().itemName = i.ToString();
                obj.GetComponent<Button>().onClick.AddListener(obj.GetComponent<ItemButtonController>().GetItemName);
            }*/
        }

        public void Test()
        {
            Debug.Log(itemName);
        }
    }
}
