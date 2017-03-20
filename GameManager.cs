using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	public GameObject ResetLevelText;
	public GameObject ResetLevelButton;
	public GameObject ExitLevelButton;
	public GameObject[] _Gems;
   
	public int boardWidth = 6;
	public int boardHeight = 9;
	private GameObject[,] board;

	private float gemSize = 0.8f;
	AudioSource audio; 
	public AudioSource matchedAudio;
	//contains the list with adjacent objects a gem has when user clicks
	private List<GameObject> userSelectedGems;
	private List<List<GameObject>> possibleMoves;

	private int score = 0;

	public int Score { get { return score; } }


	public class Pos
	{
		public int i { get; set; }

		public int j { get; set; }

		public Pos (int _i, int _j)
		{
			i = _i;
			j = _j;
		}
	}

	void Awake()
	{
		ResetLevelText.SetActive(false);
		ResetLevelButton.SetActive(false);
		ExitLevelButton.SetActive(false);
	}


	void Start ()
	{
		audio = GetComponent<AudioSource> ();
		userSelectedGems = new List<GameObject> ();
		possibleMoves = new List<List<GameObject>> ();
      
		GenerateGame ();
	}

	private void GenerateGame ()
	{
		board = new GameObject[boardWidth, boardHeight];
		for (int i = 0; i < boardWidth; i++)
		{
			for (int j = 0; j < boardHeight; j++)
			{
				board [i, j] = GenerateGemAtPos (i, j);
			}
		}
	}

	private GameObject GenerateGemAtPos (int i, int j, int gemType = -99)
	{
		int generatedGemType = gemType;
		if (generatedGemType == -99)
			generatedGemType = Random.Range (0, _Gems.Length);

		GameObject selectedGem = _Gems [generatedGemType];
		GameObject generatedGem = Instantiate (selectedGem, GetGemPos (i, j), selectedGem.transform.rotation) as GameObject;
		generatedGem.AddComponent<Gems> ();
		generatedGem.GetComponent<Gems> ().SetGemPos (i, j);
		generatedGem.GetComponent<Gems> ().SetTypeAndReference (generatedGemType, this);
		generatedGem.GetComponent<Gems> ().SetColor (_Gems [generatedGemType].GetComponent<Renderer> ().material.color);
		generatedGem.SetActive (true);
		generatedGem.name = "Gem";
		return generatedGem;
	}

	private Vector3 GetGemPos (int i, int j)
	{
		float posX = i * gemSize - ((float)boardWidth) / 4 + ((boardWidth % 2 == 0) ? gemSize : gemSize / 2);
		float posZ = j * gemSize - ((float)boardHeight) / 4 + ((boardHeight % 2 == 0) ? gemSize : gemSize / 2);
		float posY = gemSize / 2;

		return new Vector3 (posX, posZ, posY);
	}

	void Update ()
	{
		ApplyGameLogic ();

		if (Input.GetMouseButtonUp (0))
		{
			DeselectAllGems ();
			OutOfMoves ();
		}

	}

	private void ApplyGameLogic ()
	{
		if (userSelectedGems.Count > 2)
		{
			ScoreManager.score += score + (userSelectedGems.Count * 100);
			for (int i = 0; i < userSelectedGems.Count; i++)
			{
				matchedAudio.Play();
				Destroy (userSelectedGems [i]);
				//matchedAudio.PlayOneShot;
			}
		}
		userSelectedGems.Clear ();

		while (NeedToFillBoard ())
		{
			GenerateTopGems ();
			ApplyGravity ();
		}
	}

    
	private void OutOfMoves ()
	{
		possibleMoves = GetAvailableMoves ();
		if (possibleMoves.Count == 0)
		{
			//Show UI elements
			audio.pitch = 0.5f;
			ResetLevelText.SetActive (true);
			ResetLevelButton.SetActive (true);
			ExitLevelButton.SetActive (true);

		}
	}

	public void ResetLevel ()
	{
		// Reset level
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}


	private bool ValidPosition (Pos p)
	{
		return p.i >= 0 && p.i < boardWidth && p.j >= 0 && p.j < boardHeight;
	}

	//Gets all available moves
	private List<List<GameObject>> GetAvailableMoves ()
	{
		List<List<GameObject>> availableMoves = new List<List<GameObject>> ();
		for (int i = 0; i < boardWidth; i++)
		{
			for (int j = 0; j < boardHeight; j++)
			{
				SelectAdjacentGems (i, j, board [i, j].GetComponent<Gems> ().GemType, false/*dont paint selected gems*/);
				if (userSelectedGems.Count > 2)
				{
					availableMoves.Add (new List<GameObject> (userSelectedGems));
				}
				for (int k = 0; k < userSelectedGems.Count; k++)
					userSelectedGems [k].GetComponent<Gems> ().UnSelectGem ();
				userSelectedGems.Clear ();
			}
		}
		return availableMoves;
	}

	//Checks if the board needs to be filled
	private bool NeedToFillBoard ()
	{
		for (int i = 0; i < boardWidth; i++)
		{
			for (int j = 0; j < boardHeight; j++)
			{
				if (board [i, j] == null)
					return true;
			}
		}
		return false;
	}

	//Moves Gems when other Gems are cleared form the board.
	private void ApplyGravity ()
	{
		for (int i = 0; i < boardWidth; i++)
		{
			for (int j = 0; j < boardHeight; j++)
			{
				if (j - 1 >= 0)
				{
					if (board [i, j - 1] == null && board [i, j] != null)
					{
						board [i, j - 1] = board [i, j];
						board [i, j - 1].transform.position = GetGemPos (i, j - 1);
						board [i, j - 1].GetComponent<Gems> ().SetGemPos (i, j - 1);
						board [i, j] = null;
					}
				}
			}
		}
	}

	//Creates Gems for the top row when other gems are cleared from the board.
	private void GenerateTopGems ()
	{
		for (int i = 0; i < boardWidth; i++)
		{
			if (board [i, boardHeight - 1] == null)
				board [i, boardHeight - 1] = GenerateGemAtPos (i, boardHeight - 1);
		}
	}


	public void SelectAdjacentGems (int i, int j, int gemType, bool paintSelected = true)
	{
		board [i, j].GetComponent<Gems> ().SelectGem (paintSelected);

		userSelectedGems.Add (board [i, j]);

		if (i + 1 < boardWidth)
		if (board [i + 1, j] != null && board [i + 1, j].GetComponent<Gems> ().GemType == gemType &&
		    !board [i + 1, j].GetComponent<Gems> ().GemIsSelected)
			SelectAdjacentGems (i + 1, j, gemType, paintSelected);
		if (i - 1 >= 0)
		if (board [i - 1, j] != null && board [i - 1, j].GetComponent<Gems> ().GemType == gemType &&
		    !board [i - 1, j].GetComponent<Gems> ().GemIsSelected)
			SelectAdjacentGems (i - 1, j, gemType, paintSelected);


		if (j + 1 < boardHeight)
		if (board [i, j + 1] != null && board [i, j + 1].GetComponent<Gems> ().GemType == gemType &&
		    !board [i, j + 1].GetComponent<Gems> ().GemIsSelected)
			SelectAdjacentGems (i, j + 1, gemType, paintSelected);
		if (j - 1 >= 0)
		if (board [i, j - 1] != null && board [i, j - 1].GetComponent<Gems> ().GemType == gemType &&
		    !board [i, j - 1].GetComponent<Gems> ().GemIsSelected)
			SelectAdjacentGems (i, j - 1, gemType, paintSelected);
	}

	private void DeselectAllGems ()
	{
		for (int i = 0; i < boardWidth; i++)
		{
			for (int j = 0; j < boardHeight; j++)
			{
				if (board [i, j] != null)
					board [i, j].GetComponent<Gems> ().UnSelectGem ();
			}
		}
	}

	public void loadLevel(string leveltoLoad)
	{

		// load the specified level
		SceneManager.LoadScene (leveltoLoad);
	}
		
}