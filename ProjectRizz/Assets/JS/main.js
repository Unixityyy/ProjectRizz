let time = console.time()

function tick(dt)
{
    if (console.time() - time >= 1000)
    {
        time = console.time()
        if (LocalPlayer.Material == 1)
        {
            LocalPlayer.Material = 0
        }
        else
        {
            LocalPlayer.Material = 1
        }
    }
}