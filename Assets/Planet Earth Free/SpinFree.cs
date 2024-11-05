using UnityEngine;
using System.Collections;

/// <summary>
/// Spin the object at a specified speed
/// </summary>
public class SpinFree : MonoBehaviour {
	[Tooltip("Spin: Yes or No")]
	public bool spin;
	[Tooltip("Spin the parent object instead of the object this script is attached to")]
	public bool spinParent;
	public float speed = 10f;

	[HideInInspector]
	public bool clockwise = true;
	[HideInInspector]
	public float direction = 1f;
	[HideInInspector]
	public float directionChangeSpeed = 2f;

    public float zoomSpeed = 20f;    // 控制缩放速度
    public float minZoomDistance = 200f; // 最小缩放距离
    public float maxZoomDistance = 800f; // 最大缩放距离
    private Transform mainCameraTransform;
    private float initialDistance;   // 摄像机和物体之间的初始距离

    private float lastTouchDistance;     // 上一次双指触摸的距离

    void Start() {
        // 获取主摄像机的 Transform 并计算初始距离
        mainCameraTransform = Camera.main.transform;
        initialDistance = Vector3.Distance(mainCameraTransform.position, transform.position);
        Debug.Log("初始距离：" + initialDistance);
    }

    // Update is called once per frame
    void Update() {
		if (direction < 1f) {
			direction += Time.deltaTime / (directionChangeSpeed / 2);
		}

		if (spin) {
			if (clockwise) {
				if (spinParent)
					transform.parent.transform.Rotate(Vector3.up, (speed * direction) * Time.deltaTime);
				else
					transform.Rotate(Vector3.up, (speed * direction) * Time.deltaTime);
			} else {
				if (spinParent)
					transform.parent.transform.Rotate(-Vector3.up, (speed * direction) * Time.deltaTime);
				else
					transform.Rotate(-Vector3.up, (speed * direction) * Time.deltaTime);
			}
		}


        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0.0f) {
            // 更新距离并控制范围
            initialDistance -= scrollInput * zoomSpeed;
            initialDistance = Mathf.Clamp(initialDistance, minZoomDistance, maxZoomDistance);

            // 根据新的距离调整摄像机的位置
            mainCameraTransform.position = transform.position - mainCameraTransform.forward * initialDistance;
        }


        // 如果检测到双指触摸，在Android上进行缩放
        if (Input.touchCount == 2) {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // 计算两次触摸的距离
            float currentTouchDistance = Vector2.Distance(touch0.position, touch1.position);

            // 如果是新的双指触摸，初始化lastTouchDistance
            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began) {
                lastTouchDistance = currentTouchDistance;
                return;
            }

            // 计算缩放变化
            float distanceDelta = currentTouchDistance - lastTouchDistance;
            initialDistance -= distanceDelta * zoomSpeed;
            initialDistance = Mathf.Clamp(initialDistance, minZoomDistance, maxZoomDistance);

            // 根据新的距离调整摄像机的位置
            mainCameraTransform.position = transform.position - mainCameraTransform.forward * initialDistance;

            // 更新上一次触摸距离
            lastTouchDistance = currentTouchDistance;
        }


    }
}