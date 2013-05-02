using UnityEngine;
using System.Collections;

public class Menu2DSliderIndicator : MonoBehaviour {

	void onMoved(Vector2 deltaMovement)
	{
		transform.position += new Vector3(deltaMovement.x, deltaMovement.y);
	}
}
