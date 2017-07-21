// (c) Copyright HutongGames, LLC 2010-2017. All rights reserved.

using UnityEngine;
using UnityEngine.Purchasing;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("IAP")]
	[Tooltip("Get information about existing product by ID in the Unity IAP catalog.")]
	public class IAPGetProductInfo : FsmStateAction
	{
		
		[RequiredField]
		[Tooltip("Enter the product ID")]
		public FsmString productId;
		
		[ActionSection("Results")]
		
		[UIHint(UIHint.Variable)]
		public FsmString productTitle;
		
		[UIHint(UIHint.Variable)]
		public FsmString productDescription;
		
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(ProductType))]
		public FsmEnum productEnum;
		
		[ActionSection("Options")]
		
		public FsmBool everyFrame;
		
		private string _productId;
		private bool checkProduct;
		
		public override void Reset()
		{
			
			everyFrame = false;
			productTitle = null;
			productDescription = null;
			
		}
		
		
		public override void OnEnter()
		{
			
			doCheck();
			if (!everyFrame.Value)
				Finish();		
		}
		
		public override void OnUpdate()
		{
			
			doCheck();
			
		}
		
		void doCheck()
		
		{
			_productId = productId.Value;
			checkProduct = StoreManager.Instance.HasProductInCatalog(_productId);
			if(checkProduct)
			{
				
				StoreManager.Instance.productInformation(_productId);
				productTitle.Value = StoreManager.Instance.currentProduct.defaultDescription.Title;
				productDescription.Value = StoreManager.Instance.currentProduct.defaultDescription.Description;
				productEnum.Value = StoreManager.Instance.currentProduct.type;
				
			}

            else
			{
				Debug.Log("This product does not exist in your catalog. Do you have the right product ID?");
				return;
			}
		}
		
		
		public class StoreManager
		{
			
			private static StoreManager instance = new StoreManager();
			private ProductCatalog catalog;
            public ProductCatalogItem currentProduct;
            			
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
			
			// Get Product Information
			public void productInformation(string productID)
			{
				foreach (var product in catalog.allProducts) {
					if (product.id == productID) 
					{
						currentProduct = product;

                    }
				}
			}
			
		}
		
	}
	
}
