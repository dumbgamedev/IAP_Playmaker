// (c) Copyright HutongGames, LLC 2010-2017. All rights reserved.

using UnityEngine;
using UnityEngine.Purchasing;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("IAP")]
	[Tooltip("Restore product purchases from the Unity IAP store.")]
	public class IAPRestorePurchase : FsmStateAction
	{

		[ActionSection("Purchase Results")]
		
		[Tooltip("This event will fire if the store has been successfully restored")]
		public FsmEvent success;
		
		[Tooltip("This event will fire if the store has not been successfully restored")]
		public FsmEvent failure;
		
		[ActionSection("Error Events")]

		[Tooltip("This event will fire if initialization to the store has failed")]
		public FsmEvent initializationFailure;
	
		private bool RestoreSuccess = false;
		private bool RestoreFailure = false;

		public override void Reset()
		{

			initializationFailure = null;
			success = null;
			failure = null;
			
		}

		
		public override void OnEnter()
		{
			
			Restore();			
		}

		
		public override void OnUpdate()
		{
			
			// Check for success and then fire event.
			if(RestoreSuccess)
			{
				
				Fsm.Event(success);
				
			}
			
			// Check for failure and then fire event.
			if(RestoreFailure)
			{
				
				Fsm.Event(failure);
				
			}
			
			if(StoreManager.Instance.initilizationFailure)
			{
				Fsm.Event(initializationFailure);
				
			}
			
		}
		
		
		void Restore()
		{
			
			if (Application.platform == RuntimePlatform.WSAPlayerX86 || Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.WSAPlayerARM) 
			{
				StoreManager.Instance.ExtensionProvider.GetExtension<IMicrosoftExtensions>().RestoreTransactions();
			} 
			
			else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.tvOS) 
			{
				StoreManager.Instance.ExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(OnTransactionsRestored);
			} 
			
			else if (Application.platform == RuntimePlatform.Android && StandardPurchasingModule.Instance().androidStore == AndroidStore.SamsungApps) 
			{
				StoreManager.Instance.ExtensionProvider.GetExtension<ISamsungAppsExtensions>().RestoreTransactions(OnTransactionsRestored); 
			} 
			
			else if (Application.platform == RuntimePlatform.Android && StandardPurchasingModule.Instance().androidStore == AndroidStore.CloudMoolah) 
			{
				StoreManager.Instance.ExtensionProvider.GetExtension<IMoolahExtension>().RestoreTransactionID((restoreTransactionIDState) => 
				{ 
					OnTransactionsRestored(restoreTransactionIDState != RestoreTransactionIDState.RestoreFailed && restoreTransactionIDState != RestoreTransactionIDState.NotKnown);
				});
			}
			
			else {
				
				Debug.LogWarning(Application.platform.ToString() + " is not a supported platform for the restore button");
				Debug.Log("This is not a supported platform for the restore button");
				
				RestoreFailure = true;
			}
			
		}
		
		void OnTransactionsRestored(bool success)
		{
			RestoreSuccess = true;
		}

		public class StoreManager : IStoreListener
		{
			private static StoreManager instance = new StoreManager();
			private ProductCatalog catalog;
			
			protected IStoreController controller;
			protected IExtensionProvider extensions;
			
			public bool initilizationFailure = false;

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

			public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
			{
				this.controller = controller;
				this.extensions = extensions;

			}
			
			public void OnInitializeFailed (InitializationFailureReason error)
			{
				initilizationFailure = true;
				
            }

            public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
			{

				return PurchaseProcessingResult.Complete;
			}
			
			public void OnPurchaseFailed (Product product, PurchaseFailureReason reason)
			{
				
			} 
		}
	}
}