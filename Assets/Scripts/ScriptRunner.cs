using UnityEngine;
using GorillaLocomotion;
using Jint;
using Jint.Runtime;
using System;
using System.Reflection;
using System.IO;
using Photon.VR;
using Photon.Pun;
using easyInputs;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections;

public class ScriptRunner : MonoBehaviour
{
    public class PlayerBindings
    {
        private readonly Player _player;
        private readonly FieldInfo _velocityField;

        public PlayerBindings(Player locomotionScript)
        {
            _player = locomotionScript;
            _velocityField = typeof(Player).GetField(
                "denormalizedVelocityAverage",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            if (_velocityField == null)
            {
                Debug.LogError("Reflection failed: 'denormalizedVelocityAverage' field not found on Player script.");
            }
        }

        public Vector3 CurrentVelocityAverage
        {
            get
            {
                if (_velocityField != null)
                {
                    return (Vector3)_velocityField.GetValue(_player);
                }
                return Vector3.zero;
            }
        }

        public Vector3 Position
        {
            get => _player.gameObject.transform.position;
        }

        public int Material
        {
            get => _player.JSMaterial;
            set => _player.JSMaterial = value;
        }

        public string Name
        {
            get => PhotonNetwork.NickName;
        }
    }

    public class PlayerSettings
    {
        private readonly Player _player;

        public PlayerSettings(Player locomotionScript)
        {
            _player = locomotionScript;
        }

        public float MaxJumpSpeed
        {
            get => _player.maxJumpSpeed;
            set
            {
                if (value >= 0)
                {
                    _player.maxJumpSpeed = value;
                }
                else
                {
                    Debug.LogWarning("JS attempted to set MaxJumpSpeed to a negative value. Canceled.");
                }
            }
        }

        public float JumpMultiplier
        {
            get => _player.jumpMultiplier;
            set
            {
                if (value > 0)
                {
                    _player.jumpMultiplier = value;
                }
                else
                {
                    Debug.LogWarning("JS attempted to set JumpMultiplier to a negative value. Canceled.");
                }
            }
        }

        public float VibrateIntensity
        {
            get => _player.VibrateIntensity;
            set => _player.VibrateIntensity = Mathf.Clamp(value, 0f, 1f);
        }

        public float VibrateDuration
        {
            get => _player.VibrateDuration;
            set => _player.VibrateDuration = value;
        }
    }

    public class LeftController
    {
        private readonly Player _player;

        public LeftController(Player locomotionScript)
        {
            _player = locomotionScript;
        }

        public bool XButtonPressed
        {
            get => EasyInputs.GetPrimaryButtonDown(EasyHand.LeftHand);
        }

        public bool YButtonPressed
        {
            get => EasyInputs.GetSecondaryButtonDown(EasyHand.LeftHand);
        }

        public Vector2 Thumbstick2DAxis
        {
            get => EasyInputs.GetThumbStick2DAxis(EasyHand.LeftHand);
        }

        public bool ThumbstickPressed
        {
            get => EasyInputs.GetThumbStickButtonDown(EasyHand.LeftHand);
        }

        public float TriggerValue
        {
            get => EasyInputs.GetTriggerButtonFloat(EasyHand.LeftHand);
        }

        public float GripValue
        {
            get => EasyInputs.GetGripButtonFloat(EasyHand.LeftHand);
        }
    }

    public class RightController
    {
        private readonly Player _player;

        public RightController(Player locomotionScript)
        {
            _player = locomotionScript;
        }

        public bool AButtonPressed
        {
            get => EasyInputs.GetPrimaryButtonDown(EasyHand.RightHand);
        }

        public bool BButtonPressed
        {
            get => EasyInputs.GetSecondaryButtonDown(EasyHand.RightHand);
        }

        public Vector2 Thumbstick2DAxis
        {
            get => EasyInputs.GetThumbStick2DAxis(EasyHand.RightHand);
        }

        public bool ThumbstickPressed
        {
            get => EasyInputs.GetThumbStickButtonDown(EasyHand.RightHand);
        }

        public float TriggerValue
        {
            get => EasyInputs.GetTriggerButtonFloat(EasyHand.RightHand);
        }

        public float GripValue
        {
            get => EasyInputs.GetGripButtonFloat(EasyHand.RightHand);
        }
    }

    private string jsCode;

    [Header("References")]
    public Player player;

    private Engine _engine;
    private bool _scriptLoaded = false;
    public static ScriptRunner instance;
    [Header("DO NOT CHANGE THIS. THIS WILL GET SET AT RUNTIME!!!!!!!!!!!!!")]
    public bool _hasAssetPermission;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            _hasAssetPermission = PlayerPrefs.GetInt("assetAgreement", 0) == 1;
        }
    }

    public IEnumerator LoadWarning()
    {
        SceneManager.LoadScene("DangerousAssets");
        while (SceneManager.GetActiveScene().name != "MonkeTag")
        {
            yield return null;
        }
    }

    private void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "JS/main.js");
        if (File.Exists(filePath))
        {
            try
            {
                jsCode = File.ReadAllText(filePath);
                Debug.Log($"Successfully loaded script from: {filePath}");
                StartCoroutine(engineInit(jsCode));
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to read file at {filePath}: {ex.Message}");
            }
        }
        else
        {
            Debug.Log($"No custom script.");
        }
    }

    IEnumerator engineInit(string jsCode)
    {
        while (SceneManager.GetActiveScene().name != "MonkeTag")
        {
            yield return null;
        }
        player = Player.Instance;
        InitializeEngine(jsCode);
    }

    private void InitializeEngine(string scriptCode)
    {
        try
        {
            _engine = new Engine(cfg =>
            {
                cfg.TimeoutInterval(TimeSpan.FromSeconds(5));
                cfg.MaxStatements(100000);
            });

            _engine.SetValue("log", new Action<object>(msg => Debug.Log($"JS: {msg}")));
            _engine.SetValue("hasAssetPermission", new Func<bool>(() => {
                return _hasAssetPermission; 
            }));
            var playerInstance = new PlayerBindings(Player.Instance);
            var settingsInstance = new PlayerSettings(Player.Instance);
            var leftController = new LeftController(Player.Instance);
            var rightController = new RightController(Player.Instance);
            _engine.SetValue("LocalPlayer", playerInstance);
            _engine.SetValue("PlayerSettings", settingsInstance);
            _engine.SetValue("RightController", rightController);
            _engine.SetValue("LeftController", leftController);

            _engine.Execute(scriptCode);
            _scriptLoaded = true;

            if (!_engine.Global.HasOwnProperty("tick"))
            {
                Debug.LogWarning("JS: Script does not define a 'tick(deltaTime)' function. Per-frame execution will be skipped.");
            }
        }
        catch (JavaScriptException jex)
        {
            Debug.LogError($"[JINT] JS Initialization Error: {jex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ScriptRunner] Host Initialization Error: {ex.Message}");
        }
    }

    private void Update()
    {
        if (_scriptLoaded && _engine != null)
        {
            ExecuteTick(Time.deltaTime);
        }
    }

    private void ExecuteTick(float deltaTime)
    {
        try
        {
            if (_engine.Global.HasOwnProperty("tick"))
            {
                _engine.Invoke("tick", deltaTime);
            }
        }
        catch (JavaScriptException jex)
        {
            Debug.LogError($"[JINT] JS Runtime Error in tick(): {jex.Message}");
            _scriptLoaded = false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ScriptRunner] Host Error in tick(): {ex.Message}");
            _scriptLoaded = false;
        }
    }
}