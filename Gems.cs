using UnityEngine;
using System.Collections;

public class Gems : MonoBehaviour
{

	private int gemType = 1;

	public int GemType { get { return gemType; } }

	//position in the board logic matrix
	private int posX = -1;
	private int posZ = -1;

	private bool gemIsSelected = false;

	public bool GemIsSelected  { get { return gemIsSelected; } }

	private Color gemColor;

	public Color GemColor  { get { return gemColor; } }

	private GameManager gBoard;

	void Start ()
	{
       
	}

	void OnMouseOver ()
	{
		if (Input.GetMouseButtonDown (0))
		{
			gBoard.SelectAdjacentGems (posX, posZ, gemType);
		}
		GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0.5f);
	}

	void OnMouseExit ()
	{
		GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1f);
	}

	public void SetColor (Color c)
	{
		GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1f);
		gemColor = c;
	}

	public void SelectGem (bool paintGem = true)
	{
		if (paintGem)
			GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0.5f);
		gemIsSelected = true;
	}

	public void UnSelectGem ()
	{
		GetComponent<SpriteRenderer> ().color = gemColor;
		gemIsSelected = false;
	}

	public void SetTypeAndReference (int type, GameManager gm)
	{
		gBoard = gm;
		gemType = type;
	}

	public void SetGemPos (int i, int j)
	{
		posX = i;
		posZ = j;
	}
}
