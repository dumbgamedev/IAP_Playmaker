// (c) Copyright HutongGames, LLC 2010-2017. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("IAP")]
	[Tooltip("Enable test store. Do not use for production.")]
	public class IAPCreateTestStore : FsmStateAction
	{

		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;
		
		public override void Reset()
		{
			
			storeObject =  null;

		}
		
		public override void OnEnter()
		{
			var newObject = new GameObject( "TestStore" );
			newObject.AddComponent<IAPSimulator>();
			storeObject.Value = newObject;
			Finish();
		}
	}
}