namespace EnemyStates
{
    public enum States
    {
        Idle,
        Walking,
        Attacking,
        Hit,
        Dead
    }

    public static class StatesHelper
    {
        public static string GetStateName(States state)
        {
            switch(state)
            {
                case States.Idle:
                    return "Idle";
                case States.Walking:
                    return "Walking";
                case States.Hit:
                    return "Hit";
                case States.Attacking:
                    return "Attacking";
                case States.Dead:
                    return "Dead";
            }

            return "missing state";
        }
    }
}