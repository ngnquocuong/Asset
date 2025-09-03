using Voodoo.Optimisation.Chestroom;
using Voodoo.Optimization._Common;

namespace Voodoo.Optimization.ChestRoom.InGame
{
    public class ChestRoomKey : CollectibleView
    {
        protected override void OnEatenInternal()
        {
            base.OnEatenInternal();

            ChestroomController.Instance.CollectKey();
        }


    }
}