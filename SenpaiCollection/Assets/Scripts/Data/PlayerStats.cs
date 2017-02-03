using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats
{
	public enum SenpaiType {
		SenpaiRedJacket,
		SenpaiBlackJacket,
		SenpaiBlackUniform,
		SnepaiBlueJumpsuit
	};

	private static int m_senpaisCollected;
	private static List<SenpaiType> m_senpaiTypeList = new List<SenpaiType>();

	public static void AddSenpaiType(SenpaiType senpai) {
		m_senpaiTypeList.Add (senpai);
		m_senpaisCollected = m_senpaiTypeList.Count;
	}

	public static void Clear() {
		m_senpaiTypeList.Clear ();
		m_senpaisCollected = 0;
	}

	public static List<SenpaiType> SenpaiTypeList {
		get {
			return m_senpaiTypeList;
		} 
		set {
			m_senpaiTypeList = value;
		}
	}

	public static int SenpaisCollected {
		get {
			return m_senpaisCollected;
		} 

		set {
			m_senpaisCollected = value;
		}
	}
		
}

