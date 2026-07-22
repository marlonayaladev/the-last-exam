using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameplayManager : MonoBehaviour
{
	public static GameplayManager Instance { get; private set; }
	public static bool Restart { get; set; }

	enum GameState
	{
		Tutorial,
		InGame,
		GameOver,
	};
	GameState state = GameState.Tutorial;

	PlayerRank rank = PlayerRank.Gold;
	float timeSpent = 0f;
	int score = 0;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		UIManager.Instance.ShowHUD(false);
		UIManager.Instance.UpdateHUD(score, timeSpent, rank);

		if (Restart)
		{
			Restart = false;
			OnStartGame();
		}
		else
		{
			UIManager.Instance.ShowScreen("Tutorial");
		}
	}

	void Update()
	{
		if (CanPlay())
		{
			timeSpent += Time.deltaTime;
			rank = LevelManager.Instance.GetRank(timeSpent, score);
			UIManager.Instance.UpdateHUD(score, timeSpent, rank);
		}
	}

	void ReloadScene()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
	}

	public void OnStartGame()
	{
		state = GameState.InGame;
		UIManager.Instance.ShowHUD(true);
		UIManager.Instance.ShowScreen("");
	}

	public void OnRestart()
	{
		Restart = true;
		Invoke("ReloadScene", 0.5f);
	}

	public void OnPickup()
	{
		++score;
		rank = LevelManager.Instance.GetRank(timeSpent, score);
		UIManager.Instance.UpdateHUD(score, timeSpent, rank);
	}

	public void OnPlayerCaught()
	{
		state = GameState.GameOver;
		UIManager.Instance.ShowScreen("GameOver");
	}

	public void OnGoal()
	{
		state = GameState.GameOver;
		StartCoroutine(ShowScreamer());
	}

	IEnumerator ShowScreamer()
	{
		UIManager.Instance.ShowHUD(false);
		UIManager.Instance.ShowScreen("");

		// Create full-screen Canvas
		Canvas canvas = new GameObject("ScreamerCanvas").AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 999;
		canvas.gameObject.AddComponent<CanvasScaler>();
		canvas.gameObject.AddComponent<GraphicRaycaster>();

		// Create Image
		GameObject imgObj = new GameObject("ScreamerImage");
		imgObj.transform.SetParent(canvas.transform, false);
		RectTransform rect = imgObj.AddComponent<RectTransform>();
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;

		Image img = imgObj.AddComponent<Image>();
		Texture2D tex = Resources.Load<Texture2D>("HD-wallpaper-the-exorcist");
		if (tex != null)
		{
			Rect spriteRect = new Rect(0, 0, tex.width, tex.height);
			img.sprite = Sprite.Create(tex, spriteRect, new Vector2(0.5f, 0.5f));
		}
		img.preserveAspect = false;

		// Play scare sound if available
		AudioSource src = canvas.gameObject.AddComponent<AudioSource>();
		AudioClip clip = Resources.Load<AudioClip>("Scary Screamer - Sound Effect (Free)");
		if (clip != null) src.PlayOneShot(clip);

		// Hide everything else
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		// Wait then restart
		yield return new WaitForSeconds(3f);
		OnRestart();
	}

	public bool CanPlay()
	{
		return (state == GameState.InGame);
	}

	public void OnLanguageChanged()
	{
		UIManager.Instance.OnLanguageChanged();
		UIManager.Instance.UpdateHUD(score, timeSpent, rank);
	}
}
