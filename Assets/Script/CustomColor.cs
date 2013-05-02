using UnityEngine;

public static class CustomColor 
{
	//in unity i colori rgb vanno normalizzati tra 0 - 1 quindi bisogna prendere il valore rgb e dividerlo per 255 (es. 155/255 = 0.607f)
	public static Color Red 	{ get { return new Color(1f, 0f, 0f); } }
	public static Color Green 	{ get { return new Color(0f, 1f, 0f); } }
	public static Color Blue 	{ get { return new Color(0f, 0f, 1f); } }
	public static Color Yellow	{ get { return new Color(1f, 1f, 0f); } }
	public static Color Pink	{ get { return new Color(1f, 0f, 0.607f); } }
	public static Color Cyan 	{ get { return new Color(0f, 1f, 1f); } }
	public static Color Orange 	{ get { return new Color(1f, 0.372f, 0f); } }
	public static Color Violet 	{ get { return new Color(0.607f, 0f, 1f); } }
	
	public static Color[] List = { 
		CustomColor.Red,	
		CustomColor.Green, 
		CustomColor.Blue, 
		CustomColor.Yellow, 
		CustomColor.Pink,	
		CustomColor.Cyan, 
		CustomColor.Orange, 
		CustomColor.Violet 
	};
}
