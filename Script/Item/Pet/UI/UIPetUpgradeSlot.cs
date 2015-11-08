using UnityEngine;
using System.Collections;

public class UIPetUpgradeSlot : MonoBehaviour {

	#region - serialized -
	[SerializeField] UIPetImg icon;
	[SerializeField] Animation effect;
	[SerializeField] SpriteText txt_Mat;
	#endregion
	#region - member -
	#endregion
	#region - init & release -
	#endregion
	#region - call back -
	#endregion
	#region - public -
	public void SetSlot(PetListElement _element)
	{
		icon.SetSlotImg(_element.PetRecord.Icon);
//		txt_Mat.Text = _element.PetName;
	}

	public void Clear()
	{
		icon.DeleteSlotImg();
//		txt_Mat.Text = "";
	}
	#endregion
	#region - method -
	#endregion
	#region - delegate -

	#endregion
}
