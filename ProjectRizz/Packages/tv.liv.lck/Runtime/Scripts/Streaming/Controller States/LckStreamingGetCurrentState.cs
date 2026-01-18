using Liv.Lck.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Liv.Lck.Streaming
{
    public class LckStreamingGetCurrentState : LckStreamingBaseState
    {
        public override void EnterState(LckStreamingController controller)
        {
            _ = GetCurrentState(controller, controller.CancellationTokenSource.Token);
        }

        private async Task GetCurrentState(LckStreamingController controller, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                controller.Log("currently waiting for get current state");

                var isSubscribed = await controller.LckCore.IsUserSubscribed();

                if (isSubscribed.IsOk == false)
                {
                    switch (isSubscribed.Err)
                    {
                        case CoreError.UserNotLoggedIn:
                            // go back to login show code state
                            controller.SwitchState(controller.ShowCodeState);
                            return;
                        case CoreError.InternalError:
                            controller.LogError($"Internal error checking if user is Subscribed: {isSubscribed.Err} - {isSubscribed.Message}");
                            controller.SwitchState(controller.InternalErrorState);
                            return;
                        case CoreError.InvalidArgument:
                            controller.LogError($"Invalid Argument error checking if user is Subscribed: {isSubscribed.Err} - {isSubscribed.Message}");
                            controller.SwitchState(controller.InvalidArgumentState);
                            return;
                        case CoreError.MissingTrackingId:
                            controller.LogError($"MissingTrackingId error please make sure Tracking ID is setup correctly in LCK Settings: {isSubscribed.Err} - {isSubscribed.Message}");
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
                    if (isSubscribed.Ok)
                    {
                        // user is subscribed, go check if the platform is connected
                        controller.SwitchState(controller.WaitingForConfigureState);
                        return;
                    }
                    else
                    {
                        // logged in but not subscribed
                        controller.SwitchState(controller.CheckSubscribedState);
                        return;
                    }
                }
            }
        }
    }
}
