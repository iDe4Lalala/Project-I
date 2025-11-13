using UnityEngine;
using UnityEngine.UI;

public class AspectRatioManager : MonoBehaviour 
{
	/// <summary>
	/// アスペクト比を管理する
	/// </summary>

	[SerializeField] private float _xAspect;	  // アスペクト比のX値
	[SerializeField] private float _yAspect;	  // アスペクト比のY値

	private CanvasScaler[] _canvasScaler;

	private void Awake()
	{
		//Cameraのアスペクト比を設定する
		var camera = gameObject.GetComponent<Camera>();
		Rect rect = calcAspect(_xAspect, _yAspect);
		camera.rect = rect;
	}
	
	public void SetCanvas(Canvas canvas)
	{
		/// <summary>
		/// Canvasを設定する
		/// </summary>

		_canvasScaler = canvas.GetComponents<CanvasScaler>();
		
		//Canvasのアスペクト比を設定する
		for (int i = 0; i < _canvasScaler.Length; i++)
		{
			_canvasScaler[i].matchWidthOrHeight = CheckScreenRatio(i);
		}
	}

	private Rect calcAspect(float width, float height)
	{
		/// <summary>
		/// アスペクト比を計算してRectを返す
		/// </summary>
		
		// アスペクト比を計算
		float targetAspect = width / height;
		float windowAspect = (float)Screen.width / (float)Screen.height;
		float scaleHeight = windowAspect / targetAspect;
		Rect rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

		// スクリーンのアスペクト比に応じてRectを調整
		if(1.0f > scaleHeight)
		{
			rect.x = 0;
			rect.y = (1.0f - scaleHeight) / 2.0f;
			rect.width = 1.0f;
			rect.height = scaleHeight;
		}
		else
		{
			float scaleWidth = 1.0f / scaleHeight;
			rect.x = (1.0f - scaleWidth) / 2.0f;
			rect.y = 0.0f;
			rect.width = scaleWidth;
			rect.height = 1.0f;
		}

		return rect;
	}

	private int CheckScreenRatio(int i)
	{
		/// <summary>
		/// 画面比率をチェックする
		/// </summary>
		
		if (Screen.width * _canvasScaler[i].referenceResolution.y / _canvasScaler[i].referenceResolution.x 
			< Screen.height)
		{
			return 0;
		} 
		else 
		{
			return 1;
		}
	}
}