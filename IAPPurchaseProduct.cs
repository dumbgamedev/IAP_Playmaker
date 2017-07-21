// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.
using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("IAP")]
	[Tooltip("Purchase a product from the Unity IAP store.")]
	public class IAPPurchaseProduct : FsmStateAction
	{

		[RequiredField]
		[Tooltip("Products can be added to the store by using Window > Unity IAP > IAP Catalog.")]
		[Title("Product ID")]
		public FsmString _productId;
		
		[ActionSection("Purchase Results")]
		
		[Tooltip("This event will fire when the product has successfully been purchased")]
		public FsmEvent success;
		
		[Tooltip("This event will fire if the product has not successfully been purchased")]
		public FsmEvent failure;
		
		[ActionSection("Error Events")]
		
		[Tooltip("This event will fire if the product ID is not found within the current IAP catalog")]
		public FsmEvent missingProduct;
		
		[Tooltip("This event will fire if initialization to the store has failed")]
		public FsmEvent initializationFailure;
	
		private string productId;

		public override void Reset()
		{
			_productId = null;
			missingProduct = null;
			initializationFailure = null;
			success = null;
			failure = null;
		}

		
		public override void OnEnter()
		{
			productId = _productId.Value;
			PurchaseProduct();
		}

		void PurchaseProduct()
		{
			
			// Check if product exists
			var hasProduct = StoreManager.Instance.HasProductInCatalog(productId);
			
			// This product exists, so continue to initialize the purchase
			if(hasProduct)
			{
				
				 // Start to buy this product by ID. Check to see if there is a initlization failure.
				var initilizationSuccess = StoreManager.Instance.InitiatePurchase(productId);
				
				// Initilization success was false (failed), therefore finish or do failure event.
				if(!initilizationSuccess)
				{
					
					if(initializationFailure == null)
					{
						Finish();
					}
					else
					{
						Fsm.Event(initializationFailure);
					}
					
				}		

			}
			
			// No product by this ID was found.
			else
			{
				Debug.Log("This product does not exist in the catalog");
				
				if(missingProduct == null)
				{
					Finish();
				}
				else
				{
					Fsm.Event(missingProduct);
				}
			}				

		}
		
		
		public override void OnUpdate()
		{
			
			// Check for success and then fire event.
			if(StoreManager.Instance.purchaseSuccess)
			{
                if (success == null)
                {
                    Finish();
                }

                else
                {
                    Fsm.Event(success);
                }
            }
			
			// Check for failure and then fire event.
			if(StoreManager.Instance.purchaseSuccess)
			{

                if (failure == null)
                {
                    Finish();
                }

                else
                {
                    Fsm.Event(failure);
                }
			}
			
		}
		

		public class StoreManager : IStoreListener
		{
			private static StoreManager instance = new StoreManager();
			private ProductCatalog catalog;
			
			protected IStoreController controller;
			protected IExtensionProvider extensions;
			
			public bool purchaseSuccess;
			public bool purchaseFailure;
			
			private StoreManager()
			{
				catalog = ProductCatalog.LoadDefaultCatalog();
				
				StandardPurchasingModule module = StandardPurchasingModule.Instance();
				module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
				
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
				UnityPurchasing.Initialize (this, builder);
			}
			
			public static StoreManager Instance {
				get {
					return instance;
				}
			}
			
			public IStoreController StoreController {
				get {
					return controller;
				}
			}
			
			public IExtensionProvider ExtensionProvider {
				get {
					return extensions;
				}
			}
			
			public bool HasProductInCatalog(string productID)
			{
				foreach (var product in catalog.allProducts) {
					if (product.id == productID) {
						return true;
					}
				}
				return false;
			}
			
			public Product GetProduct(string productID)
			{
				if (controller != null) {
					return controller.products.WithID(productID);
				}
				return null;
			}
			
			
			public bool InitiatePurchase(string productID)
			{
				if (controller == null) {
					return false;
				}
				
				controller.InitiatePurchase(productID);
                return true;
			}
			
			public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
			{
				this.controller = controller;
				this.extensions = extensions;

			}
			
			public void OnInitializeFailed (InitializationFailureReason error)
			{
                Debug.Log("OnInitializeFailed. InitializationFailureReason:" + error);
            }

            public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
			{

				purchaseSuccess = true;
				return PurchaseProcessingResult.Complete;
			}
			
			public void OnPurchaseFailed (Product product, PurchaseFailureReason reason)
			{
				purchaseFailure = true;
				
				} 
			}
		}
	}