//Original asset by baba-s
//Modified under free license by LISA: The Fool Dev Team

using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TUFF
{
	/// <summary>
	/// TextMesh Pro で 1 文字ずつ表示する演出を再生するコンポーネント
	/// </summary>
	[RequireComponent(typeof(TMP_Text))]
	public partial class TMP_Typewriter : MonoBehaviour
	{
		//==============================================================================
		// 変数(SerializeField)
		//==============================================================================
		[SerializeField] private TMP_Text m_textUI = null;

		//==============================================================================
		// 変数
		//==============================================================================
		private string m_parsedText;
		private Action m_onComplete;
		private Tween m_tween;
		private int m_lastMaxVisibleCharacters = -1;
		[Header("Voicebank Data")]
		public AudioClip clip;
		public float pitch = 1;
		public float pitchVariation = 0.1f;

		//==============================================================================
		// 関数
		//==============================================================================
		/// <summary>
		/// アタッチされた時や Reset された時に呼び出されます
		/// </summary>
		private void Reset()
		{
			m_textUI = GetComponent<TMP_Text>();
			m_lastMaxVisibleCharacters = -1;
		}

		/// <summary>
		/// 破棄される時に呼び出されます
		/// </summary>
		private void OnDestroy()
		{
			m_tween?.Kill();
			m_tween = null;
			m_onComplete = null;
		}

		/// <summary>
		/// 演出を再生します
		/// </summary>
		/// <param name="text">表示するテキスト ( リッチテキスト対応 )</param>
		/// <param name="speed">表示する速さ ( speed == 1 の場合 1 文字の表示に 1 秒、speed == 2 の場合 0.5 秒かかる )</param>
		/// <param name="onComplete">演出完了時に呼び出されるコールバック</param>
		public void Play(string text, float speed, Action onComplete)
		{
			m_textUI.text = text;
			m_onComplete = onComplete;
			StartCoroutine(PlayCoroutine(text, speed, onComplete));
			//m_textUI.ForceMeshUpdate();
		}

		private IEnumerator PlayCoroutine(string text, float speed, Action onComplete)
        {
			m_textUI.maxVisibleCharacters = 0;
			yield return new WaitForEndOfFrame();

			m_parsedText = m_textUI.GetParsedText();

			var length = m_parsedText.Length;

			var duration = 1 / speed * length;

			OnUpdate(0);

			m_tween?.Kill();
			m_tween = DOTween
				.To(value => OnUpdate(value), 0, 1, duration)
				.SetEase(Ease.Linear)
				.OnComplete(() => OnComplete())
			;
			if (GameManager.instance.configData.textSpeed >= 1) Skip();
		}

		/// <summary>
		/// 演出をスキップします
		/// </summary>
		/// <param name="withCallbacks">演出完了時に呼び出されるコールバックを実行する場合 true</param>
		public void Skip(bool withCallbacks = true)
		{
			m_tween?.Kill();
			m_tween = null;

			OnUpdate(1);
			m_textUI.maxVisibleCharacters = 99999;
			if (!withCallbacks) return;

			m_onComplete?.Invoke();
			m_onComplete = null;
		}

		/// <summary>
		/// 演出を一時停止します
		/// </summary>
		public void Pause()
		{
			m_tween?.Pause();
		}

		/// <summary>
		/// 演出を再開します
		/// </summary>
		public void Resume()
		{
			m_tween?.Play();
		}

		/// <summary>
		/// 演出を更新する時に呼び出されます
		/// </summary>
		private void OnUpdate(float value)
		{
			var current = Mathf.Lerp(0, m_parsedText.Length, value);
			var count = Mathf.FloorToInt(current);

			m_textUI.maxVisibleCharacters = count;
			if (m_lastMaxVisibleCharacters != m_textUI.maxVisibleCharacters)
			{
				if(clip != null) AudioManager.instance.PlaySFX(clip, 1f, UnityEngine.Random.Range(pitch - pitchVariation, pitch + pitchVariation));
				m_lastMaxVisibleCharacters = m_textUI.maxVisibleCharacters;
			}
		}


		/// <summary>
		/// 演出が更新した時に呼び出されます
		/// </summary>
		private void OnComplete()
		{
			m_textUI.maxVisibleCharacters = 99999;
			m_tween = null;
			m_onComplete?.Invoke();
			m_onComplete = null;
			m_lastMaxVisibleCharacters = -1;
		}
	}
}