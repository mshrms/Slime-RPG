using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameStates
{
	// Начало игры / ожидание атаки на новый тайл (ожидание тапа на поле)
	TapToStart, 
	// Процесс автобоя на тайле (тапы по полю заблокированы)
	Game,
	// Победа в бою на тайле (появление на несколько секунд надписи победа, тапы заблокированы), затем режим сменяется на тап ту старт
	Win, 
	// Проигрыш в бою на тайле, игрок умер, перезапуск текущего уровня с первого этапа (чтобы пофармить монеты)
	DeadScreen,
	// Победа на всех уровнях, показ кнопки с рестартом игры
	GameEnd
}

public class GameManager : MonoBehaviour
{
	// многофункциональная кнопка для переключения игровых режимов
	public Button tapToStartButton;
	
	// подсказка тап ту старт
	public GameObject tapToStartScreen;

	// победа на одном уровне, тап досрочно убирает надпись победы
	public GameObject winPanel;
	public float winPanelShowDuration;

	// игрок умер, тап для начала уровня заново
	public GameObject deadScreen;

	// победа на всех уровнях, тап для новой игры
	public GameObject gameEndScreen;

	private GameStates gameState;

	public GameStates GameState
	{
		get => gameState;
		set
		{
			gameState = value;
			ChangeGameState();
		} 
	}

	#region Events

	public static Action StartStage;
	public static Action GameRestart;
	public static Action DeadScreen;
	public static Action WinScreen;
	public static Action GameEnd;

	#endregion

	private void Awake()
	{
		tapToStartButton.onClick.AddListener(HandleGameFieldTap);
		GameRestart += RestartGame;
		WinScreen += ShowWinScreen;

		GameState = GameStates.TapToStart;
	}

	private void OnDestroy()
	{
		tapToStartButton.onClick.RemoveAllListeners();
		GameRestart -= RestartGame;
		WinScreen -= ShowWinScreen;

	}


	private void ShowWinScreen()
	{
		GameState = GameStates.Win;
	}
	
	private void RestartGame()
	{
		SceneManager.LoadScene(0);
	}

	/// <summary>
	/// Определяет действие после нажатия кнопки на поле в зависимости от игрового состояния
	/// </summary>
	private void HandleGameFieldTap()
	{
		switch (GameState)
		{
			case GameStates.TapToStart:
				GameState = GameStates.Game;
				StartStage?.Invoke();
				break;
			
			case GameStates.Game:
				// не реагировать
				break;
			
			case GameStates.Win:
				// скрыть надпись победы и включить подсказку для старта нового этапа
				StopAllCoroutines();
				GameState = GameStates.TapToStart;
				break;
			
			case GameStates.DeadScreen:
				GameState = GameStates.TapToStart;
				break;
			
			case GameStates.GameEnd:
				GameRestart?.Invoke();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void ChangeGameState()
	{
		switch (GameState)
		{
			case GameStates.TapToStart:
				tapToStartScreen.SetActive(true);
				winPanel.SetActive(false);
				break;
			
			case GameStates.Game:
				tapToStartScreen.SetActive(false);
				winPanel.SetActive(false);
				break;
			
			case GameStates.Win:
				StartCoroutine(ShowWinPanel());
				break;
			
			case GameStates.DeadScreen:
				deadScreen.SetActive(true);
				break;
			
			case GameStates.GameEnd:
				gameEndScreen.SetActive(true);
				break;
			
			default:
				Debug.LogError("Unknown State");
				break;
		}
	}

	private IEnumerator ShowWinPanel()
	{
		winPanel.SetActive(true);
		
		for (float t = 0; t < winPanelShowDuration; t += Time.deltaTime)
		{
			yield return null;
		}

		winPanel.SetActive(false);
		GameState = GameStates.TapToStart;
	}
}
