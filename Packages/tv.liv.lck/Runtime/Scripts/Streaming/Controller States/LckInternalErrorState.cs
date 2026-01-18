using Liv.Lck.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Liv.Lck.Streaming
{
    public class LckInternalErrorState : LckStreamingBaseState
    {
        public override void EnterState(LckStreamingController controller)
        {
            controller.ShowNotification(Tablet.NotificationType.InternalError);
            _ = CheckInternalError(controller, controller.CancellationTokenSource.Token);
        }

        private async Task CheckInternalError(LckStreamingController controller, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                controller.Log("currently checking Internal Error state");

                var result = await controller.LckCore.HasUserConfiguredStreaming();

                if (result.IsOk == false)
                {
                    switch (result.Err)
                    {
                        case CoreError.UserNotLoggedIn:
                            // managed to reach backend again, go back to login show code state
                            controller.SwitchState(controller.ShowCodeState);
                            return;
                        case CoreError.InternalError:
                            // no internet or can't reach backend, continue checking in this state
                            break;
                        case CoreError.InvalidArgument:
                            controller.LogError($"Invalid Argument error checking HasUserConfiguredStreaming: {result.Err} - {result.Message}");
                            controller.SwitchState(controller.InvalidArgumentState);
                            return;
                        case CoreError.MissingTrackingId:
                            controller.LogError($"MissingTrackingId error please make sure Tracking ID is setup correctly in LCK Settings: {result.Err} - {result.Message}");
                            controller.SwitchState(controller.MissingTrackingIdState);
                            return;
                        default:
                            controller.LogError("Tried to check an LCKCore Error missing from this switch statement");
                            controller.SwitchState(controller.GetCurrentState);
                            return;
                    }
                }
                else
                {
                    if (result.Ok)
                    {
                        // logged in and stream config is good
                        controller.SwitchState(controller.ConfiguredCorrectlyState);
                        return;
                    }
                    else
                    {
                        // user is logged in but isn't configured yet
                        controller.SwitchState(controller.CheckSubscribedState);
                        return;
                    }
                }

                await Task.Delay(3000, cancellationToken);
            }
        }
    }
}
