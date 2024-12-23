using UnityEngine;

namespace Assets.Scripts.UI.Utils
{
	public class UIElementRotationBehaviour : MonoBehaviour
	{
		[SerializeField] private float RotationSpeed;

		private void Update()
		{
			transform.Rotate(Vector3.back, RotationSpeed * Time.deltaTime);
		}
	}
}
