using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scenes
{
	public static class MySceneManager
	{
		private const int MaxBrains = 100;

		static MySceneManager()
		{
			Brains = MaxBrains;
		}

		public static void CompleteVisualNovellScene()
		{
			LoadNextLevel();
		}


		public static void CompleteTowerDefenceScene()
		{
			Brains = GameObject.FindObjectOfType<PlayerIngameManager>().Brains;

			LoadNextLevel();
		}

		public static int GetBrains()
		{
			return Brains;
		}

		private static int Brains;

		private static void LoadNextLevel()
		{
			// TODO: 111
			return;
			var scene = SceneManager.GetActiveScene().name;
			SceneManager.LoadSceneAsync(GetNextScene(scene));
		}

		private static string GetNextScene(string scene)
		{
			switch (scene)
			{
				case "main_menu_scene":
					return "vn_1_scene";
				case "vn_1_scene":
					return "tower_1_scene";
				case "tower_1_scene":
					return "vn_2_scene";
				case "vn_2_scene":
					return "tower_2_scene";
				case "tower_2_scene":
					return "vn_3_scene";
				case "vn_3_scene":
					return "tower_3_scene";
				case "tower_3_scene":
					return "vn_final_scene";
				case "vn_final_scene":
					return "titles_scene";
				case "titles_scene":
					return "main_menu_scene";
				default:
					Debug.LogError("Сцена следующая для : " + scene + " не найдена.");
					return "";
			}
		}
	}
}
