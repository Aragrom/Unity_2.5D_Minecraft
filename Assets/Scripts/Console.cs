using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;

/// <summary>
/// A console to display Unity's debug logs in-game.
/// </summary>
[BurstCompile]
public class Console : MonoBehaviour
{
	struct Log
	{
		public string message;
		public string stackTrace;
		public LogType type;
	}

	public InputEngine inputEngine = null;

	List<Log> logs = new List<Log>();
	Vector2 scrollPosition;
	bool show;
	bool collapse;

	// Visual elements:

	static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
	{
		{ LogType.Assert, Color.white },
		{ LogType.Error, Color.red },
		{ LogType.Exception, Color.red },
		{ LogType.Log, Color.white },
		{ LogType.Warning, Color.yellow },
	};

	const int margin = 20;

	Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
	Rect titleBarRect = new Rect(0, 0, 10000, 20);
	GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
	GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

	[BurstCompile]
	void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	[BurstCompile]
	void OnDisable()
	{
		Application.logMessageReceived += null;
	}

	[BurstCompile]
	private void Awake()
    {
		inputEngine = GameObject.Find("Input Engine").GetComponent<InputEngine>();
    }

	[BurstCompile]
	void Update()
	{
		if (inputEngine.F1)
		{
			inputEngine.F1 = false;

			show = !show;
		}

		if (logs.Count > 20)
		{
			logs.RemoveRange(0, 10);
		}
	}

	[BurstCompile]
	void OnGUI()
	{
		if (!show)
		{
			return;
		}

		windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
	}

	/// <summary>
	/// A window that displayss the recorded logs.
	/// </summary>
	/// <param name="windowID">Window ID.</param>
	[BurstCompile]
	void ConsoleWindow(int windowID)
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		// Iterate through the recorded logs.
		for (int i = 0; i < logs.Count; i++)
		{
			var log = logs[i];

			// Combine identical messages if collapse option is chosen.
			if (collapse)
			{
				var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

				if (messageSameAsPrevious)
				{
					continue;
				}
			}

			GUI.contentColor = logTypeColors[log.type];
			GUILayout.Label(log.message);
		}

		GUILayout.EndScrollView();

		GUI.contentColor = Color.white;

		GUILayout.BeginHorizontal();

		if (GUILayout.Button(clearLabel))
		{
			logs.Clear();
		}

		collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

		GUILayout.EndHorizontal();

		// Allow the window to be dragged by its title bar.
		GUI.DragWindow(titleBarRect);
	}

	/// <summary>
	/// Records a log from the log callback.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="stackTrace">Trace of where the message came from.</param>
	/// <param name="type">Type of message (error, exception, warning, assert).</param>
	[BurstCompile]
	void HandleLog(string message, string stackTrace, LogType type)
	{
		logs.Add(new Log()
		{
			message = message,
			stackTrace = stackTrace,
			type = type,
		});
	}
}
