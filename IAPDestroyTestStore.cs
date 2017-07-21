// (c) Copyright HutongGames, LLC 2010-2017. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("IAP")]
	[Tooltip("Destroy test store. Do not use for production.")]
	public class IAPDestroyTestStore : FsmStateAction
	{

		[UIHint(UIHint.Variable)]
		[Tooltip("Game object that contains the test store.")]
		public FsmGameObject storeObject;
		
		public override void Reset()
		{
			
			storeObject =  null;

		}
		
		public override void OnEnter()
		{
			Object.Destroy(storeObject.Value);
			Finish();
		}
		
	}
}