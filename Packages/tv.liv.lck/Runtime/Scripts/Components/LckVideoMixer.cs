using System;
using System.Collections.Generic;
using System.Linq;
using Liv.Lck.Core;
using Liv.Lck.Settings;
using Liv.Lck.Telemetry;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;
using static Liv.Lck.LckEvents;

namespace Liv.Lck
{
    internal class LckVideoMixer : ILckVideoMixer
    {
        private ILckCamera _activeCamera;
        private readonly ILckEventBus _eventBus;
        private readonly ILckTelemetryClient _telemetryClient;

        public RenderTexture CameraTrackTexture { get; private set; }

        [Preserve]
        public LckVideoMixer(ILckOutputConfigurer outputConfigurer, ILckEventBus eventBus, 
            ILckTelemetryClient telemetryClient)
        {
            _eventBus = eventBus;
            _telemetryClient = telemetryClient;

            _eventBus.AddListener<CameraResolutionChangedEvent>(OnResolutionChanged);
            LckMediator.CameraRegistered += OnCameraRegistered;
            LckMediator.CameraUnregistered += OnCameraUnregistered;
            
            // Initialize texture resolution based on initial output configuration
            var initialCameraTrackDescriptor = outputConfigurer.GetActiveCameraTrackDescriptor().Result;
            UpdateTextureResolution(initialCameraTrackDescriptor.CameraResolutionDescriptor);
        }

        public LckResult<ILckCamera> GetActiveCamera()
        {
            return LckResult<ILckCamera>.NewSuccess(_activeCamera);
        }

        public LckResult ActivateCameraById(string cameraId, string monitorId = null)
        {
            var cameraToActivate = LckMediator.GetCameraById(cameraId);
            if (cameraToActivate == null)
            {
                return LckResult.NewError(LckError.CameraIdNotFound,
                    LckResultMessageBuilder.BuildCameraIdNotFoundMessage(cameraId, LckMediator.GetCameras().ToList()));
            }
            
            // Swap active camera
            _activeCamera?.DeactivateCamera();
            _activeCamera = cameraToActivate;
            _activeCamera.ActivateCamera(CameraTrackTexture);
            TriggerActiveCameraChangedEvent();

            // Update corresponding monitor's texture
            if (!string.IsNullOrEmpty(monitorId))
            {
                var updateMonitorTextureResult = UpdateMonitorTexture(monitorId);
                if (!updateMonitorTextureResult.Success)
                    return updateMonitorTextureResult;
            }

            return LckResult.NewSuccess();
        }
        
        public LckResult StopActiveCamera()
        {
            if (_activeCamera != null)
            {
                _activeCamera.DeactivateCamera();
                _activeCamera = null;
                TriggerActiveCameraChangedEvent();
            }
            
            return LckResult.NewSuccess();
        }
        
        public void Dispose()
        {
            ReleaseCameraTrackTextures();
            
            LckMediator.CameraRegistered -= OnCameraRegistered;
            LckMediator.CameraUnregistered -= OnCameraUnregistered;
        }

        private void TriggerActiveCameraChangedEvent()
        {
            TriggerActiveCameraChangedEvent(LckResult<ILckCamera>.NewSuccess(_activeCamera));
        }
        
        private void TriggerActiveCameraChangedEvent(LckResult<ILckCamera> result)
        {
            _eventBus.Trigger(new ActiveCameraChangedEvent(result));
        }
        
        private void ReleaseCameraTrackTextures()
        {
            if (!CameraTrackTexture)
                return;
            
            CameraTrackTexture.Release(); 
            UnityEngine.Object.Destroy(CameraTrackTexture);
            CameraTrackTexture = null;
            
            LckLog.Log("Released camera track texture");
        }
        
        private LckResult UpdateMonitorTexture(string monitorId)
        {
            var monitor = LckMediator.GetMonitorById(monitorId);
            if (monitor == null)
            {
                return LckResult.NewError(LckError.MonitorIdNotFound,
                    LckResultMessageBuilder.BuildMonitorIdNotFoundMessage(monitorId,
                        LckMediator.GetMonitors().ToList()));
            }
            
            monitor.SetRenderTexture(CameraTrackTexture);
            return LckResult.NewSuccess();
        }

        private static RenderTexture InitializeTargetRenderTexture(CameraResolutionDescriptor cameraResolutionDescriptor)
        {
            var width = (int)cameraResolutionDescriptor.Width;
            var height = (int)cameraResolutionDescriptor.Height;
            
#if UNITY_2020
            RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(width, height,
                RenderTextureFormat.ARGB32,  LckSettings.Instance.EnableStencilSupport ?  24 : 16)
#else
            RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(
                width,
                height,
                GraphicsFormat.R8G8B8A8_UNorm,
                LckSettings.Instance.EnableStencilSupport ? GraphicsFormat.D24_UNorm_S8_UInt : GraphicsFormat.D16_UNorm)
#endif
            {
                memoryless = RenderTextureMemoryless.None,
                useMipMap = false,
                msaaSamples = 1,
                sRGB = true,
            };

            var renderTexture = new RenderTexture(renderTextureDescriptor);
            renderTexture.antiAliasing = 1;
            renderTexture.filterMode = FilterMode.Point;
            renderTexture.name = "LCK RenderTexture";
            renderTexture.Create();

            //NOTE: These need to be called twice to make sure the ptr is available
            renderTexture.GetNativeTexturePtr();
            renderTexture.GetNativeDepthBufferPtr();
            
            return renderTexture;
        }
        
        private void InitCameraTexture(CameraResolutionDescriptor resolution)
        {
            ReleaseCameraTrackTextures();
            CameraTrackTexture = InitializeTargetRenderTexture(resolution);

            var cameras = LckMediator.GetCameras();
            if (!CameraTrackTexture) return;

            if (_activeCamera == null)
            {
                foreach (var camera in cameras)
                {
                    ActivateCameraById(camera.CameraId);
                    break;
                }
            }
            else
            {
                ActivateCameraById(_activeCamera.CameraId);
            }

            _eventBus.Trigger(new ActiveCameraTrackTextureChangedEvent(
                LckResult<RenderTexture>.NewSuccess(CameraTrackTexture)));
        }
        
        
        private void OnCameraRegistered(ILckCamera camera)
        {
            
        }

        private void OnCameraUnregistered(ILckCamera camera)
        {
            if (_activeCamera == camera)
            {
                StopActiveCamera();
            }
        }
        
        private void OnResolutionChanged(CameraResolutionChangedEvent cameraResolutionChangedEvent)
        {
            var eventResult = cameraResolutionChangedEvent.Result;
            if (!eventResult.Success)
            {
                LckLog.LogWarning($"{nameof(LckVideoMixer)} ignoring failed camera resolution change ({cameraResolutionChangedEvent.Result.Message})");
                return;
            }
            
            UpdateTextureResolution(eventResult.Result);
        }

        private void UpdateTextureResolution(CameraResolutionDescriptor resolution)
        {
            try
            {
                InitCameraTexture(resolution);
            }
            catch (Exception e)
            {
                var telemetryContext = new Dictionary<string, object>
                {
                    { "errorString", "SetTrackResolutionFailed" },
                    { "message", e.Message }
                };
                // TODO: Change error type now that this could also be related to a stream?
                _telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.RecorderError, telemetryContext));
                LckLog.LogError(e.Message);
            }
        }
    }
}
