using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region InputManager Variables
    public static InputManager Instance { get; private set; }

    private GameInput _input;
    private InputActionMap _currentActionMap;

    public InputMode CurrentInputMode { get; private set; }
    #endregion

    #region Public Input Streams
    //Add a new input stream when you need new controls. It must exist in the InputActionAsset!
    public Vector2 PlayerMovement =>
        CurrentInputMode == InputMode.Player
            ? _input.Player.Move.ReadValue<Vector2>()
            : Vector2.zero;
    public bool JumpHeld => _input.Player.Jump.IsPressed();
    public bool JumpPressed => _input.Player.Jump.WasPressedThisFrame();
    #endregion

    #region EnsureExistence Code
    //The InputManager is a singleton that will create itself in any scene that it doesn't detect itself in
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureExistence()
    {
        if (Instance != null) return;

        var prefab = Resources.Load<InputManager>("Prefabs/InputManager/InputManager");
        if (prefab == null)
        {
            Debug.LogError("<color=red>InputManager prefab not found in the Resources folder.</color> Check path or your prefab location!");
            return;
        }
        Instantiate(prefab);
        Debug.Log("<color=green>InputManager has successfully be created in the current scene.</color>");
    }
    #endregion

    #region Singleton Code
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _input = new GameInput();
        Debug.Log("<color=green>InputManager has loaded Game Input.</color>");
    }
    #endregion

    private void OnEnable()
    {
        //This will need to be sensitive to the scene. Not sure how to do that at this point. 
        SetActiveInputMap(InputMode.Player);
    }

    private void OnDisable()
    {
        _currentActionMap?.Disable();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            _currentActionMap?.Disable();
            _input?.Dispose();
            Instance = null;
        }
    }

    public void SetActiveInputMap(InputMode mode)
    {
        _currentActionMap?.Disable();
        CurrentInputMode = mode;

        _currentActionMap = mode switch
        {
            InputMode.Player => _input.Player,
            InputMode.UI => _input.UI,
            _ => _input.Camera
        };
        _currentActionMap.Enable();
    }
}

public enum InputMode
{

    Player,
    UI,
    Camera
    //Do we need a camera input mode? Adding this incase.
}