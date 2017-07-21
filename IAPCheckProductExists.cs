// (c) Copyright HutongGames, LLC 2010-2017. All rights reserved.

using UnityEngine;
using UnityEngine.Purchasing;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("IAP")]
	[Tooltip("Check by ID if a product exists within the current IAP Unity catalog.")]
	public class IAPCheckProductExists : FsmStateAction
	{
		
		[RequiredField]
		[Tooltip("Enter the product ID")]
		public FsmString productId;
		
		[ActionSection("Events")]
		
		[Tooltip("This event will fire if the product ID is found within the current IAP catalog")]
		public FsmEvent hasProduct;
		
		[Tooltip("This event will fire if the product ID is not found within the current IAP catalog")]
		public FsmEvent missingProduct;
				
		private string _productId;
		private bool checkProduct;
		
			
		public override void Reset()
		{

			productId = null;
			hasProduct = null;
			missingProduct = null;
		}

		
		public override void OnEnter()
		{
			
			doCheck();
			
		}

		void doCheck()
		{
			_productId = productId.Value;
			checkProduct = StoreManager.Instance.HasProductInCatalog(_productId);
			
			if(checkProduct)
			{
				Fsm.Event(hasProduct);
				
			}
			else
			{
				Fsm.Event(missingProduct);
			}
		}

		
		public class StoreManager
		{
			
			private static StoreManager instance = new StoreManager();
			private ProductCatalog catalog;

            private StoreManager()
			{
				catalog = ProductCatalog.LoadDefaultCatalog();
				
				StandardPurchasingModule module = StandardPurchasingModule.Instance();
				
				ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
				foreach (var product in catalog.allProducts) {
					if (product.allStoreIDs.Count > 0) {
						var ids = new IDs();
						foreach (var storeID in product.allStoreIDs) {
							ids.Add(storeID.id, storeID.store);
						}
						builder.AddProduct(product.id, product.type, ids);
					} else {
						builder.AddProduct(product.id, product.type);
					}
				}
			}
			
			public static StoreManager Instance {
				get {
					return instance;
				}
			}

            // Check to See if Items Exist in Catalog
            public bool HasProductInCatalog(string productID)
			{
				foreach (var product in catalog.allProducts) {
					if (product.id == productID) {
						return true;
					}
				}
				return false;
			}
			
		}
		
	}
		
}
