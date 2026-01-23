using UnityEngine;
using UnityEngine.UI;

public class AspectRatioManager : MonoBehaviour 
{
	[SerializeField] private float _xAspect;
	[SerializeField] private float _yAspect;
	private CanvasScaler[] _canvasScaler;

	private void Awake()
	{
		var camera = gameObject.GetComponent<Camera>();
		Rect rect = calcAspect(_xAspect, _yAspect);
		camera.rect = rect;
	}
	
	public void SetCanvas(Canvas canvas)
	{
		_canvasScaler = canvas.GetComponents<CanvasScaler>();

		for (int i = 0; i < _canvasScaler.Length; i++)
		{
			_canvasScaler[i].matchWidthOrHeight = CheckScreenRatio(i);
		}
	}

	private Rect calcAspect(float width, float height)
	{
		float targetAspect = width / height;
		float windowAspect = (float)Screen.width / (float)Screen.height;
		float scaleHeight = windowAspect / targetAspect;
		Rect rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

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