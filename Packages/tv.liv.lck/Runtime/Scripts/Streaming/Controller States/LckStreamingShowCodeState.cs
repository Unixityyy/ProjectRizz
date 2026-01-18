using Liv.Lck.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Liv.Lck.Streaming
{
    public class LckStreamingShowCodeState : LckStreamingBaseState
    {
        public override void EnterState(LckStreamingController controller)
        {
            controller.SetNotificationStreamCode("Loading...");
            controller.ShowNotification(Tablet.NotificationType.EnterStreamCode);
            
            _ = GetCodeFromCore(controller, controller.CancellationTokenSource.Token);
        }

        private async Task GetCodeFromCore(LckStreamingController controller, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                controller.Log("currently waiting to get code from core");

                var loginResult = await controller.LckCore.StartLoginAttemptAsync();
                if (loginResult.IsOk)
                {
                    var formattedCode = loginResult.Ok.Insert(3, "-");
                    controller.SetNotificationStreamCode(formattedCode);
                    _ = WaitForUserToPairTablet(controller, controller.CancellationTokenSource.Token);
                    return;
                }
                else
                {
                    switch (loginResult.Err)
                    {
                        case CoreError.UserNotLoggedIn:
                            // continue the StartLoginAttemptAsync checks
                            break;
                        case CoreError.InternalError:
                            controller.LogError($"Internal error checking the StartLoginAttemptAsync: {loginResult.Err} - {loginResult.Message}");
                            controller.SwitchState(controller.InternalErrorState);
                            return;
                        case CoreError.InvalidArgument:
                            controller.LogError($"Invalid Argument error while running StartLoginAttemptAsync: {loginResult.Err} - {loginResult.Message}");
                            controller.SwitchState(controller.InvalidArgumentState);
                            return;
                        case CoreError.MissingTrackingId:
                            controller.LogError($"MissingTrackingId error please make sure Tracking ID is setup correctly in LCK Settings: {loginResult.Err} - {loginResult.Message}");
                            controller.SwitchState(controller.MissingTrackingIdState);
                            return;
                        default:
                            controller.LogError("Tried to check an LCKCore Error missing from this switch statement");
                            controller.SwitchState(controller.GetCurrentState);
                            return;
                    }
                }

                await Task.Delay(1000, cancellationToken);
            }
        }

        private async Task WaitForUserToPairTablet(LckStreamingController controller, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                controller.Log("currently waiting for user to pair tablet");

                var loginResult = await controller.LckCore.CheckLoginCompletedAsync();
                if (loginResult.IsOk)
                {
                    if (loginResult.Ok)
                    {
                        // Login completed, go check if subscribed
                        controller.SwitchState(controller.CheckSubscribedState);
                        return;
                    }
                }
                else
                {
                    switch (loginResult.Err)
                    {
                        case CoreError.UserNotLoggedIn:
                            // continue with next CheckLoginCompletedAsync
                            break;
                        case CoreError.InternalError:
                            controller.LogError($"Internal error while running CheckLoginCompletedAsync: {loginResult.Err} - {loginResult.Message}");
                            controller.SwitchState(controller.InternalErrorState);
                            return;
                        case CoreError.InvalidArgument:
                            controller.LogError($"Invalid Argument error while running CheckLoginCompletedAsync: {loginResult.Err} - {loginResult.Message}");
                            controller.SwitchState(controller.InvalidArgumentState);
                            return;
                        case CoreError.MissingTrackingId:
                            controller.LogError($"MissingTrackingId error please make sure Tracking ID is setup correctly in LCK Settings: {loginResult.Err} - {loginResult.Message}");
                            controller.SwitchState(controller.MissingTrackingIdState);
                            return;
                        default:
                            controller.LogError("Tried to check an LCKCore Error missing from this switch statement");
                            controller.SwitchState(controller.GetCurrentState);
                            return;
                    }
                }

                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
