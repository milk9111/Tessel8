
namespace EnemyStates.SkeletonStates
{
    public class SkeletonIdle : BaseState
    {
        private bool _inStoppingDistance;
        
        public override void DoAction()
        {
            _animator.SetBool("Walking", !_inStoppingDistance);
            
            if (!_inStoppingDistance)
            {
                _controller.ChangeState(States.Walking);
            }
        }

        public void SetInStoppingDistance(bool inStoppingDistance)
        {
            _inStoppingDistance = inStoppingDistance;
        }
    }
}