using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		private Vector3 targetPoint;
		Transform targetWaypoint;
		Vector3 targetDirection;
    	private bool moveToTarget = false;
    	private Transform playerTransform;
		public List<Transform> waypoints;
    	private int currentIndex = 0;
		public float speed;
		private float step;
		public GameObject Camera;
		private bool isRotating = true;
		public float rotationSpeed = 0.4f; // 回転速度
		public float arrivalThreshold = 3f; // ウェイポイントに到達したとみなす距離
		private float direction;

		private void Awake()
    	{
        	playerTransform = GetComponent<Transform>();
    	}

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
    	{
			MoveInput(value.Get<Vector2>());
    	}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			//Debug.Log(newMoveDirection);
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		// 以下追加
		void Update()
		{
			// 現在のウェイポイントを取得
            targetWaypoint = waypoints[currentIndex];
			targetDirection = (targetWaypoint.position - Camera.transform.position).normalized;

			// 地形の法線を取得
			Vector3 terrainNormal = GetTerrainNormal();
			// 地形の傾斜を考慮してtargetDirectionを調整
			Vector3 projectedDirection = Vector3.ProjectOnPlane(targetDirection, terrainNormal);
			targetDirection = projectedDirection.normalized;

			
			// ウェイポイントが設定されている場合のみ処理を行う
            if (waypoints.Count > 0)
            {
                // カメラの回転をウェイポイントの方向に徐々に合わせる
                if (isRotating)
                {
					// 地形の傾斜に合わせたカメラの回転を計算
					Quaternion terrainRotation = Quaternion.FromToRotation(Vector3.up, terrainNormal);
					Quaternion targetRotation = terrainRotation * Quaternion.LookRotation(targetDirection);
					float angleDiff = Quaternion.Angle(Camera.transform.rotation, targetRotation);

					//Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

					// 現在の回転とターゲットの回転の角度差を計算
                    // float angleDiff = Quaternion.Angle(Camera.transform.rotation, targetRotation);
                    
                    // 時計回りと反時計回りのどちらが近いかを判定
                    Vector3 cross = Vector3.Cross(Camera.transform.forward, targetDirection);
                    direction = (cross.y < 0) ? 1 : -1;

                    // デバッグ用のログ出力
                    // Debug.Log("Target Rotation: " + targetRotation.eulerAngles);
                    // Debug.Log("Camera Rotation: " + Camera.transform.rotation.eulerAngles);
                    // カメラの回転がウェイポイントの方向に一致したらisRotatingをfalseに
                    if (angleDiff < 1.4f)
                    {
                        isRotating = false;
                        look = Vector2.zero; // lookの値をリセット
                    }
                }

                // ウェイポイントに近づいたら次のウェイポイントに進む
                if (Vector3.Distance(transform.position, targetWaypoint.position) < 3f)
                {
                    // currentIndex = (currentIndex + 1) % waypoints.Count;
					currentIndex++;
                    isRotating = true; // 次のウェイポイントに向けて回転を再開
                }
            }
		}

		private Vector3 GetTerrainNormal()
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, LayerMask.GetMask("Terrain")))
			{
				return hit.normal;
			}
			return Vector3.up;
		}

		public void TriggerAutoMove(int shouldMove)
		{
			switch(shouldMove)
			{
				case -1: // 後方歩行
				move = new Vector2(0, -1); 
				break;
				
				case 1: // 前進歩行
				// move = new Vector2(direction.x, direction.z);
				move = new Vector2(0, 1);
				break;
				
				default: // 変更なし
				move = Vector2.zero;
				break;
			}
		}

		public void TriggerAutoRun(int shouldRun)
		{
			switch(shouldRun)
			{
				case -1: // 後方歩行
				move = new Vector2(0, -1);
				sprint = true; // 走る判定
				break;
				
				case 1: // 前進歩行
				move = new Vector2(0, 1);
				sprint = true;
				break;
				
				default: // 変更なし
				sprint = false;
				break;
			}
		}

		public void TriggerJump(bool Jump)
		{
			jump = Jump ? true : false; // 前方に移動するためのベクトルを設定
		}
		
		public void RotateLookDirection(int lookDirection)
		{
			switch(lookDirection)
			{
				case -1: // 左向き
					look = new Vector2(-0.4f, 0); 
				break;
				
				case 1: // 右向き
					look = new Vector2(0.4f, 0); 
					//look = Vector2.zero;
				break;
				
				default: // 変更なし
					if(isRotating)
					{
						look = new Vector2(-0.4f* direction, 0);
					}
					else
					{
						look = Vector2.zero;
					}
				break;
			}
		}
	}
}