using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using Unity.VisualScripting;
using System;
using UnityEngine.InputSystem.XInput;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using develop_common;




// https://github.com/msasaki-jyobi/3DGameKitS/blob/feature-student/Assets/_MyProject/Scripts/Input/CameraController.cs


namespace develop_tps
{
    public enum EDirection
    {
        Horizontal,
        Vertical
    }
    /// <summary>
    /// ユニットのカメラ制御を管理するクラス
    /// </summary>
    public class UnitCameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private CinemachineBrain _cinemachineBrain;
        [SerializeField] private CinemachineVirtualCamera _vcam;
        [SerializeField] private InputActionReference _axisReference;
        [SerializeField] private CinemachineCollider _cinemachineCollider;
        [SerializeField] private CinemachineInputProvider _provider;

        // Camera Parameter
        private CinemachinePOV _vcamPOV;


        private void Start()
        {
            _vcamPOV = _vcam.AddCinemachineComponent<CinemachinePOV>();

            SetCameraAxis(new Vector2(0, 0));
        }

        public void SetCameraAxis(Vector2 axis)
        {
            _vcamPOV.m_HorizontalAxis.Value = axis.x;
            _vcamPOV.m_VerticalAxis.Value = axis.y;
        }
        /// <summary>
        /// Axisの値を返す(AIM時にカメラの向きにプレイヤーを向かせる時に利用）
        /// </summary>
        /// <param name="parallel">水平・垂直</param>
        /// <returns></returns>
        public float GetAxisValue(EDirection parallel)
        {
            switch (parallel)
            {
                case EDirection.Horizontal:
                    return _vcamPOV.m_HorizontalAxis.Value;

                case EDirection.Vertical:
                    return _vcamPOV.m_VerticalAxis.Value;
            }
            return 0;
        }
        /// <summary>
        /// カメラの優先度を切り替える(別のエイムカメラに切り替える時など）
        /// </summary>
        public void SetPriority(int value = 10)
        {
            _vcam.m_Priority = value;
        }
        /// <summary>
        /// InputPrividerにセットする
        /// </summary>
        public void SetCinemachineInputProvider()
        {
            if (_provider == null)
                _provider = _vcam.AddComponent<CinemachineInputProvider>();
            _provider.XYAxis = _axisReference;
            _provider.ZAxis = _axisReference;
            _provider.gameObject.SetActive(false);
            _provider.gameObject.SetActive(true);
        }
    }
}