using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class OpenShopAction : EventAction
    {
        public ShopData shopData = new ShopData();
        public OpenShopAction()
        {
            eventName = "Open Shop";
            eventColor = new Color(0.5f, 1f, 0.95f, 1f);
        }
        public override void Invoke()
        {
            if (shopData == null) { isFinished = true; return; }
            UIController.instance.OpenShop(shopData, this);
        }
    }
}

