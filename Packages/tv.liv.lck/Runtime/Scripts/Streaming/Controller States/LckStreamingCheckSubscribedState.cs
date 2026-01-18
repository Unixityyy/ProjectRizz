using Liv.Lck.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Liv.Lck.Streaming
{
    public class LckStreamingCheckSubscribedState : LckStreamingBaseState
    {
        public override void EnterState(LckStreamingController controller)
        {
             controller.ShowNotification(Tablet.NotificationType.CheckSubscribed);
             _ = CheckSubscribedState(controller, controller.CancellationTokenSource.Token);
        }

        private async Task CheckSubscribedState(LckStreamingController controller, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                controller.Log("currently waiting check subscribed state");
                var result = await controller.LckCore.IsUserSubscribed();

                if (result.IsOk == false)
                {
                    switch (result.Err)
                    {
                        case CoreError.UserNotLoggedIn:
                            // go back to login show code state
                            controller.SwitchState(controller.ShowCodeState);
                            return;
                        case CoreError.InternalError:
                            controller.LogError($"Internal error checking if user is Subscribed: {result.Err} - {result.Message}");
                            controller.SwitchState(controller.InternalErrorState);
                            return;
                        case CoreError.InvalidArgument:
                            controller.LogError($"Invalid Argument error checking if user is Subscribed: {result.Err} - {result.Message}");
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
                        // user is subscribed
                        controller.SwitchState(controller.WaitingForConfigureState);
                        return;
                    }
                    else
                    {
                        // logged in but not subscribed
                    }
                }

                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
