using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using AOT;

namespace Liv.Lck.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GameInfo
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string GameName;
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string GameVersion;
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string ProjectName;
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string CompanyName;
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string EngineVersion;
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string RenderPipeline;
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string GraphicsAPI;
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string Platform;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LckInfo
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string Version;
        public int BuildNumber;
    }

    /// <summary>
    /// Specifies the verbosity of the LCK SDK's internal logging.
    /// Use this to control the amount of diagnostic information printed to the console.
    /// </summary>
    public enum LevelFilter
    {
        /// <summary>Disables all logging from the SDK.</summary>
        Off,
        /// <summary>Logs only critical errors.</summary>
        Error,
        /// <summary>Logs errors and warnings.</summary>
        Warn,
        /// <summary>Logs errors, warnings, and informational messages.</summary>
        Info,
        /// <summary>Logs all messages, including detailed debug information.</summary>
        Debug,
        /// <summary>Logs all messages, including highly verbose trace information for deep debugging.</summary>
        Trace,
    }

    /// <summary>
    /// Represents high-level error categories that can be returned by SDK operations.
    /// Check the 'Err' property on a failed Result to identify the cause of failure.
    /// </summary>
    public enum CoreError
    {
        /// <summary>An unexpected internal error occurred within the SDK.</summary>
        InternalError = 0,
        /// <summary>The 'trackingId' provided during initialisation was missing or invalid.</summary>
        MissingTrackingId = 1,
        /// <summary>An invalid argument was passed to an SDK method.</summary>
        InvalidArgument = 2,
        /// <summary>The operation could not be completed because the user is not logged in.</summary>
        UserNotLoggedIn = 3,
    }

    /// <summary>
    /// A generic class that encapsulates the result of an SDK operation. Operations can either
    /// succeed and return a value, or fail and return an error. This object cleanly represents
    /// both outcomes without using exceptions for control flow.
    /// </summary>
    /// <typeparam name="T">The type of the value returned on success.</typeparam>
    public class Result<T>
    {
        private readonly bool _success;
        private readonly string _message;
        private readonly CoreError? _error;
        private readonly T _result;

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// **Always check this property first.** If this is <c>true</c>, you can safely access the <see cref="Ok"/> property.
        /// If this is <c>false</c>, check the <see cref="Err"/> and <see cref="Message"/> properties for failure details.
        /// </summary>
        public bool IsOk => _success;

        /// <summary>
        /// Gets the detailed error message if the operation failed.
        /// This property is only meaningful when <see cref="IsOk"/> is <c>false</c>.
        /// </summary>
        public string Message => _message;

        /// <summary>
        /// Gets the high-level error category if the operation failed.
        /// This property is only meaningful when <see cref="IsOk"/> is <c>false</c>.
        /// </summary>
        public CoreError? Err => _error;

        /// <summary>
        /// Gets the successful result of the operation.
        /// **Warning:** Only access this property after confirming that <see cref="IsOk"/> is <c>true</c>.
        /// Accessing it on a failed result will return the default value for type <typeparamref name="T"/> (e.g., null, 0, or false).
        /// </summary>
        public T Ok => _result;

        private Result(bool success, string message, CoreError? error, T result)
        {
            _success = success;
            _message = message;
            _error = error;
            _result = result;
        }

        /// <summary>
        /// Creates a new <see cref="Result{T}"/> object representing a successful operation.
        /// </summary>
        /// <param name="result">The value to wrap.</param>
        public static Result<T> NewSuccess(T result)
        {
            return new Result<T>(true, null, null, result);
        }

        /// <summary>
        /// Creates a new <see cref="Result{T}"/> object representing a failed operation.
        /// </summary>
        /// <param name="error">The error category.</param>
        /// <param name="message">The detailed error message.</param>
        public static Result<T> NewError(CoreError error, string message)
        {
            return new Result<T>(false, message, error, default(T));
        }
    }

    /// <summary>
    /// The primary static class for interacting with the LCK Core SDK.
    /// It provides methods for initialisation, authentication, and querying user status.
    /// Generally, it would not be recommended to modify or re-implement this class.
    /// Should you desire a custom UX flow for streaming, see <see cref="Liv.Lck.Streaming.LckStreamingController"/>.
    /// </summary>
    public static class LckCore
    {
        private static ReturnCode _lastReturnCode;
        private static string _loginCode;
        
        [MonoPInvokeCallback(typeof(LckCoreNative.start_login_attempt_callback_delegate))]
        private static void StartLoginAttemptCallback(ReturnCode returnCode, string loginCode)
        {
            _lastReturnCode = returnCode;
            if (returnCode == ReturnCode.Ok)
            {
                _loginCode = String.Copy(loginCode);
            }
        }

        public static void SetMaxLogLevel(LevelFilter levelFilter)
        {
            LckCoreNative.set_max_log_level(levelFilter);
        }

        /// <summary>
        /// Initialises the LCK SDK. This method must be called once, typically during your
        /// game's startup sequence, before any other LCK SDK methods are used.
        /// Currently called by a 'RuntimeInitializeOnLoadMethod' through <see cref="LckCoreHandler"/>.
        /// </summary>
        /// <param name="trackingId">Your unique application tracking ID provided by LIV.</param>
        /// <param name="gameInfo">A struct containing details about your game and its engine configuration.</param>
        /// <param name="lckInfo">A struct containing details about the LCK package being used.</param>
        /// <returns>A <see cref="Result{T}"/> of type <c>bool</c> indicating if initialisation was successful.</returns>
        public static Result<bool> Initialize(string trackingId, GameInfo gameInfo, LckInfo lckInfo)
        {
            if (string.IsNullOrEmpty(trackingId))
            {
                return Result<bool>.NewError(CoreError.MissingTrackingId, "Tracking ID cannot be null or empty.");
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            using(var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using(var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                IntPtr ctxPtr = activity.GetRawObject();
                var androidResult = LckCoreNative.initialize_android(ctxPtr);

                if (androidResult != ReturnCode.Ok) {
                    return Result<bool>.NewError(CoreError.InternalError, $"Failed to initialize LckCore for Android: {androidResult}");
                }
            }
#endif

            var result = LckCoreNative.initialize(trackingId, gameInfo, lckInfo);

            if (result != ReturnCode.Ok)
            {
                if (result == ReturnCode.InvalidArgument)
                {
                    return Result<bool>.NewError(CoreError.InvalidArgument, "Invalid argument provided to initialize LckCore.");
                }
                else
                {
                    return Result<bool>.NewError(CoreError.InternalError, $"Failed to initialize LckCore: {result}");
                }
            }

            return Result<bool>.NewSuccess(true);
        }

        public static async Task<Result<bool>> HasUserConfiguredStreaming()
        {
            var returnCode = ReturnCode.Ok;
            var hasConfigured = false;

            IntPtr hasConfiguredPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(bool)));
            Marshal.WriteByte(hasConfiguredPtr, 0);

            await Task.Run(() =>
            {
                returnCode = LckCoreNative.has_user_configured_streaming(hasConfiguredPtr);
            });

            if (returnCode == ReturnCode.Ok)
            {
                hasConfigured = Marshal.ReadByte(hasConfiguredPtr) != 0;
            }

            Marshal.FreeHGlobal(hasConfiguredPtr);

            if (returnCode != ReturnCode.Ok)
            {
                if (returnCode == ReturnCode.UserNotLoggedIn)
                {
                    return Result<bool>.NewError(CoreError.UserNotLoggedIn, "User is not logged in.");
                }
                else
                {
                    return Result<bool>.NewError(CoreError.InternalError, $"Failed to check user configuration: {returnCode}");
                }
            }

            return Result<bool>.NewSuccess(hasConfigured);
        }

        public static async Task<Result<bool>> IsUserSubscribed()
        {
            var returnCode = ReturnCode.Ok;
            var isSubscribed = false;

            IntPtr isSubscribedPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(bool)));
            Marshal.WriteByte(isSubscribedPtr, 0);

            await Task.Run(() =>
            {
                returnCode = LckCoreNative.is_user_subscribed(isSubscribedPtr);
            });

            if (returnCode == ReturnCode.Ok)
            {
                isSubscribed = Marshal.ReadByte(isSubscribedPtr) != 0;
            }

            Marshal.FreeHGlobal(isSubscribedPtr);

            if (returnCode != ReturnCode.Ok)
            {
                if (returnCode == ReturnCode.UserNotLoggedIn)
                {
                    return Result<bool>.NewError(CoreError.UserNotLoggedIn, "User is not logged in.");
                }
                else
                {
                    return Result<bool>.NewError(CoreError.InternalError, $"Failed to check user subscription: {returnCode}");
                }
            }

            return Result<bool>.NewSuccess(isSubscribed);
        }

        public static async Task<Result<string>> StartLoginAttemptAsync()
        {
            var returnCode = ReturnCode.Ok;

            Debug.Log("Starting login attempt task...");
            await Task.Run(() =>
            {
                var result = LckCoreNative.start_login_attempt(StartLoginAttemptCallback);

                if (result != ReturnCode.Ok)
                {
                    returnCode = result;
                }
            });

            Debug.Log($"Login attempt task completed with return code: {returnCode}");

            if (returnCode != ReturnCode.Ok || _loginCode == null)
            {
                return Result<string>.NewError(CoreError.InternalError, $"Failed to start login attempt: {returnCode}");
            }
            else {
                return Result<string>.NewSuccess(_loginCode);
            }
        }

        public static async Task<Result<bool>> CheckLoginCompletedAsync()
        {
            var returnCode = ReturnCode.Ok;
            var isComplete = false;

            Debug.Log("Starting check login completed task...");
            await Task.Run(() =>
            {
                IntPtr completePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(bool)));
                Marshal.WriteByte(completePtr, 0);

                var result = LckCoreNative.check_login_attempt_completed(completePtr);

                if (result != ReturnCode.Ok)
                {
                    returnCode = result;
                }
                else
                {
                    isComplete = Marshal.ReadByte(completePtr) != 0;
                }

                Marshal.FreeHGlobal(completePtr);
            });

            Debug.Log($"Check login completed task finished with return code: {returnCode}, isComplete: {isComplete}");

            if (returnCode != ReturnCode.Ok)
            {
                return Result<bool>.NewError(CoreError.InternalError, $"Failed to check login completed: {returnCode}");
            }
            else
            {
                return Result<bool>.NewSuccess(isComplete);
            }
        }

        // Called from Editor only
        public static void Dispose()
        {
            var result = LckCoreNative.dispose();

            if (result != ReturnCode.Ok)
            {
                throw new InvalidOperationException($"Failed to dispose LckCore: {result}");
            }
        }
    }
}
