// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using UnityEngine.Purchasing;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("IAP")]
	[Tooltip("Print all products within the catalog to a debug log.")]
	public class IAPPrintAllProducts : FsmStateAction
	{
		public override void OnEnter()
		{
			StoreManager.Instance.allCatalog();
			Finish();
		}

		public class StoreManager
		{
			
			private static StoreManager instance = new StoreManager();
			private ProductCatalog catalog;
			public int _totalProducts;
			
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
			
			// Print entire catalog of products to the debug log
			public void allCatalog()
			{
				int i = 0;
				foreach (var product in catalog.allProducts) 
				{
					i ++;
					Debug.Log ("#" + i + ". Item ID: " + product.id + ". Type: " + product.type +  ". Title: " + product.defaultDescription.Title + ". Description: "  + product.defaultDescription.Title);

				}
			}
		}
	}
}
