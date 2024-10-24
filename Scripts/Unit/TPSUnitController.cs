using develop_common;
using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

// https://github.com/msasaki-jyobi/3DGameKitS/blob/feature-student/Assets/_MyProject/Scripts/Input/UnitController.cs#L43

namespace develop_tps
{
    public class TPSUnitController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Camera _camera;
        [SerializeField] private Rigidbody _rigidBody;
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimatorStateController _animatorStateController;
        [SerializeField] private UnitActionLoader _unitActionLoader;
        [SerializeField] private TPSCliffUp _tpsCliffUp;

        [Header("LineDatas")]
        [SerializeField] private LineData _groundLineData;

        [Header("Input Parameter")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _dashRange = 1.5f;
        [SerializeField] private float _stamina = 10f;
        [SerializeField] private float _staminaHealSpeed = 1f;
        [SerializeField] private int _jumpLimit = 1;
        [SerializeField] private float _jumpPower = 10f;
        [SerializeField] private float _crouchSpeed = 2f;
        [SerializeField] private float _crouchCrossTime = 0.2f;


        [Header("Gravity")]
        [SerializeField] private float _addGravity = 600f;

        [Header("Slope Parameter")]
        [SerializeField] private float _maxSlopeAngle = 45f;
        [SerializeField] private float _slopeDistance = 0.2f;

        [Header("Ignore Input")]
        [SerializeField] private bool _isNotVertical;
        [SerializeField] private bool _isNotHorizontal;
        [SerializeField] private bool _isNotJump;
        [SerializeField] private bool _isNotCrouth;

        [Header("Capsule Size")]
        [SerializeField] private CapsuleCollider _capsuleCollider;
        [SerializeField] private float _standCenterY = 0.92f;
        [SerializeField] private float _standHeight = 1.8f;
        [SerializeField] private float _crouthCenterY = 0.68f;
        [SerializeField] private float _crouthHeight = 1.33f;

        [Header("Motion")]
        public string LocomotionStateName = "Locomotion";
        public string JumpStateName = "Jump";

        // Ground Check
        public bool CanJump { private set; get; }

        // private Parameter
        private float _defaultSpeed;
        private float _defaultCrouchSpeed;
        private int _jumpCount;

        // private Input Parameter
        private float _InputX;
        private float _InputY;
        private Vector3 _tpsVelocity;
        private Quaternion _targetRotation;
        private float _rotateSpeed = 600f;

        // private Slope Parameter
        private float _slopeAngle;
        private float _staminaTimer;

        // private KnockedBack
        private bool isKnockedBack;
        private Vector3 knockbackDirection;
        private float knockbackTime = 0.5f; // 吹き飛ばしの時間
        private float knockbackCounter;

        // Key Parameter
        public bool IsJump { private set; get; }
        private ReactiveProperty<bool> _isCrouth = new ReactiveProperty<bool>();
        private bool _isClimb;
        private bool _isDash;

        public event Action<float, float> StaminaUpdateEvent;

        private void Start()
        {
            // Handle InputReader
            _inputReader.MoveEvent += OnMoveHandle;
            _inputReader.LookEvent += OnLookHandle;
            _inputReader.PrimaryFireEvent += OnFireHandle;
            _inputReader.PrimaryJumpEventEvent += OnJumpHandle;
            _inputReader.PrimaryDashEvent += OnDashHandle;

            // Handle ActionLoader
            _unitActionLoader.PlayActionEvent += OnPlayActionHandle;
            _unitActionLoader.FinishActionEvent += OnFinishActionHandle;
            _unitActionLoader.FrameFouceEvent += OnFrameFouceHandle;
            _unitActionLoader.FrameResetVelocityEvent += OnFrameResetVelocityHandle;

            // Handle Motion
            _animatorStateController.FinishMotionEvent += OnFinishMotionHandle;

            // Init Parameter
            _defaultSpeed = _moveSpeed;
            _defaultCrouchSpeed = _crouchSpeed;

            _isCrouth
                .Subscribe((x) =>
                {
                    if (_isNotCrouth) return;

                    var targetWeight = x ? 1 : 0;
                    var centerY = x ? _crouthCenterY : _standCenterY;
                    var height = x ? _crouthHeight : _standHeight;

                    _moveSpeed = x ? _defaultCrouchSpeed : _defaultSpeed;
                    _capsuleCollider.center = new Vector3(0, centerY, 0);
                    _capsuleCollider.height = height;
                    // DoTweenを使って0.2秒かけてweightを変更
                    DOTween.To(() => _animator.GetLayerWeight(2),
                               value => _animator.SetLayerWeight(2, value),
                               targetWeight,
                               _crouchCrossTime);
                });

            // Stamina
            _staminaTimer = _stamina;
        }
        private void Update()
        {
            if (_rigidBody.isKinematic)
                return;


            // 現在のデバイスを取得する
            //Debug.Log($"Now Control: {_inputReader.GetCurrentInputDevice()}");

            CheckGround();
            if (!IsCheckInputControl()) return;
            //if (_unitStatus.UnitState != EUnitState.Play) return;
            Rotation();
            Motion();

            if (!_isNotCrouth)
                if (Input.GetKeyDown(KeyCode.C))
                    _isCrouth.Value = !_isCrouth.Value;

            // Stamina
            if (_staminaTimer >= 0 && (_InputX != 0 || _InputY != 0) && _isDash)
                _staminaTimer -= Time.deltaTime;
            else if (_staminaTimer <= _stamina)
                _staminaTimer += Time.deltaTime * _staminaHealSpeed;
            if (_staminaTimer <= 0)
            {
                _staminaTimer = 0;
                _moveSpeed = _isCrouth.Value ? _defaultCrouchSpeed : _defaultSpeed;
            }

            StaminaUpdateEvent?.Invoke(_staminaTimer, _stamina);
        }
        private void FixedUpdate()
        {
            // ジャンプ処理
            Jump();

            if (isKnockedBack)
            {
                _rigidBody.velocity = new Vector3(knockbackDirection.x, _rigidBody.velocity.y, knockbackDirection.z);
                knockbackCounter -= Time.fixedDeltaTime;

                if (knockbackCounter <= 0)
                {
                    isKnockedBack = false;
                }
            }
            else
            {
                if (!IsCheckInputControl()) return;
                //if (_unitStatus.UnitState != EUnitState.Play) return;
                Move();
            }
        }
        private void Move()
        {
            // 追加重力
            if (_addGravity > 0)
                _rigidBody.AddForce(Vector2.down * _addGravity * Time.fixedDeltaTime, ForceMode.Acceleration);
            //if (_unitStatus.UnitState != EUnitState.Play) return;
            // 移動処理
            if (CheckSloopAngle())
                _rigidBody.velocity = new Vector3(_tpsVelocity.x * _moveSpeed, _rigidBody.velocity.y, _tpsVelocity.z * _moveSpeed);
        }
        /// <summary>
        /// ユニットのジャンプを制御
        /// </summary>
        private void Jump()
        {
            if (!IsCheckInputControl()) return;
            if (!IsJump) return;

            if (CanJump)
                _jumpCount = 0;

            if (_jumpCount < _jumpLimit)
            {
                // Y軸速度リセット
                Vector3 velocity = _rigidBody.velocity;
                velocity.y = 0;
                _rigidBody.velocity = velocity;
                // ジャンプ回数加算
                _jumpCount++;
                // ジャンプ処理
                _rigidBody.velocity = Vector3.zero;
                _rigidBody.AddForce(transform.up * _jumpPower, ForceMode.Impulse);

                // ジャンプ音声
                //AudioManager.Instance.PlayOneShotClipData(JumpVoice);
            }

            IsJump = false;
        }

        private void Rotation()
        {
            float rotY = _camera.transform.rotation.eulerAngles.y;

            // カメラから見て方角を決める
            var tpsHorizontalRotation = Quaternion.AngleAxis(rotY, Vector3.up);
            _tpsVelocity = tpsHorizontalRotation * new Vector3(_InputX, 0, _InputY).normalized;

            // ユニットの回転処理
            var rotationSpeed = _rotateSpeed * Time.deltaTime;
            // 移動方向を向く
            if (_tpsVelocity.magnitude > 0.5f)
            {
                _targetRotation = Quaternion.LookRotation(_tpsVelocity, Vector3.up);
            }
            // なめらかに振り向く ここが影響しているNavMesh
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, rotationSpeed);
        }

