using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.TransitionKit;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour {


	// static reference for sceneManager
	public static SceneTransitionManager manager;

	public Texture2D maskTexture;
	private bool _isUiVisible = true;
	private int m_sceneIndex = 1;


	void Awake()
	{
		DontDestroyOnLoad( gameObject );

		// setup reference to game manager
		if (manager == null)
			manager = this.GetComponent<SceneTransitionManager>();

		SceneManager.LoadScene( m_sceneIndex );
	}

	public void TransitionToScene(int sceneIndex) {
		TransitionFadeToScene (sceneIndex);
	}

	public void TransitionFadeToScene(int sceneIndex) {
		m_sceneIndex = sceneIndex;
		var fader = new FadeTransition()
		{
			nextScene = m_sceneIndex,
			fadedDelay = 0.2f,
			fadeToColor = Color.black
		};
		TransitionKit.instance.transitionWithDelegate( fader );
	}

	public void TransitionMaskToScene(int sceneIndex) {
		m_sceneIndex = sceneIndex;
		var mask = new ImageMaskTransition()
		{
			maskTexture = maskTexture,
			backgroundColor = Color.yellow,
			nextScene = SceneManager.GetActiveScene().buildIndex == 1 ? 2 : 1
		};
		TransitionKit.instance.transitionWithDelegate( mask );
	}

	public void TransitionBigSquareToScene(int sceneIndex) {
		m_sceneIndex = sceneIndex;
		var squares = new SquaresTransition()
		{
			nextScene = SceneManager.GetActiveScene().buildIndex == 1 ? 2 : 1,
			duration = 2.0f,
			squareSize = new Vector2( 5f, 4f ),
			smoothness = 0.0f
		};
		TransitionKit.instance.transitionWithDelegate( squares );
	}

	public void TransitionWindToScene(int sceneIndex) {
		m_sceneIndex = sceneIndex;
		var wind = new WindTransition()
		{
			nextScene = SceneManager.GetActiveScene().buildIndex == 1 ? 2 : 1,
			duration = 1.0f,
			size = 0.3f
		};
		TransitionKit.instance.transitionWithDelegate( wind );
	}


	void OnEnable()
	{
		TransitionKit.onScreenObscured += onScreenObscured;
		TransitionKit.onTransitionComplete += onTransitionComplete;
	}


	void OnDisable()
	{
		// as good citizens we ALWAYS remove event handlers that we added
		TransitionKit.onScreenObscured -= onScreenObscured;
		TransitionKit.onTransitionComplete -= onTransitionComplete;
	}


	void onScreenObscured()
	{
		_isUiVisible = false;
	}


	void onTransitionComplete()
	{
		if (m_sceneIndex == 2) {
			GameManager.gm.ActivateScene ();
		}

		_isUiVisible = true;
	}
}
