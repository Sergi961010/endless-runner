using UnityEngine;

namespace TheCreators.Player
{
    [CreateAssetMenu(fileName = "DigState", menuName = "PlayerStates/Dig")]
    public class NewDigState : NewPlayerState
    {
        public float _undergroundYPosition = -3.2f;
        public float _surfaceYPosition = -2f;
        public float duration = .5f;
        public float elapsedTime;
        public bool burrow;
        public override void Enter()
        {
            _context.PlayerAnimator.PlayLockedAnimation(animations[0]);
            ChangeSortingOrder(2);
            burrow = true;
        }
        public override void Exit()
        {
            ChangeSortingOrder(0);
        }
        public override void LogicUpdate()
        {
            _context.PlayerAnimator.PlayAnimation(animations[1]);
        }
        public override void PhysicsUpdate()
        {
            if (burrow)
                HandleBurrow();
            if (_context.InputManager.SwipeDetection.UnburrowPerformed)
            {
                _context.PlayerAnimator.PlayLockedAnimation(animations[2]);
                HandleUnburrow();
            }
        }
        private void HandleBurrow()
        {
            Vector2 newPosition = _context.transform.position;
            float percentageComplete = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            newPosition.x = _context.transform.position.x + _context.RB.velocity.x * Time.deltaTime;
            newPosition.y = Mathf.Lerp(_surfaceYPosition, _undergroundYPosition, percentageComplete);
            _context.RB.isKinematic = true;
            _context.RB.MovePosition(newPosition);

            if (elapsedTime >= duration)
            {
                elapsedTime = 0;
                burrow = false;
            }
        }
        private void HandleUnburrow()
        {
            Vector2 newPosition = _context.transform.position;
            float percentageComplete = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            newPosition.x = _context.transform.position.x + _context.RB.velocity.x * Time.deltaTime;
            newPosition.y = Mathf.Lerp(_undergroundYPosition, _surfaceYPosition, percentageComplete);
            _context.RB.MovePosition(newPosition);

            if (elapsedTime >= duration)
            {
                _context.RB.isKinematic = false;
                elapsedTime = 0;
                _context.StateMachine.SwitchState(_context.runState);
            }
        }
        private void ChangeSortingOrder(int value)
        {
            _context.SpriteRenderer.sortingOrder = value;
        }
    }
}