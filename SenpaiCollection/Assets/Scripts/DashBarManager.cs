using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashBarManager : MonoBehaviour {

	private Image m_dashBar;
	private float m_maxWidth;

	private float m_currentWidth;


	void Awake() {
		m_dashBar = gameObject.GetComponent<Image> ();
	}

	// Use this for initialization
	void Start () {
		m_maxWidth = m_dashBar.rectTransform.rect.width;
		m_currentWidth = m_maxWidth;
	}
		
	public void Decrease(float rate) {
		if (m_currentWidth <= 0.0f) return;
		
		m_currentWidth = Mathf.Max(0.0f, m_currentWidth - (rate * Time.deltaTime));
		m_dashBar.rectTransform.sizeDelta = new Vector2 (m_currentWidth, m_dashBar.rectTransform.rect.height);

	}

	public void Increase(float rate) {
		if (m_currentWidth >= m_maxWidth) return;

		m_currentWidth = Mathf.Min(m_maxWidth, m_currentWidth + (rate * Time.deltaTime));
		m_dashBar.rectTransform.sizeDelta = new Vector2 (m_currentWidth, m_dashBar.rectTransform.rect.height);

	}

	public bool IsEmpty() {
		return (m_currentWidth <= 0.0f);
	}

	public bool IsFull() {
		return (m_currentWidth >= m_maxWidth);
	}

}