        private void Motion()
        {
            // 速度をAnimatorに反映する
            _animator?.SetFloat("Speed", _tpsVelocity.magnitude * _moveSpeed, 0.1f, Time.deltaTime);

            if (_tpsCliffUp != null)
                if (_tpsCliffUp.CheckCliffUp())
                    if (!_isClimb && _unitActionLoader.UnitStatus == EUnitStatus.Ready)
                    {
                        _animatorStateController?.StatePlay("ClimbUp", EStatePlayType.SinglePlay, true, true);
                        _rigidBody.velocity = Vector3.zero;
                        _isClimb = true;

                        // 操作不可
                        _unitActionLoader.ChangeStatus(EUnitStatus.Executing);
                        _rigidBody.isKinematic = true;
                    }

            return;
            if (CanJump)
                _animatorStateController?.StatePlay(LocomotionStateName, EStatePlayType.Loop, false);
            else
                _animatorStateController?.StatePlay(JumpStateName, EStatePlayType.Loop, false);
        }

        private void CheckGround()
        {
            CanJump = UtilityFunction.CheckLineData(_groundLineData, transform);
            //if (!_canJump)
            //    if (_wallCheckData != null)
            //        _actionLoader.LoadActionData(new List<UnitActionData>() { _wallCheckData });
        }
        /// <summary>
        /// 角度を検知して移動できるか判定
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool CheckSloopAngle()
        {
            float maxSlopeAngle = _maxSlopeAngle;
            float downwardAngle = 25f; // この値を調整して、Rayの下向きの角度を変更
            Vector3 forwardDown = (transform.forward - Vector3.up * Mathf.Tan(downwardAngle * Mathf.Deg2Rad)).normalized;
            Ray ray = new Ray(transform.position + Vector3.up * 0.1f, forwardDown);

            float rayDistance = _slopeDistance; // Rayの長さ
            Color rayColor = Color.blue; // Rayの色
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, rayColor);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                _slopeAngle = angle;
                return angle <= maxSlopeAngle ? true : false;
            }
            else
            {
                _slopeAngle = 0;
                return true;
            }
        }
        public bool IsCheckInputControl()
        {
            bool check = true;
            //if (IsNotInputReader) check = false; // 操作不可
            if (_unitActionLoader.UnitStatus == EUnitStatus.Executing) check = false;
            return check;
        }
        /// <summary>
        /// Velocity knockback
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="force"></param>
        public void Knockback(Vector3 direction)
        {
            // プレイヤーが向いている方向に合わせて力を加える
            Vector3 localForce = transform.forward * direction.z + transform.right * direction.x + transform.up * direction.y;
            // Rigidbody に力を加える (Impulse モードで瞬間的に力を加える)
            _rigidBody.AddForce(localForce, ForceMode.Impulse);
        }

        private void OnPlayActionHandle(ActionBase actionBase)
        {
            if (actionBase.ActionStart != null)
            {
                if (actionBase.ActionStart.IsResetVelocity)
                    OnFrameResetVelocityHandle();
            }
        }
        private void OnFinishActionHandle(ActionBase actionBase)
        {
            Debug.Log($"Finish! {actionBase}");
        }

        private void OnFinishMotionHandle(string stateName, bool flg)
        {
            if (stateName == "ClimbUp")
            {
                _animatorStateController?.StatePlay(LocomotionStateName, EStatePlayType.Loop, true, false);
                _isClimb = false;

                // 操作可能
                _unitActionLoader.ChangeStatus(EUnitStatus.Ready);
                _rigidBody.isKinematic = false;
            }
        }

        private void OnFrameFouceHandle(Vector3 power)
        {
            Knockback(power);
        }
        private void OnFrameResetVelocityHandle()
        {
            Debug.Log("Reset");
            isKnockedBack = false;
            knockbackDirection = Vector3.zero;
            knockbackCounter = 0;
            _rigidBody.velocity = Vector3.zero;
        }

        private void OnMoveHandle(Vector2 movement)
        {
            _InputX = !_isNotVertical ? movement.x : 0;
            _InputY = !_isNotHorizontal ? movement.y : 0;
        }
        private void OnLookHandle(Vector2 lookValue)
        {
            //Debug.Log($"Look:{lookValue}");
        }
        private void OnFireHandle(bool fire)
        {
            //Debug.Log($"Fire:{fire}");
            //KeyType = EKeyType.Fire1;
            //_actionLoader.LoadActionData();
        }
        private void OnJumpHandle(bool jump)
        {
            if (_isNotJump) return;
            IsJump = jump;
            //KeyType = EKeyType.Jump;
        }
        private void OnDashHandle(bool dash)
        {
            _isDash = dash;
            if (!_isCrouth.Value)
                _moveSpeed = dash ? _defaultSpeed * _dashRange : _defaultSpeed;
            else
                _moveSpeed = dash ? _defaultCrouchSpeed * _dashRange : _defaultCrouchSpeed;
            //KeyType = EKeyType.Dash;
        }


    }
}