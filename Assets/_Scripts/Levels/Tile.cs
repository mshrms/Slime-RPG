using UnityEngine;

// TODO: убрать наследование от монобеха где оно не нужно
public class Tile : MonoBehaviour
{
	// для первой клетки это точка спавна игрока, а для остальных - точка его остановки
	public Transform playerPosition;

	public Transform enemiesPosition;
	public Vector2 enemiesSpawnBoxSize;
}