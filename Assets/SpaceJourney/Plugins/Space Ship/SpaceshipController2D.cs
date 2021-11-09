using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class SpaceshipController2D : MonoBehaviour
{
	private const float IdleCameraDistanceSmooth = 0.85f;
	private static readonly Vector3[] RotationDirections = { Vector3.right, Vector3.up, Vector3.forward };
	public Transform CachedTransform { get; private set; }

	public Vector3 CameraOffsetVector
	{
		get
		{
			return new Vector3(0.0f, Mathf.Sin(m_camera.Angle * Mathf.Deg2Rad) * m_camera.Offset, m_camera.DistanceOffset);
		}
	}

	public float CurrentSpeed
	{
		get
		{
			return Mathf.Lerp(m_spaceship.SpeedRange.x, m_spaceship.SpeedRange.y, SpeedFactor);
		}
	}

	public Vector4 RawInput { get; private set; }
	public Vector4 SmoothedInput { get; private set; }

	public float SpeedFactor
	{
		get
		{
			return m_spaceship.AccelerationCurve.Evaluate(SmoothedInput.w);
		}
	}

	private Transform m_cachedCameraTransform;

	[SerializeField, Tooltip("Camera options.")] private CameraSettings m_camera = new CameraSettings
	{
		Angle = 18.0f,
		Offset = 44.0f,
		DistanceOffset = -10f,
		PositionSmooth = 10.0f,
		RotationSmooth = 5.0f,
		OnRollCompensationFactor = 0.5f,
		LookAtPointOffset = new CameraLookAtPointOffsetSettings
		{
			OnIdle = new Vector2(0.0f, 10.0f),
			Smooth = new Vector2(30.0f, 30.0f),
			OnMaxSpeed = new Vector2(20.0f, -20.0f),
			OnTurn = new Vector2(30.0f, -30.0f)
		},
		normalCursor = null,
		aimingCursor = null,
		shakeAmount = 0.75f,
		shakeDuration = 0.02f,
		RotateCamera = false
	};

	private float m_idleCameraDistance;
	private Quaternion m_initialAvatarRotation;
	private float m_initialCameraFOV;

	[SerializeField, Tooltip("Input options.")] private InputSettings m_input = new InputSettings
	{
		Mode = InputMode.KeyboardAndMouse,
		Response = new Vector4(6.0f, 6.0f, 6.0f, 0.75f),
		Keyboard = new KeyboardSettings
		{
			Sensitivity = 1.5f,
			SensitivityOnMaxSpeed = 1.0f
		},
		Mouse = new MouseSettings
		{
			ActiveArea = new Vector2(450.0f, 300.0f),
			MovementThreshold = 75.0f,
			Sensitivity = 1.0f,
			SensitivityOnMaxSpeed = 0.85f
		}
	};

	private Vector2 m_lookAtPointOffset;

	[SerializeField, Tooltip("Spaceship options.")] public SpaceshipSettings m_spaceship = new SpaceshipSettings
	{
		AccelerationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f),
		BankAngleSmooth = 2.5f,
		Maneuverability = new Vector3(75.0f, 75.0f, -50.0f),
		MaxBankAngleOnTurnAxis = 45.0f,
		MaxBankAngleOnTurnLeftRight = 60f,
		MaxBankAngleOnTurnUpDown = 60f,
		MaxBankAngleSideways = 30f,
		SidewaysSpeed = 25f,
		SpeedRange = new Vector2(30.0f, 600.0f),
		HP = 150,
		HP_text = null,
		enemies = new List<GameObject>(),
		enemies_text = null
	};

	[SerializeField, Tooltip("Shooting options.")] public ShootingSettings m_shooting = new ShootingSettings
	{

		bulletSettings = new BulletSettings{
			BulletBarrels = new List<GameObject>(),
			Bullet = null,
			BulletSpeed = 300f,
			BulletFireDelay = 0.15f,
			BulletDamage = 15f,
			BulletLifetime = 7f,
			TargetDistance = 300f
		},
		rocketSettings = new RocketSettings{
			RocketBarrels = new List<GameObject>(),
			Rocket = null,
			RocketSpeed = 3.5f,
            RocketInitialSpeed = 150f,
            RocketFireDelay = 1f,
			RocketDamage = 200f,
			RocketLifetime = 7f,
			RocketTurningSpeed = 40f,
			LockOnDistance = 700f,
			RocketOffsetLimit = new Vector3(250f,250f,250f)
		}
	};

	//singleton
	public static SpaceshipController2D instance;

	void OnValidate()
	{
		prevOff = m_camera.Offset;
	}
	private void Awake()
	{
		
		if(instance==null){
			instance = this;
		}else{
			Debug.LogError("Singleton pattern violated! Two player controled spaceships present in the scene (2D)");
		}
		if(m_camera.normalCursor!=null){
			Cursor.SetCursor(m_camera.normalCursor, new Vector2(m_camera.normalCursor.width * 0.5f, m_camera.normalCursor.height * 0.5f), CursorMode.Auto);
		}
		RawInput = Vector4.zero;
		SmoothedInput = Vector4.zero;
		CachedTransform = transform;
		m_cachedCameraTransform = m_camera.TargetCamera.transform;
		m_idleCameraDistance = CameraOffsetVector.magnitude;
		m_initialAvatarRotation = m_spaceship.Avatar.localRotation;
		m_initialCameraFOV = m_camera.TargetCamera.fieldOfView;
		m_lookAtPointOffset = m_camera.LookAtPointOffset.OnIdle;

		m_cachedCameraTransform.position = CachedTransform.position + CameraOffsetVector;

		prevOff = m_camera.Offset;

	}

	Coroutine shooting =  null;
	private bool isShooting = false;

	Coroutine firing = null;
	private bool isFiring = false;

	Transform rocket_target;

	//global bullet barrel variable
	int b = 0;

	private void LateUpdate(){

		m_spaceship.HP_text.text = m_spaceship.HP.ToString();
		m_spaceship.enemies_text.text = m_spaceship.enemies.Count.ToString();

		if(m_spaceship.enemies.Count==0){
			UIcoroutines.instance.GameOver(false);
		}

		if(m_spaceship.HP<=0){
			UIcoroutines.instance.GameOver(true);
			gameObject.SetActive(false);
		}

			//Bullets on LMB
		if (Input.GetMouseButtonDown (0) && !isShooting) {
			shooting =  StartCoroutine (BulletShooting(m_shooting));
			isShooting = true;
		}
		if (Input.GetMouseButtonUp (0) && isShooting) {
			StopCoroutine (shooting);
			isShooting = false;
		}

		//Rockets on RMB
		
		Ray ray;
		if(m_camera.TargetCamera.targetTexture == null){
			ray = m_camera.TargetCamera.ScreenPointToRay( Input.mousePosition );
		}else{
			//EVERYTHING MUST BE A FLOAT
			ray = m_camera.TargetCamera.ScreenPointToRay(Input.mousePosition/(float)((float)Screen.height/(float)m_camera.TargetCamera.pixelHeight) );
		}
		
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, m_shooting.rocketSettings.LockOnDistance)) {
			if (hit.transform != transform && Vector3.Distance(transform.position, hit.point) > 10f) {
				rocket_target = hit.transform;
				if(m_camera.aimingCursor!=null){
					Cursor.SetCursor(m_camera.aimingCursor, new Vector2(m_camera.aimingCursor.width * 0.5f, m_camera.aimingCursor.height * 0.5f), CursorMode.Auto);
				}
			} else {
				rocket_target = null;
				if(m_camera.normalCursor!=null){
					Cursor.SetCursor(m_camera.normalCursor, new Vector2(m_camera.normalCursor.width * 0.5f, m_camera.normalCursor.height * 0.5f), CursorMode.Auto);
				}
			}
		} else {
			if(m_camera.normalCursor!=null){
				Cursor.SetCursor(m_camera.normalCursor, new Vector2(m_camera.normalCursor.width * 0.5f, m_camera.normalCursor.height * 0.5f), CursorMode.Auto);
			}
			rocket_target = null;
		}

		if (Input.GetMouseButtonDown (1) && !isFiring) {

			firing = StartCoroutine (RocketFiring(m_shooting,rocket_target));
			isFiring = true;

		}
		/*if (Input.GetMouseButtonUp (1) && isFiring) {
			StopCoroutine (firing);
			isFiring = false;
		}*/
	}
	private void FixedUpdate()
	{
		UpdateCamera();
		UpdateInput();
		UpdateOrientationAndPosition();

	

	}

	private void Update()
	{
		
	}
	float prevOff = 0f;

	private void UpdateCamera()
	{
		Vector2 focalPointOnMoveOffset = Vector2.Lerp(m_camera.LookAtPointOffset.OnTurn,
			m_camera.LookAtPointOffset.OnMaxSpeed, SpeedFactor);

		m_lookAtPointOffset.x = Mathf.Lerp(
			m_lookAtPointOffset.x,
			Mathf.Lerp(
				m_camera.LookAtPointOffset.OnIdle.x,
				focalPointOnMoveOffset.x * Mathf.Sign(SmoothedInput.y),
				Mathf.Abs(SmoothedInput.y)),
			m_camera.LookAtPointOffset.Smooth.x * Time.deltaTime);

		m_lookAtPointOffset.y = Mathf.Lerp(
			m_lookAtPointOffset.y,
			Mathf.Lerp(
				m_camera.LookAtPointOffset.OnIdle.y,
				focalPointOnMoveOffset.y * Mathf.Sign(SmoothedInput.x),
				Mathf.Abs(SmoothedInput.x)),
			m_camera.LookAtPointOffset.Smooth.y * Time.deltaTime);

		Vector3 lookTargetPosition = CachedTransform.position + CachedTransform.right * m_lookAtPointOffset.x +
			CachedTransform.up * m_lookAtPointOffset.y;

		Vector3 lookTargetUpVector = (CachedTransform.up + CachedTransform.right *
			SmoothedInput.z * m_camera.OnRollCompensationFactor).normalized;

		Quaternion targetCameraRotation;

		if(m_camera.RotateCamera){
			targetCameraRotation = Quaternion.Euler(90f, CachedTransform.localEulerAngles.y,0f);
		}else{
			targetCameraRotation = Quaternion.Euler(90f, 0f,0f);
		}

		m_cachedCameraTransform.rotation = Quaternion.Slerp(m_cachedCameraTransform.rotation,
			targetCameraRotation, m_camera.RotationSmooth * Time.deltaTime);

		Vector3 cameraOffset = CachedTransform.TransformDirection(CameraOffsetVector);

		m_cachedCameraTransform.position = Vector3.Lerp(m_cachedCameraTransform.position,
			CachedTransform.position + cameraOffset, m_camera.PositionSmooth * Time.deltaTime);

		float idleCameraDistance = cameraOffset.magnitude + (cameraOffset.normalized * m_spaceship.SpeedRange.x *
			Time.deltaTime / m_camera.PositionSmooth).magnitude;

		m_idleCameraDistance = Mathf.Lerp(m_idleCameraDistance, idleCameraDistance, IdleCameraDistanceSmooth * Time.deltaTime);
		float baseFrustumHeight = 2.0f * m_idleCameraDistance * Mathf.Tan(m_initialCameraFOV * 0.5f * Mathf.Deg2Rad);
		m_camera.TargetCamera.fieldOfView = 2.0f * Mathf.Atan(baseFrustumHeight * 0.5f / Vector3.Distance(
			CachedTransform.position, m_cachedCameraTransform.position)) * Mathf.Rad2Deg;
		
		//Camera push-back on throttle
		if(Input.GetButton(m_input.Keyboard.InputNames.Throttle)){
			if(m_camera.Offset==prevOff){
				m_camera.Offset+=500f;
			}
		}else if(m_camera.Offset!=prevOff){
			m_camera.Offset=prevOff;
		}
		
	}

	private void UpdateInput()
	{
		float currentKeyboardSensitivity = Mathf.Lerp(m_input.Keyboard.Sensitivity,
			m_input.Keyboard.SensitivityOnMaxSpeed, SpeedFactor);

		//Calc raw input.
		Vector4 currentRawInput = Vector4.zero;
		switch (m_input.Mode)
		{
			case InputMode.Keyboard:
				currentRawInput.x = Input.GetAxis(m_input.Keyboard.InputNames.AxisX) * currentKeyboardSensitivity;
				currentRawInput.y = Input.GetAxis(m_input.Keyboard.InputNames.AxisY) * currentKeyboardSensitivity;
				break;

			case InputMode.KeyboardAndMouse:
				float currentMouseSensitivity = Mathf.Lerp(m_input.Mouse.Sensitivity,
					m_input.Mouse.SensitivityOnMaxSpeed, SpeedFactor);

				Vector2 mouseOffsetFromScreenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f) -
					(Vector2)Input.mousePosition;

				if (Mathf.Abs(mouseOffsetFromScreenCenter.y) > m_input.Mouse.MovementThreshold)
				{
					float verticalOffsetFromCenter = mouseOffsetFromScreenCenter.y - Mathf.Sign(mouseOffsetFromScreenCenter.y) *
						m_input.Mouse.MovementThreshold;

					currentRawInput.x = Mathf.Clamp(verticalOffsetFromCenter / (m_input.Mouse.ActiveArea.y -
						m_input.Mouse.MovementThreshold), -1.0f, 1.0f) * currentMouseSensitivity;
				}

				if (Mathf.Abs(mouseOffsetFromScreenCenter.x) > m_input.Mouse.MovementThreshold)
				{
					float horizontalOffsetFromCenter = mouseOffsetFromScreenCenter.x - Mathf.Sign(mouseOffsetFromScreenCenter.x) *
						m_input.Mouse.MovementThreshold;

					currentRawInput.y = -Mathf.Clamp(horizontalOffsetFromCenter / (m_input.Mouse.ActiveArea.x -
						m_input.Mouse.MovementThreshold), -1.0f, 1.0f) * currentMouseSensitivity;
				}

				break;
		}

		currentRawInput.w = Input.GetButton(m_input.Keyboard.InputNames.Throttle) ? 1.0f : 0.0f;

		//Calc smoothed input.
		Vector4 currentSmoothedInput = Vector4.zero;
		for (int i = 0; i < 4; ++i)
		{
			currentSmoothedInput[i] = Mathf.Lerp(SmoothedInput[i], currentRawInput[i], m_input.Response[i] * Time.deltaTime);
		}

		RawInput = currentRawInput;
		SmoothedInput = currentSmoothedInput;
	}

	private void UpdateOrientationAndPosition()
	{
		/*for (int i = 0; i < 3; ++i)
		{
			CachedTransform.localRotation *= Quaternion.AngleAxis(SmoothedInput[i] *
				m_spaceship.Maneuverability[i] * Time.deltaTime, RotationDirections[i]);
		}*/

		if(Input.GetAxis("Stop")==0f){
			CachedTransform.localPosition += CachedTransform.forward * CurrentSpeed * Time.deltaTime;
		}

		Vector3 p = m_camera.TargetCamera.ScreenToWorldPoint(new Vector3((float)Input.mousePosition.x/(float)((float)Screen.height/(float)m_camera.TargetCamera.pixelHeight), (float)Input.mousePosition.y/(float)((float)Screen.height/(float)m_camera.TargetCamera.pixelHeight), m_camera.Offset));
		p.y = 0f;
		//follow
		CachedTransform.localRotation = Quaternion.Slerp(
			CachedTransform.localRotation,
			Quaternion.LookRotation(p - m_spaceship.Avatar.transform.position,Vector3.up),
			m_spaceship.BankAngleSmooth * Time.deltaTime);

		//left right
		/*m_spaceship.Avatar.localRotation = Quaternion.Slerp(
			m_spaceship.Avatar.localRotation,
			m_initialAvatarRotation * Quaternion.AngleAxis(SmoothedInput.y * m_spaceship.MaxBankAngleOnTurnLeftRight, Vector3.up),
			m_spaceship.BankAngleSmooth * Time.deltaTime);*/

		//around axis
		/*m_spaceship.Avatar.localRotation = Quaternion.Slerp(
			m_spaceship.Avatar.localRotation,
			m_initialAvatarRotation * Quaternion.AngleAxis(-SmoothedInput.y * m_spaceship.MaxBankAngleOnTurnAxis, Vector3.forward),
			m_spaceship.BankAngleSmooth * Time.deltaTime);*/
			
		//up and down
		/*m_spaceship.Avatar.localRotation = Quaternion.Slerp(
			m_spaceship.Avatar.localRotation,
			m_initialAvatarRotation * Quaternion.AngleAxis(SmoothedInput.x * m_spaceship.MaxBankAngleOnTurnUpDown, Vector3.right),
			m_spaceship.BankAngleSmooth * Time.deltaTime);*/

		if(Input.GetAxis("Sideways")!=0f){
			//CachedTransform.Translate(CachedTransform.right*Input.GetAxis("Sideways")/2f,Space.World);
			CachedTransform.localPosition += CachedTransform.right * Input.GetAxis("Sideways") * Time.deltaTime * m_spaceship.SidewaysSpeed;
		}
	}

	public void Shake(){

		if(!isRunning){
		
			StartCoroutine(shaking());
		}

	}
	float shakePercentage;
	float startAmount;
	float startDuration;

	bool isRunning = false;	
	IEnumerator shaking(){
	
		isRunning = true;
		startAmount = m_camera.shakeAmount;
		startDuration = m_camera.shakeDuration;

		while (m_camera.shakeDuration > 0.01f) {
			Vector3 rotationAmount = UnityEngine.Random.insideUnitSphere * m_camera.shakeAmount;
			rotationAmount+=m_camera.TargetCamera.transform.localEulerAngles;
 
			shakePercentage = m_camera.shakeDuration / startDuration;
 
			m_camera.shakeAmount = startAmount * shakePercentage;
			m_camera.shakeDuration = Mathf.Lerp(m_camera.shakeDuration, 0, Time.deltaTime);
 
			m_camera.TargetCamera.transform.localRotation =  Quaternion.Euler (rotationAmount);
 
			yield return null;
		}
		m_camera.shakeAmount = startAmount;
		m_camera.shakeDuration = startDuration;
		isRunning = false;
	}

	private IEnumerator BulletShooting(ShootingSettings settings){
		GameObject bullet;
		Vector3 p; 
		while(true){
			for (int i = b; i < settings.bulletSettings.BulletBarrels.Count; i++) {
				bullet = (GameObject)Instantiate (settings.bulletSettings.Bullet,settings.bulletSettings.BulletBarrels[i].transform.position, Quaternion.LookRotation(transform.forward,transform.up));
				if( settings.bulletSettings.BulletBarrels.Count > 1){
					if(b == 0) {
						b = 1;
					} else {
						b = 0;
					}
				}
				if(settings.bulletSettings.BulletBarrels[i].GetComponent<ParticleSystem>()!=null){

					settings.bulletSettings.BulletBarrels[i].GetComponent<ParticleSystem>().Play();

				}
				if(m_camera.TargetCamera.targetTexture == null){
					p = m_camera.TargetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, settings.bulletSettings.TargetDistance));
				}else{
					//EVERYTHING MUST BE A FLOAT
					p = m_camera.TargetCamera.ScreenToWorldPoint(new Vector3((float)Input.mousePosition.x/(float)((float)Screen.height/(float)m_camera.TargetCamera.pixelHeight), (float)Input.mousePosition.y/(float)((float)Screen.height/(float)m_camera.TargetCamera.pixelHeight), settings.bulletSettings.TargetDistance));
				}
			
				p.y = 0f;
				
				bullet.transform.LookAt (p);
				bullet.GetComponent<Rigidbody> ().AddForce (bullet.transform.forward*settings.bulletSettings.BulletSpeed,ForceMode.Impulse);
				yield return new WaitForSeconds(settings.bulletSettings.BulletFireDelay);
			}
			yield return null;
		}
	}

	private IEnumerator RocketFiring(ShootingSettings settings, Transform target){
		int barrel;
		GameObject rocket;
		Vector3 p;
		barrel = UnityEngine.Random.Range (0, settings.rocketSettings.RocketBarrels.Count - 1);
		rocket = (GameObject)Instantiate (settings.rocketSettings.Rocket, settings.rocketSettings.RocketBarrels [barrel].transform.position, Quaternion.LookRotation (transform.forward, transform.up));
		if(m_camera.TargetCamera.targetTexture == null){
					p = m_camera.TargetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, settings.bulletSettings.TargetDistance));
				}else{
					//EVERYTHING MUST BE A FLOAT
					p = m_camera.TargetCamera.ScreenToWorldPoint(new Vector3((float)Input.mousePosition.x/(float)((float)Screen.height/(float)m_camera.TargetCamera.pixelHeight), (float)Input.mousePosition.y/(float)((float)Screen.height/(float)m_camera.TargetCamera.pixelHeight), settings.bulletSettings.TargetDistance));
				}
		if (rocket.GetComponent<RocketScript> () != null && target != null) {
			rocket.transform.LookAt(target.position +  new Vector3(UnityEngine.Random.Range(-m_shooting.rocketSettings.RocketOffsetLimit.x,m_shooting.rocketSettings.RocketOffsetLimit.x),
			UnityEngine.Random.Range(-m_shooting.rocketSettings.RocketOffsetLimit.y,m_shooting.rocketSettings.RocketOffsetLimit.y),
			UnityEngine.Random.Range(-m_shooting.rocketSettings.RocketOffsetLimit.z,m_shooting.rocketSettings.RocketOffsetLimit.z)));
			rocket.GetComponent<RocketScript> ().StartChase (target, settings.rocketSettings.RocketSpeed, settings.rocketSettings.RocketInitialSpeed);
		}else{
		
			p.y = 0f;
			
			rocket.transform.LookAt (p);
            rocket.GetComponent<Rigidbody>().AddForce(rocket.transform.forward * settings.rocketSettings.RocketInitialSpeed, ForceMode.Impulse);
        }
		yield return new WaitForSeconds(settings.rocketSettings.RocketFireDelay);
		isFiring = false;
	}

	[Serializable]
	private struct CameraLookAtPointOffsetSettings
	{
		[Tooltip("Offset of the look-at point (relative to the spaceship) when flying straight with a minimum speed.")] public Vector2 OnIdle;
		[Tooltip("Offset of the look-at point (relative to the spaceship) when flying or turning with a maximum speed.")] public Vector2 OnMaxSpeed;
		[Tooltip("Offset of the look-at point (relative to the spaceship) when turning with a minimum speed.")] public Vector2 OnTurn;
		[Tooltip("How fast the look-at point interpolates to the desired value. Higher = faster.")] public Vector2 Smooth;
	}

	[Serializable]
	private struct CameraSettings
	{
		[Tooltip("Angle of the camera. 0 = behind, 90 = top-down.")][HideInInspector] public float Angle;
		[Tooltip("Look-at point options.")] public CameraLookAtPointOffsetSettings LookAtPointOffset;
		[Tooltip("Distance between the camera and the spaceship verically.")] public float Offset;
		[Tooltip("Distance between the camera and the spaceship horizontally.")] public float DistanceOffset;
		[Tooltip("Tilt of the camera when the spaceship is doing a roll. 0 = no tilt.")][HideInInspector] public float OnRollCompensationFactor;
		[Tooltip("How fast the camera follows the spaceship's position. Higer = faster.")] public float PositionSmooth;
		[Tooltip("How fast the camera follows the spaceship's rotation. Higer = faster.")] public float RotationSmooth;
		[Tooltip("Camera object.")] public Camera TargetCamera;
		[Tooltip("Standard cursor")] public Texture2D normalCursor;
		[Tooltip("Cursor when aiming at target")] public Texture2D aimingCursor;
		[Tooltip("Shake amount when hit")] public float shakeAmount;
		[Tooltip("Shake duration when hit")] public float shakeDuration;
		[Tooltip("Rotate camera")] public bool RotateCamera;
	}

	private enum InputMode
	{
		Keyboard,
		KeyboardAndMouse
	}

	[Serializable]
	private struct InputSettings
	{
		[Tooltip("Keyboard options.")] public KeyboardSettings Keyboard;
		[Tooltip("Input mode.")] public InputMode Mode;
		[Tooltip("Mouse options.")] public MouseSettings Mouse;
		[Tooltip("How fast the input interpolates to the desired value. Higher = faster.")] public Vector4 Response;
	}

	[Serializable]
	private struct KeyboardInputNames
	{
		[Tooltip("Rotation around x-axis (vertical movement).")] public string AxisX;
		[Tooltip("Rotation around y-axis (horizontal movement).")] public string AxisY;
		[Tooltip("Rotation around z-axis (roll).")] public string AxisZ;
		[Tooltip("Speed control.")] public string Throttle;
	}

	[Serializable]
	private struct KeyboardSettings
	{
		[Tooltip("Names of input axes (from InputManager).")] public KeyboardInputNames InputNames;
		[Tooltip("Keyboard sensitivity when flying with a minimum speed.")] public float Sensitivity;
		[Tooltip("Keyboard sensitivity when flying with a maximum speed.")] public float SensitivityOnMaxSpeed;
	}

	[Serializable]
	private struct MouseSettings
	{
		[Tooltip("Mouse input is set to a maximum when the cursor is out of bounds of that area.")] public Vector2 ActiveArea;
		[Tooltip("How far the cursor should be moved from the center of the screen to make the spaceship turn.")] public float MovementThreshold;
		[Tooltip("Mouse sensitivity when flying with a minimum speed.")] public float Sensitivity;
		[Tooltip("Mouse sensitivity when flying with a maximum speed.")] public float SensitivityOnMaxSpeed;
	}

	[Serializable]
	public struct SpaceshipSettings
	{
		[Tooltip("Defines how speed changes over time.")] public AnimationCurve AccelerationCurve;
		[Tooltip("The spaceship's model.")] public Transform Avatar;
		[Tooltip("How fast the spaceship tilts when doing a sideways turns. Higher = faster.")] public float BankAngleSmooth;
		[Tooltip("How fast the spaceship turns. Higher = faster.")][HideInInspector] public Vector3 Maneuverability;
		[Tooltip("Maximum tilt of the spaceship when doing a sideways turns.")][HideInInspector] public float MaxBankAngleOnTurnAxis;
		[Tooltip("Maximum turn to left/right")][HideInInspector] public float MaxBankAngleOnTurnLeftRight;
		[Tooltip("Maximum turn up/down")][HideInInspector] public float MaxBankAngleOnTurnUpDown;
		[Tooltip("Maximum tilt when going sideways")][HideInInspector] public float MaxBankAngleSideways;
		[Tooltip("Speed when going sideways")] public float SidewaysSpeed;
		[Tooltip("Minimum and maximum speed of the spaceship.")] public Vector2 SpeedRange;
		[Tooltip("Ship HP")] public int HP;
		[Tooltip("Text displaying ship HP")] public Text HP_text;
		[Tooltip("Text displaying ship HP")] public List<GameObject> enemies;
		[Tooltip("Text displaying ship HP")] public Text enemies_text;
		
	}

	[Serializable]
	public struct ShootingSettings
	{
		[Tooltip("The bullet settings.")] public BulletSettings bulletSettings;
		[Tooltip("The rocket settings.")] public RocketSettings rocketSettings;
	
	}

	[Serializable]
	public struct BulletSettings
	{
		[Tooltip("The origin point(s) of bullets.")] public List<GameObject> BulletBarrels;
		[Tooltip("The bullet prefab.")] public GameObject Bullet;
		[Tooltip("The bullet speed.")] public float BulletSpeed;
		[Tooltip("The bullet firing delay.")] public float BulletFireDelay;
		[Tooltip("The bullet damage.")] public float BulletDamage;
		[Tooltip("How long before the bullet disappears.")] public float BulletLifetime;
		[Tooltip("The dostance from the cursors position on the screen in 3D space.")] public float TargetDistance;
	
	}

	[Serializable]
	public struct RocketSettings
	{
		[Tooltip("The origin point(s) of rockets.")] public List<GameObject> RocketBarrels;
		[Tooltip("The rocket prefab.")] public GameObject Rocket;
        [Tooltip("The initial rocket speed.")] public float RocketInitialSpeed;
        [Tooltip("The rocket chase speed.")] public float RocketSpeed;
		[Tooltip("The rocket firing delay.")] public float RocketFireDelay;
		[Tooltip("The rocket damage.")] public float RocketDamage;
		[Tooltip("How long before the rocket disappears.")] public float RocketLifetime;
		[Tooltip("How fast does the rocket turn.")] public float RocketTurningSpeed;
		[Tooltip("Max distance of rocket lock-on")] public float LockOnDistance;
		[Tooltip("Rocket start offset, random range")] public Vector3 RocketOffsetLimit;
	
	}

}
